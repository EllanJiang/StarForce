/*
* 文件名：NetSocket
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/21 19:17:30
* 修改记录：
*/

#if UNITY_5_3_OR_NEWER
#define UNITY
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine;
#endif
#endif
#if NETSTANDARD || NETCOREAPP
using System.Runtime.InteropServices;
#endif

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LogicShared.LiteNetLib
{
#if UNITY_IOS && !UNITY_EDITOR
    public class UnitySocketFix : MonoBehaviour
    {
        internal IPAddress BindAddrIPv4;
        internal IPAddress BindAddrIPv6;
        internal bool Reuse;
        internal IPv6Mode IPv6;
        internal int Port;
        internal bool Paused;
        internal NetSocket Socket;

        private void Update()
        {
            if (Socket == null)
                Destroy(gameObject);
        }

        private void OnApplicationPause(bool pause)
        {
            if (Socket == null)
                return;
            if (pause)
            {
                Paused = true;
                Socket.Close(true);
            }
            else if (Paused)
            {
                if (!Socket.Bind(BindAddrIPv4, BindAddrIPv6, Port, Reuse, IPv6))
                {
                    LogicShared.Logger.Error($"[S] Cannot restore connection \"{BindAddrIPv4}\",\"{BindAddrIPv6}\" port {Port}");
                    Socket.OnErrorRestore();
                }
            }
        }
    }
#endif
    
    /// <summary>
    /// 监听socket接口
    /// </summary>
    internal interface INetSocketListener
    {
        /// <summary>
        /// 处理收到socket发送过来的数据/消息/网络包
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="errorCode"></param>
        /// <param name="remoteEndPoint"></param>
        void OnMessageReceived(byte[] data, int length, SocketError errorCode, IPEndPoint remoteEndPoint);
    }
    
    /// <summary>
    /// socket相关操作方法
    /// </summary>
    internal sealed class NetSocket
    {
        public const int ReceivePollingTime = 500000;   //0.5 second，Polling时长
        private Socket _udpSocketv4;                    
        private Socket _udpSocketv6;
        private Thread _threadv4;
        private Thread _threadv6;
        private readonly INetSocketListener _listener;  //监听接口
        //参考：https://www.cnblogs.com/ganzhihui/p/9696217.html
        private const int SioUdpConnreset = -1744830452; //SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12
        private static readonly IPAddress MulticastAddressV6 = IPAddress.Parse("ff02::1");  //多点广播ipb6地址
        internal static readonly bool IPv6Support;       //是否支持IPv6
        
#if UNITY_IOS && !UNITY_EDITOR
        private UnitySocketFix _unitySocketFix;

        public void OnErrorRestore()
        {
            Close(false);
            _listener.OnMessageReceived(null, 0, SocketError.NotConnected, new IPEndPoint(0,0));
        }
#endif
        
        public int LocalPort { get; private set; }      //本地端口
        //volatile无法保证线程安全
        //volatile只能修饰字段，不能修饰属性，局部变量
        //多线程操作volatile修饰的字段时应该使用lock
        //volatile关键字指示一个字段可以由多个正在执行中的线程进行修改。
        //volatile的作用：保证字段在任何时间呈现的值都是最新的值，保证字段的原子性和可见性（原子性：同一时刻只能有一个线程对该字段进行修改;可见性：一个线程对该字段的修改可以及时地被其他线程观察到）,但是不保证有序性。
        //volatile的缺点：存取volatile修饰的字段会比一般字段消耗的资源要多一点，因为任何线程修改了volatile修饰的字段都会立刻同步给其他线程
        //参考：https://www.cnblogs.com/caoyue777/archive/2008/06/29/1232195.html
        public volatile bool IsRunning;                 //socket是否正在运行
        
        /// <summary>
        /// Time To Live
        /// </summary>
        public short Ttl
        {
            get
            {
#if UNITY_SWITCH
                return 0;
#else
                if (_udpSocketv4.AddressFamily == AddressFamily.InterNetworkV6)
                    return (short)_udpSocketv4.GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.HopLimit);
                return _udpSocketv4.Ttl;
#endif
            }
            set
            {
#if !UNITY_SWITCH
                if (_udpSocketv4.AddressFamily == AddressFamily.InterNetworkV6)
                    _udpSocketv4.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.HopLimit, value);
                else
                    _udpSocketv4.Ttl = value;
#endif
            }
        }
        
        static NetSocket()
        {
#if DISABLE_IPV6 || (!UNITY_EDITOR && ENABLE_IL2CPP && !UNITY_2018_3_OR_NEWER)
            IPv6Support = false;
#elif !UNITY_2019_1_OR_NEWER && !UNITY_2018_4_OR_NEWER && (!UNITY_EDITOR && ENABLE_IL2CPP && UNITY_2018_3_OR_NEWER)
            string version = UnityEngine.Application.unityVersion;
            IPv6Support = Socket.OSSupportsIPv6 && int.Parse(version.Remove(version.IndexOf('f')).Split('.')[2]) >= 6;
#elif UNITY_2018_2_OR_NEWER
            IPv6Support = Socket.OSSupportsIPv6;
#elif UNITY
#pragma warning disable 618
            IPv6Support = Socket.SupportsIPv6;
#pragma warning restore 618
#else
            IPv6Support = Socket.OSSupportsIPv6;
#endif
        }

        public NetSocket(INetSocketListener listener)
        {
            _listener = listener;
        }

        /// <summary>
        /// socket是否还在运行
        /// </summary>
        /// <returns></returns>
        private bool IsActive()
        {
#if UNITY_IOS && !UNITY_EDITOR
            var unitySocketFix = _unitySocketFix; //save for multithread
            if (unitySocketFix != null && unitySocketFix.Paused)
                return false;
#endif
            return IsRunning;
        }

        /// <summary>
        /// socket接收网络包接口
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveLogic(object state)
        {
            Socket socket = (Socket)state;
            EndPoint bufferEndPoint = new IPEndPoint(socket.AddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, 0);
            byte[] receiveBuffer = new byte[NetConstants.MaxPacketSize];

            while (IsActive())
            {
                int result;

                //Reading data
                try
                {
                    if (socket.Available == 0 && !socket.Poll(ReceivePollingTime, SelectMode.SelectRead))
                        continue;
                    result = socket.ReceiveFrom(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None,
                        ref bufferEndPoint);
                }
                catch (SocketException ex)
                {
                    switch (ex.SocketErrorCode)
                    {
#if UNITY_IOS && !UNITY_EDITOR
                        case SocketError.NotConnected:
#endif
                        case SocketError.Interrupted:
                        case SocketError.NotSocket:
                            return;
                        case SocketError.ConnectionReset:
                        case SocketError.MessageSize:
                        case SocketError.TimedOut:
                            LogicShared.Logger.Error($"[R]Ignored error: {(int)ex.SocketErrorCode} - {ex.ToString()}");
                            break;
                        default:
                            LogicShared.Logger.Error($"[R]Error code: {(int)ex.SocketErrorCode} - {ex.ToString()}");
                            _listener.OnMessageReceived(null, 0, ex.SocketErrorCode, (IPEndPoint)bufferEndPoint);
                            break;
                    }
                    continue;
                }
                catch (ObjectDisposedException)
                {
                    return;
                }

                //All ok!
                //LogicShared.Logger.Error($"[R]Received data from {bufferEndPoint.ToString()}, Received data bytes count: {result}" );
                _listener.OnMessageReceived(receiveBuffer, result, 0, (IPEndPoint)bufferEndPoint);
            }
        }

        /// <summary>
        /// 绑定socket（Udp）
        /// </summary>
        /// <param name="addressIPv4"></param>
        /// <param name="addressIPv6"></param>
        /// <param name="port"></param>
        /// <param name="reuseAddress"></param>
        /// <param name="ipv6Mode"></param>
        /// <returns></returns>
        public bool Bind(IPAddress addressIPv4, IPAddress addressIPv6, int port, bool reuseAddress, IPv6Mode ipv6Mode)
        {
            if (IsActive())
                return false;
            bool dualMode = ipv6Mode == IPv6Mode.DualMode && IPv6Support;
            
            _udpSocketv4 = new Socket(
                dualMode ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, 
                SocketType.Dgram, 
                ProtocolType.Udp); 

            if (!BindSocket(_udpSocketv4, new IPEndPoint(dualMode ? addressIPv6 : addressIPv4, port), reuseAddress, ipv6Mode))
                return false;

            LocalPort = ((IPEndPoint) _udpSocketv4.LocalEndPoint).Port;

#if UNITY_IOS && !UNITY_EDITOR
            if (_unitySocketFix == null)
            {
                var unityFixObj = new GameObject("LiteNetLib_UnitySocketFix");
                GameObject.DontDestroyOnLoad(unityFixObj);
                _unitySocketFix = unityFixObj.AddComponent<UnitySocketFix>();
                _unitySocketFix.Socket = this;
                _unitySocketFix.BindAddrIPv4 = addressIPv4;
                _unitySocketFix.BindAddrIPv6 = addressIPv6;
                _unitySocketFix.Reuse = reuseAddress;
                _unitySocketFix.Port = LocalPort;
                _unitySocketFix.IPv6 = ipv6Mode;
            }
            else
            {
                _unitySocketFix.Paused = false;
            }
#endif
            if (dualMode)
                _udpSocketv6 = _udpSocketv4;

            IsRunning = true;
            //开启一个新的线程来接受socket消息
            _threadv4 = new Thread(ReceiveLogic)
            {
                Name = "SocketThreadv4(" + LocalPort + ")",
                IsBackground = true
            };
            _threadv4.Start(_udpSocketv4);

            //Check IPv6 support
            if (!IPv6Support || ipv6Mode != IPv6Mode.SeparateSocket)
                return true;

            //IpV6
            _udpSocketv6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
            //Use one port for two sockets
            if (BindSocket(_udpSocketv6, new IPEndPoint(addressIPv6, LocalPort), reuseAddress, ipv6Mode))
            {
                _threadv6 = new Thread(ReceiveLogic)
                {
                    Name = "SocketThreadv6(" + LocalPort + ")",
                    IsBackground = true
                };
                _threadv6.Start(_udpSocketv6);
            }

            return true;
        }

        /// <summary>
        /// 绑定指定socket
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="endPoint"></param>
        /// <param name="reuseAddress"></param>
        /// <param name="ipv6Mode"></param>
        /// <returns></returns>
        private bool BindSocket(Socket socket, IPEndPoint endPoint, bool reuseAddress, IPv6Mode ipv6Mode)
        {
            //Setup socket
            socket.ReceiveTimeout = 500;
            socket.SendTimeout = 500;
            socket.ReceiveBufferSize = NetConstants.SocketBufferSize;
            socket.SendBufferSize = NetConstants.SocketBufferSize;
#if !UNITY || UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
#if NETSTANDARD || NETCOREAPP
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
#endif
            try
            {
                socket.IOControl(SioUdpConnreset, new byte[] { 0 }, null);
            }
            catch
            {
                //ignored
            }
#endif

            try
            {
                //是否复用Address地址
                socket.ExclusiveAddressUse = !reuseAddress;
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, reuseAddress);
            }
            catch
            {
                //Unity with IL2CPP throws an exception here, it doesn't matter in most cases so just ignore it
            }
            if (socket.AddressFamily == AddressFamily.InterNetwork)
            {
                Ttl = NetConstants.SocketTTL;

#if NETSTANDARD || NETCOREAPP
                if(!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
#endif
                //禁止数据报进行分段
                try { socket.DontFragment = true; }
                catch (SocketException e)
                {
                    Logger.Error($"[B]DontFragment error: {e.SocketErrorCode}");
                }
                //开启广播
                try { socket.EnableBroadcast = true; }
                catch (SocketException e)
                {
                    Logger.Error($"[B]Broadcast error: {e.SocketErrorCode}");
                }
            }
            else //IPv6 specific
            {
                if (ipv6Mode == IPv6Mode.DualMode)
                {
                    try
                    {
                        //Disable IPv6 only mode
                        socket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                    }
                    catch(Exception e)
                    {
                        Logger.Error($"[B]Bind exception (dualmode setting) {e.ToString()}");
                    }
                }
            }

            //Bind
            try
            {
                socket.Bind(endPoint);
                Logger.Debug($"[B]Successfully bind to port: {((IPEndPoint)socket.LocalEndPoint).Port}");

                //join multicast
                if (socket.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    try
                    {
#if !UNITY
                        socket.SetSocketOption(
                            SocketOptionLevel.IPv6,
                            SocketOptionName.AddMembership,
                            new IPv6MulticastOption(MulticastAddressV6));
#endif
                    }
                    catch (Exception)
                    {
                        // Unity3d throws exception - ignored
                    }
                }
            }
            catch (SocketException bindException)
            {
                switch (bindException.SocketErrorCode)
                {
                    //IPv6 bind fix
                    case SocketError.AddressAlreadyInUse:
                        if (socket.AddressFamily == AddressFamily.InterNetworkV6 && ipv6Mode != IPv6Mode.DualMode)
                        {
                            try
                            {
                                //Set IPv6Only
                                socket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, true);
                                socket.Bind(endPoint);
                            }
#if UNITY_2018_3_OR_NEWER
                            catch (SocketException ex)
                            {

                                //because its fixed in 2018_3
                                Logger.Debug($"[B]Bind exception: {ex.ToString()}, errorCode: {ex.SocketErrorCode}");
#else
                            catch(SocketException)
                            {
#endif
                                return false;
                            }
                            return true;
                        }
                        break;
                    //hack for iOS (Unity3D)
                    case SocketError.AddressFamilyNotSupported:
                        return true;
                }
                Logger.Debug($"[B]Bind exception: {bindException.ToString()}, errorCode: {bindException.SocketErrorCode}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="port">远端端口号</param>
        /// <returns></returns>
        public bool SendBroadcast(byte[] data, int offset, int size, int port)
        {
            if (!IsActive())
                return false;
            bool broadcastSuccess = false;
            bool multicastSuccess = false;
            try
            {
                broadcastSuccess = _udpSocketv4.SendTo(
                             data,
                             offset,
                             size,
                             SocketFlags.None,
                             new IPEndPoint(IPAddress.Broadcast, port)) > 0;

                if (_udpSocketv6 != null)
                {
                    multicastSuccess = _udpSocketv6.SendTo(
                                                data,
                                                offset,
                                                size,
                                                SocketFlags.None,
                                                new IPEndPoint(MulticastAddressV6, port)) > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[S][MCAST] {ex}");
                return broadcastSuccess;
            }
            return broadcastSuccess || multicastSuccess;
        }

        /// <summary>
        /// 向指定IP地址和端口发送消息
        /// 同步发送
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public int SendTo(byte[] data, int offset, int size, IPEndPoint remoteEndPoint, ref SocketError errorCode)
        {
            if (!IsActive())
                return 0;
            try
            {
                var socket = _udpSocketv4;
                if (remoteEndPoint.AddressFamily == AddressFamily.InterNetworkV6 && IPv6Support)
                    socket = _udpSocketv6;
                int result = socket.SendTo(data, offset, size, SocketFlags.None, remoteEndPoint);
                Logger.Debug($"[S]Send packet to {remoteEndPoint}, Send bytes count: {result}");
                return result;
            }
            catch (SocketException ex)
            {
                switch (ex.SocketErrorCode)
                {
                    case SocketError.NoBufferSpaceAvailable:
                    case SocketError.Interrupted:
                        return 0;
                    case SocketError.MessageSize: //do nothing              
                        break;
                    default:
                        Logger.Error("[S]" + ex);
                        break;
                }
                errorCode = ex.SocketErrorCode;
                return -1;
            }
            catch (Exception ex)
            {
                Logger.Error("[S]" + ex);
                return -1;
            }
        }

        /// <summary>
        /// 断开socket连接
        /// </summary>
        /// <param name="suspend">是否只是挂起，如果不是，那么把IsRunning设为false</param>
        public void Close(bool suspend)
        {
            if (!suspend)
            {
                IsRunning = false;
#if UNITY_IOS && !UNITY_EDITOR
                _unitySocketFix.Socket = null;
                _unitySocketFix = null;
#endif
            }
            //cleanup dual mode
            if (_udpSocketv4 == _udpSocketv6)
                _udpSocketv6 = null;

            if (_udpSocketv4 != null)
                _udpSocketv4.Close();
            if (_udpSocketv6 != null)
                _udpSocketv6.Close();
            _udpSocketv4 = null;
            _udpSocketv6 = null;

            if (_threadv4 != null && _threadv4 != Thread.CurrentThread)
                _threadv4.Join();
            if (_threadv6 != null && _threadv6 != Thread.CurrentThread)
                _threadv6.Join();
            _threadv4 = null;
            _threadv6 = null;
        }
    }
}