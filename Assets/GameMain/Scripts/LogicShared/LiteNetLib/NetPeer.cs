/*
  作者：LTH
  文件描述：
  文件名：NetPeer
  创建时间：2023/07/22 20:07:SS
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using LogicShared.LiteNetLib.Utils;

namespace LogicShared.LiteNetLib
{
    #region enums
    
    /// <summary>
    /// Peer connection state
    /// </summary>
    [Flags]
    public enum ConnectionState : byte
    {
        Outgoing          = 1 << 1,     //即将派发连接请求
        Connected         = 1 << 2,     //连接成功
        ShutdownRequested = 1 << 3,     //发送请求shutdown状态
        Disconnected      = 1 << 4,     //已断开连接
        Any = Outgoing | Connected | ShutdownRequested
    }

    /// <summary>
    /// 连接请求结果类型
    /// </summary>
    internal enum ConnectRequestResult
    {
        None,
        P2PLose,        //when peer connecting
        Reconnection,   //when peer was connected
        NewConnection   //when peer was disconnected
    }

    /// <summary>
    /// 断开连接结果类型
    /// </summary>
    internal enum DisconnectResult
    {
        None,
        Reject,         //由于对方拒绝而断开连接
        Disconnect      //从连接到断开连接
    }

    /// <summary>
    /// 关闭连接结果类型
    /// </summary>
    internal enum ShutdownResult
    {
        None,
        Success,        //shutdown成功
        WasConnected    //由ConnectionState==Connected 切换到shutdown
    }
    
    #endregion
    
    /// <summary>
    /// Network peer. Main purpose is sending messages to specific peer.
    /// </summary>
    public class NetPeer
    {
        #region 成员变量
        
        //Ping and RTT
        private int _rtt;                           //总的往返时延
        private int _avgRtt;                        //平均往返时延 _avgRtt=_rtt/_rttCount
        private int _rttCount;                      //往返时延次数
        private double _resendDelay = 27.0;         //重新发送间隔（毫秒）
        private int _pingSendTimer;                 //发送ping计时器.当_pingSendTimer >= NetManager.PingInterval时就会重新发送ping请求
        private int _rttResetTimer;                 //重置rrt计时器.当_rttResetTimer >= NetManager.PingInterval * 3时就会重置 _rtt,_avgRtt和_rttCount
        private readonly Stopwatch _pingTimer = new Stopwatch();
        private int _timeSinceLastPacket;           //从上一个packet的发送时间到现在的时间间隔
        private long _remoteDelta;                  //远端时间戳和本地时间戳的差值

        //Common            
        private readonly NetPacketPool _packetPool;
        private readonly object _shutdownLock = new object();

        internal volatile NetPeer NextPeer;         //下一个peer
        internal NetPeer PrevPeer;                  //上一个peer
        
        //连接次数
        internal byte ConnectionNum
        {
            get { return _connectNum; }
            private set
            {
                _connectNum = value;
                _mergeData.ConnectionNumber = value;
                _pingPacket.ConnectionNumber = value;
                _pongPacket.ConnectionNumber = value;
            }
        }
        
        //Channels
        private readonly Queue<NetPacket> _unreliableChannel;   //不可靠频道队列，指的是派发方法等于 DeliveryMethod.Unreliable = 4 的频道队列
        private readonly BaseChannel[] _channels;               //可靠频道数组，DeliveryMethod中除了DeliveryMethod.Unreliable之外，每个类型都对应一个频道
                                                                //总共可以有64个频道(0-63)，每个频道有4种不同的可靠传输方法：ReliableUnordered = 0,Sequenced = 1,ReliableOrdered = 2,ReliableSequenced = 3
        private BaseChannel _headChannel;                       //当前使用的频道

        //MTU
        private int _mtu;                                //最大传输单元大小（字节）
        private int _mtuIdx;                             //mtu索引（NetConstants.PossibleMtu的索引）
        private bool _finishMtu;                         //是否完成mtu
        private int _mtuCheckTimer;                      //mtu检查计时器
        private int _mtuCheckAttempts;                   //mtu检查次数
        private const int MtuCheckDelay = 1000;          //mtu检查时长
        private const int MaxMtuCheckAttempts = 4;       //最大mtu检查次数
        private readonly object _mtuMutex = new object();//mtu互斥锁
        
        //Fragment 消息片段
        private class IncomingFragments
        {
            public NetPacket[] Fragments;   //片段数据
            public int ReceivedCount;       //已收到的片段数量
            public int TotalSize;           //片段数据大小（字节数）
            public byte ChannelId;          //频道id
        }
        private int _fragmentId;                                                 //当前片段id
        private readonly Dictionary<ushort, IncomingFragments> _holdedFragments; //持有的尚未派发的片段，key是片段id
        private readonly Dictionary<ushort, ushort> _deliveredFragments;         //已派发的片段，key是片段id，value是片段数量

        //Merging
        private readonly NetPacket _mergeData;      //合并数据，每个packet都尽可能达到最大尺寸（NetConstants.MaxPacketSize）后再发送出去，这样性能会更好
        private int _mergePos;                      //合并位置
        private int _mergeCount;                    //合并数量

        //Connection
        private int _connectAttempts;               //尝试连接次数
        private int _connectTimer;                  //连接计时器
        private long _connectTime;                  //连接时间戳
        private byte _connectNum;                   //已连接次数
        private ConnectionState _connectionState;   //当前连接状态
        private NetPacket _shutdownPacket;          //shutdown的packet
        private const int ShutdownDelay = 300;      //shutdown间隔时长。收到shutdown请求后，当_shutdownTimer>ShutdownDelay时才会发送shutdown OK包
        private int _shutdownTimer;                 //shutdown计时器
        private readonly NetPacket _pingPacket;     //去包 ping
        private readonly NetPacket _pongPacket;     //回包 pong
        private readonly NetPacket _connectRequestPacket;   //连接请求包
        private readonly NetPacket _connectAcceptPacket;    //同意连接包
        
        /// <summary>
        /// Peer ip address and port
        /// </summary>
        public readonly IPEndPoint EndPoint;

        /// <summary>
        /// Peer parent NetManager
        /// </summary>
        public readonly NetManager NetManager;

        /// <summary>
        /// Current connection state
        /// </summary>
        public ConnectionState ConnectionState { get { return _connectionState; } }

        /// <summary>
        /// Connection time for internal purposes
        /// </summary>
        internal long ConnectTime { get { return _connectTime; } }

        /// <summary>
        /// Peer id can be used as key in your dictionary of peers
        /// </summary>
        public readonly int Id;

        /// <summary>
        /// Current ping in milliseconds
        /// </summary>
        public int Ping { get { return _avgRtt/2; } }

        /// <summary>
        /// Current MTU - Maximum Transfer Unit ( maximum udp packet size without fragmentation )
        /// </summary>
        public int Mtu { get { return _mtu; } }

        /// <summary>
        /// Delta with remote time in ticks (not accurate：不是很准确)
        /// positive - remote time > our time
        /// </summary>
        public long RemoteTimeDelta
        {
            get { return _remoteDelta; }
        }

        /// <summary>
        /// Remote UTC time (not accurate：不是很准确)
        /// </summary>
        public DateTime RemoteUtcTime
        {
            get { return new DateTime(DateTime.UtcNow.Ticks + _remoteDelta); }
        }

        /// <summary>
        /// Time since last packet received (including internal library packets)
        /// </summary>
        public int TimeSinceLastPacket { get { return _timeSinceLastPacket; } }

        /// <summary>
        /// packet重新发送间隔（毫秒）
        /// </summary>
        internal double ResendDelay { get { return _resendDelay; } }

        /// <summary>
        /// Application defined object containing data about the connection
        /// </summary>
        public object Tag;

        /// <summary>
        /// Statistics of peer connection
        /// </summary>
        public readonly NetStatistics Statistics;
        
        #endregion

        #region 构造函数
        
        /// <summary>
        /// incoming connection constructor
        /// </summary>
        /// <param name="netManager"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="id"></param>
        internal NetPeer(NetManager netManager, IPEndPoint remoteEndPoint, int id)
        {
            Id = id;
            Statistics = new NetStatistics();
            _packetPool = netManager.NetPacketPool;
            NetManager = netManager;
            SetMtu(0);

            EndPoint = remoteEndPoint;
            _connectionState = ConnectionState.Connected;
            _mergeData = new NetPacket(PacketProperty.Merged, NetConstants.MaxPacketSize);
            _pongPacket = new NetPacket(PacketProperty.Pong, 0);
            _pingPacket = new NetPacket(PacketProperty.Ping, 0) {Sequence = 1};
           
            _unreliableChannel = new Queue<NetPacket>(64);
            _headChannel = null;
            _holdedFragments = new Dictionary<ushort, IncomingFragments>();
            _deliveredFragments = new Dictionary<ushort, ushort>();

            _channels = new BaseChannel[netManager.ChannelsCount * 4];
        }
        
        /// <summary>
        /// "Connect to" constructor
        /// </summary>
        /// <param name="netManager"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="id"></param>
        /// <param name="connectNum"></param>
        /// <param name="connectData"></param>
        internal NetPeer(NetManager netManager, IPEndPoint remoteEndPoint, int id, byte connectNum, NetDataWriter connectData) 
            : this(netManager, remoteEndPoint, id)
        {
            _connectTime = DateTime.UtcNow.Ticks;
            _connectionState = ConnectionState.Outgoing;
            ConnectionNum = connectNum;

            //Make initial packet
            //用连接时间戳当做连接id
            _connectRequestPacket = NetConnectRequestPacket.Make(connectData, remoteEndPoint.Serialize(), _connectTime);
            _connectRequestPacket.ConnectionNumber = connectNum;

            //Send request
            NetManager.SendRaw(_connectRequestPacket, EndPoint);

            Logger.Debug($"[CC] ConnectId: {_connectTime}, ConnectNum: {connectNum}" );
        }

        /// <summary>
        /// "Accept" incoming constructor
        /// </summary>
        /// <param name="netManager"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="id"></param>
        /// <param name="connectId"></param>
        /// <param name="connectNum"></param>
        internal NetPeer(NetManager netManager, IPEndPoint remoteEndPoint, int id, long connectId, byte connectNum)
            : this(netManager, remoteEndPoint, id)
        {
            _connectTime = connectId;
            _connectionState = ConnectionState.Connected;
            ConnectionNum = connectNum;

            //Make initial packet
            _connectAcceptPacket = NetConnectAcceptPacket.Make(_connectTime, connectNum, false);
            //Send
            NetManager.SendRaw(_connectAcceptPacket, EndPoint);

            Logger.Debug($"[CC] ConnectId: {_connectTime}");
        }
        
        #endregion

        #region Send Method
        
        /// <summary>
        /// Send data to peer with delivery event called
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="channelNumber">Number of channel (from 0 to channelsCount - 1)</param>
        /// <param name="deliveryMethod">Delivery method (reliable, unreliable, etc.)</param>
        /// <param name="userData">User data that will be received in DeliveryEvent</param>
        /// <exception cref="ArgumentException">
        ///     If you trying to send unreliable packet type<para/>
        /// </exception>
        public void SendWithDeliveryEvent(byte[] data, byte channelNumber, DeliveryMethod deliveryMethod, object userData)
        {
            if (deliveryMethod != DeliveryMethod.ReliableOrdered && deliveryMethod != DeliveryMethod.ReliableUnordered)
                throw new ArgumentException("Delivery event will work only for ReliableOrdered/ReliableUnordered packets");
            SendInternal(data, 0, data.Length, channelNumber, deliveryMethod, userData);
        }

        /// <summary>
        /// Send data to peer with delivery event called
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="start">Start of data</param>
        /// <param name="length">Length of data</param>
        /// <param name="channelNumber">Number of channel (from 0 to channelsCount - 1)</param>
        /// <param name="deliveryMethod">Delivery method (reliable, unreliable, etc.)</param>
        /// <param name="userData">User data that will be received in DeliveryEvent</param>
        /// <exception cref="ArgumentException">
        ///     If you trying to send unreliable packet type<para/>
        /// </exception>
        public void SendWithDeliveryEvent(byte[] data, int start, int length, byte channelNumber, DeliveryMethod deliveryMethod, object userData)
        {
            if (deliveryMethod != DeliveryMethod.ReliableOrdered && deliveryMethod != DeliveryMethod.ReliableUnordered)
                throw new ArgumentException("Delivery event will work only for ReliableOrdered/ReliableUnordered packets");
            SendInternal(data, start, length, channelNumber, deliveryMethod, userData);
        }

        /// <summary>
        /// Send data to peer with delivery event called
        /// </summary>
        /// <param name="dataWriter">Data</param>
        /// <param name="channelNumber">Number of channel (from 0 to channelsCount - 1)</param>
        /// <param name="deliveryMethod">Delivery method (reliable, unreliable, etc.)</param>
        /// <param name="userData">User data that will be received in DeliveryEvent</param>
        /// <exception cref="ArgumentException">
        ///     If you trying to send unreliable packet type<para/>
        /// </exception>
        public void SendWithDeliveryEvent(NetDataWriter dataWriter, byte channelNumber, DeliveryMethod deliveryMethod, object userData)
        {
            if (deliveryMethod != DeliveryMethod.ReliableOrdered && deliveryMethod != DeliveryMethod.ReliableUnordered)
                throw new ArgumentException("Delivery event will work only for ReliableOrdered/ReliableUnordered packets");
            SendInternal(dataWriter.Data, 0, dataWriter.Length, channelNumber, deliveryMethod, userData);
        }
        
        /// <summary>
        /// Send data to peer (channel = 0)
        /// 发送可靠包（模拟TCP）
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="deliveryMethod">Send options (reliable, unreliable, etc.)</param>
        /// <exception cref="TooBigPacketException">
        ///     If size exceeds maximum limit:<para/>
        ///     MTU - headerSize bytes for Unreliable<para/>
        ///     Fragment count exceeded ushort.MaxValue<para/>
        /// </exception>
        public void Send(byte[] data)
        {
            SendInternal(data, 0, data.Length, 0, DeliveryMethod.ReliableOrdered, null);
        }

        /// <summary>
        /// Send data to peer (channel = 0)
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="deliveryMethod">Send options (reliable, unreliable, etc.)</param>
        /// <exception cref="TooBigPacketException">
        ///     If size exceeds maximum limit:<para/>
        ///     MTU - headerSize bytes for Unreliable<para/>
        ///     Fragment count exceeded ushort.MaxValue<para/>
        /// </exception>
        public void Send(byte[] data, DeliveryMethod deliveryMethod)
        {
            SendInternal(data, 0, data.Length, 0, deliveryMethod, null);
        }
        
        /// <summary>
        /// Send data to peer (channel = 0)
        /// 发送可靠包（模拟TCP）
        /// </summary>
        /// <param name="dataWriter">DataWriter with data</param>
        /// <param name="deliveryMethod">Send options (reliable, unreliable, etc.)</param>
        /// <exception cref="TooBigPacketException">
        ///     If size exceeds maximum limit:<para/>
        ///     MTU - headerSize bytes for Unreliable<para/>
        ///     Fragment count exceeded ushort.MaxValue<para/>
        /// </exception>
        public void Send(NetDataWriter dataWriter )
        {
            SendInternal(dataWriter.Data, 0, dataWriter.Length, 0, DeliveryMethod.ReliableOrdered, null);
        }


        /// <summary>
        /// Send data to peer (channel = 0)
        /// </summary>
        /// <param name="dataWriter">DataWriter with data</param>
        /// <param name="deliveryMethod">Send options (reliable, unreliable, etc.)</param>
        /// <exception cref="TooBigPacketException">
        ///     If size exceeds maximum limit:<para/>
        ///     MTU - headerSize bytes for Unreliable<para/>
        ///     Fragment count exceeded ushort.MaxValue<para/>
        /// </exception>
        public void Send(NetDataWriter dataWriter, DeliveryMethod deliveryMethod)
        {
            SendInternal(dataWriter.Data, 0, dataWriter.Length, 0, deliveryMethod, null);
        }

        /// <summary>
        /// Send data to peer (channel = 0)
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="start">Start of data</param>
        /// <param name="length">Length of data</param>
        /// <param name="options">Send options (reliable, unreliable, etc.)</param>
        /// <exception cref="TooBigPacketException">
        ///     If size exceeds maximum limit:<para/>
        ///     MTU - headerSize bytes for Unreliable<para/>
        ///     Fragment count exceeded ushort.MaxValue<para/>
        /// </exception>
        public void Send(byte[] data, int start, int length, DeliveryMethod options)
        {
            SendInternal(data, start, length, 0, options, null);
        }

        /// <summary>
        /// Send data to peer
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="channelNumber">Number of channel (from 0 to channelsCount - 1)</param>
        /// <param name="deliveryMethod">Send options (reliable, unreliable, etc.)</param>
        /// <exception cref="TooBigPacketException">
        ///     If size exceeds maximum limit:<para/>
        ///     MTU - headerSize bytes for Unreliable<para/>
        ///     Fragment count exceeded ushort.MaxValue<para/>
        /// </exception>
        public void Send(byte[] data, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            SendInternal(data, 0, data.Length, channelNumber, deliveryMethod, null);
        }

        /// <summary>
        /// Send data to peer
        /// </summary>
        /// <param name="dataWriter">DataWriter with data</param>
        /// <param name="channelNumber">Number of channel (from 0 to channelsCount - 1)</param>
        /// <param name="deliveryMethod">Send options (reliable, unreliable, etc.)</param>
        /// <exception cref="TooBigPacketException">
        ///     If size exceeds maximum limit:<para/>
        ///     MTU - headerSize bytes for Unreliable<para/>
        ///     Fragment count exceeded ushort.MaxValue<para/>
        /// </exception>
        public void Send(NetDataWriter dataWriter, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            SendInternal(dataWriter.Data, 0, dataWriter.Length, channelNumber, deliveryMethod, null);
        }

        /// <summary>
        /// Send data to peer
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="start">Start of data</param>
        /// <param name="length">Length of data</param>
        /// <param name="channelNumber">Number of channel (from 0 to channelsCount - 1)</param>
        /// <param name="deliveryMethod">Delivery method (reliable, unreliable, etc.)</param>
        /// <exception cref="TooBigPacketException">
        ///     If size exceeds maximum limit:<para/>
        ///     MTU - headerSize bytes for Unreliable<para/>
        ///     Fragment count exceeded ushort.MaxValue<para/>
        /// </exception>
        public void Send(byte[] data, int start, int length, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            SendInternal(data, start, length, channelNumber, deliveryMethod, null);
        }
        
        private void SendInternal(
            byte[] data, 
            int start, 
            int length, 
            byte channelNumber, 
            DeliveryMethod deliveryMethod,
            object userData)
        {
            //只有连接成功后才能使用该方法发送数据
            if (_connectionState != ConnectionState.Connected || channelNumber >= _channels.Length)
                return;

            //Select channel
            PacketProperty property;
            BaseChannel channel = null;

            if (deliveryMethod == DeliveryMethod.Unreliable)
            {
                property = PacketProperty.Unreliable;
            }
            else
            {
                property = PacketProperty.Channeled;
                channel = CreateChannel((byte)(channelNumber*4 + (byte)deliveryMethod));
            }

            //Prepare  
            Logger.Debug("[RS]Packet: " + property);

            //Check fragmentation
            int headerSize = NetPacket.GetHeaderSize(property);
            //Save mtu for multi_thread
            int mtu = _mtu;
            if (length + headerSize > mtu)
            {
                //if cannot be fragmented
                //只有可靠包才能将数据包分段
                if (deliveryMethod != DeliveryMethod.ReliableOrdered && deliveryMethod != DeliveryMethod.ReliableUnordered)
                    throw new TooBigPacketException("Unreliable packet size exceeded maximum of " + (mtu - headerSize) + " bytes");

                int packetFullSize = mtu - headerSize;
                int packetDataSize = packetFullSize - NetConstants.FragmentHeaderSize;
                //总共需要分成这么多个片段进行发送
                int totalPackets = length / packetDataSize + (length % packetDataSize == 0 ? 0 : 1);

                Logger.Debug(string.Format("FragmentSend:\n" +
                           " MTU: {0}\n" +
                           " headerSize: {1}\n" +
                           " packetFullSize: {2}\n" +
                           " packetDataSize: {3}\n" +
                           " totalPackets: {4}",
                    mtu, headerSize, packetFullSize, packetDataSize, totalPackets));

                if (totalPackets > ushort.MaxValue)
                    throw new TooBigPacketException("Data was split in " + totalPackets + " fragments, which exceeds " + ushort.MaxValue +",that is unacceptable");

                //当前片段id
                ushort currentFragmentId = (ushort)Interlocked.Increment(ref _fragmentId); //线程安全的_fragmentId++

                for(ushort partIdx = 0; partIdx < totalPackets; partIdx++)
                {
                    int sendLength = length > packetDataSize ? packetDataSize : length;

                    NetPacket p = _packetPool.GetPacket(headerSize + sendLength + NetConstants.FragmentHeaderSize);
                    p.Property = property;
                    p.UserData = userData;
                    p.FragmentId = currentFragmentId;
                    p.FragmentPart = partIdx;
                    p.FragmentsTotal = (ushort)totalPackets;
                    p.MarkFragmented();

                    Buffer.BlockCopy(data, partIdx * packetDataSize, p.RawData, NetConstants.FragmentedHeaderTotalSize, sendLength);
                    channel.AddToQueue(p);

                    length -= sendLength;
                }
                return;
            }

            //Else just send
            NetPacket packet = _packetPool.GetPacket(headerSize + length);
            packet.Property = property;
            Buffer.BlockCopy(data, start, packet.RawData, headerSize, length);
            packet.UserData = userData;

            if (channel == null) //unreliable
            {
                lock(_unreliableChannel)
                    _unreliableChannel.Enqueue(packet);
            }
            else
            {
                channel.AddToQueue(packet);
            }
        }
        
        #endregion

        #region Disconnect Method

        public void Disconnect(byte[] data)
        {
            NetManager.DisconnectPeer(this, data);
        }

        public void Disconnect(NetDataWriter writer)
        {
            NetManager.DisconnectPeer(this, writer);
        }

        public void Disconnect(byte[] data, int start, int count)
        {
            NetManager.DisconnectPeer(this, data, start, count);
        }

        public void Disconnect()
        {
            NetManager.DisconnectPeer(this);
        }

        /// <summary>
        /// 处理断开连接
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        internal DisconnectResult ProcessDisconnect(NetPacket packet)
        {
            if ((_connectionState == ConnectionState.Connected || _connectionState == ConnectionState.Outgoing) &&
                packet.Size >= 9 &&
                BitConverter.ToInt64(packet.RawData, 1) == _connectTime &&     //连接id相等
                packet.ConnectionNumber == _connectNum)                                 //连接次数相等   
            {
                return _connectionState == ConnectionState.Connected
                    ? DisconnectResult.Disconnect
                    : DisconnectResult.Reject;
            }
            return DisconnectResult.None;
        }

        /// <summary>
        /// shutdown socket
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="force">if true then force shutdown and don't send anything else send shutdown packet</param>
        /// <returns></returns>
        internal ShutdownResult Shutdown(byte[] data, int start, int length, bool force)
        {
            lock (_shutdownLock)
            {
                //trying to shutdown already disconnected
                if (_connectionState == ConnectionState.Disconnected ||
                    _connectionState == ConnectionState.ShutdownRequested)
                {
                    return ShutdownResult.None;
                }

                var result = _connectionState == ConnectionState.Connected
                    ? ShutdownResult.WasConnected
                    : ShutdownResult.Success;

                //don't send anything
                if (force)
                {
                    _connectionState = ConnectionState.Disconnected;
                    return result;
                }

                //reset time for reconnect protection
                Interlocked.Exchange(ref _timeSinceLastPacket, 0);

                //send shutdown packet
                _shutdownPacket = new NetPacket(PacketProperty.Disconnect, length) {ConnectionNumber = _connectNum};
                FastBitConverter.GetBytes(_shutdownPacket.RawData, 1, _connectTime);
                if (_shutdownPacket.Size >= _mtu)
                {
                    //Drop additional data
                    Logger.Error("[Peer] Disconnect additional data size more than MTU - 8!");
                }
                else if (data != null && length > 0)
                {
                    Buffer.BlockCopy(data, start, _shutdownPacket.RawData, 9, length);
                }
                _connectionState = ConnectionState.ShutdownRequested;
                Logger.Debug("[Peer] Send disconnect");
                NetManager.SendRaw(_shutdownPacket, EndPoint);
                return result;
            }
        }

        #endregion

        #region Set Or Get Method

        /// <summary>
        /// 设置最大传输单元
        /// </summary>
        /// <param name="mtuIdx">NetConstants.PossibleMtu的索引</param>
        private void SetMtu(int mtuIdx)
        {
            _mtu = NetConstants.PossibleMtu[mtuIdx] - NetManager.ExtraPacketSizeForLayer;
        }

         /// <summary>
        /// 添加可靠包
        /// </summary>
        /// <param name="method"></param>
        /// <param name="packet"></param>
        internal void AddReliablePacket(DeliveryMethod method, NetPacket packet)
        {
            if (packet.IsFragmented)
            {
                Logger.Debug(string.Format("Fragment. Id: {0}, Part: {1}, Total: {2}", packet.FragmentId,
                    packet.FragmentPart, packet.FragmentsTotal));
                //Get needed array from dictionary
                ushort packetFragId = packet.FragmentId;
                IncomingFragments incomingFragments;
                if (!_holdedFragments.TryGetValue(packetFragId, out incomingFragments))
                {
                    incomingFragments = new IncomingFragments
                    {
                        Fragments = new NetPacket[packet.FragmentsTotal],
                        ChannelId = packet.ChannelId
                    };
                    _holdedFragments.Add(packetFragId, incomingFragments);
                }

                //Cache
                var fragments = incomingFragments.Fragments;

                //Error check
                if (packet.FragmentPart >= fragments.Length || 
                    fragments[packet.FragmentPart] != null || 
                    packet.ChannelId != incomingFragments.ChannelId)
                {
                    _packetPool.Recycle(packet);
                    Logger.Error("Invalid fragment packet");
                    return;
                }
                //Fill array
                fragments[packet.FragmentPart] = packet;

                //Increase received fragments count
                incomingFragments.ReceivedCount++;

                //Increase total size
                incomingFragments.TotalSize += packet.Size - NetConstants.FragmentedHeaderTotalSize;

                //Check for finish
                if (incomingFragments.ReceivedCount != fragments.Length)
                    return;

                //just simple packet
                NetPacket resultingPacket = _packetPool.GetPacket(incomingFragments.TotalSize);

                int firstFragmentSize = fragments[0].Size - NetConstants.FragmentedHeaderTotalSize;
                for (int i = 0; i < incomingFragments.ReceivedCount; i++)
                {
                    var fragment = fragments[i];
                    //Create resulting big packet 
                    //把一个个片段合成一个大的packet
                    Buffer.BlockCopy(
                        fragment.RawData,
                        NetConstants.FragmentedHeaderTotalSize,
                        resultingPacket.RawData,
                        firstFragmentSize * i,      //只有最后一个片段的大小不是firstFragmentSize
                        fragment.Size - NetConstants.FragmentedHeaderTotalSize);

                    //Free memory
                    _packetPool.Recycle(fragment);
                }
                Array.Clear(fragments, 0, incomingFragments.ReceivedCount);

                //Send to process
                NetManager.CreateReceiveEvent(resultingPacket, method, 0, this);

                //Clear memory
                _holdedFragments.Remove(packetFragId);
            }
            else //Just simple packet
            {
                NetManager.CreateReceiveEvent(packet, method, NetConstants.ChanneledHeaderSize, this);
            }
        }
        
        /// <summary>
        /// Gets maximum size of packet that will be not fragmented.
        /// </summary>
        /// <param name="deliveryMethod">Type of packet that you want send</param>
        /// <returns>size in bytes</returns>
        public int GetMaxSinglePacketSize(DeliveryMethod deliveryMethod)
        {
            return _mtu - NetPacket.GetHeaderSize(deliveryMethod == DeliveryMethod.Unreliable ? PacketProperty.Unreliable : PacketProperty.Channeled);
        }
        
        /// <summary>
        /// Returns packets count in queue for reliable channel
        /// </summary>
        /// <param name="channelNumber">number of channel [0-63]</param>
        /// <param name="ordered">type of channel ReliableOrdered or ReliableUnordered</param>
        /// <returns>packets count in channel queue</returns>
        public int GetPacketsCountInReliableQueue(byte channelNumber, bool ordered)
        {
            int idx = channelNumber * 4 +
                      (byte) (ordered ? DeliveryMethod.ReliableOrdered : DeliveryMethod.ReliableUnordered);
            var channel = _channels[idx];
            return channel != null ? ((ReliableChannel)channel).PacketsInQueue : 0;
        }
        
        /// <summary>
        /// 拒绝连接
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="connectionNumber"></param>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        internal void Reject(long connectionId, byte connectionNumber, byte[] data, int start, int length)
        {
            _connectTime = connectionId;
            _connectNum = connectionNumber;
            Shutdown(data, start, length, false);
        }

        
        /// <summary>
        /// 创建传输频道
        /// </summary>
        /// <param name="idx">index of _channels array which range of 4*[0-63] + (int)DeliveryMethod</param>
        /// <returns></returns>
        private BaseChannel CreateChannel(byte idx)
        {
            BaseChannel newChannel = _channels[idx];
            if (newChannel != null)
                return newChannel;
            switch ((DeliveryMethod)(idx % 4))
            {
                case DeliveryMethod.ReliableUnordered:
                    newChannel = new ReliableChannel(this, false, idx);
                    break;
                case DeliveryMethod.Sequenced:
                    newChannel = new SequencedChannel(this, false, idx);
                    break;
                case DeliveryMethod.ReliableOrdered:
                    newChannel = new ReliableChannel(this, true, idx);
                    break;
                case DeliveryMethod.ReliableSequenced:
                    newChannel = new SequencedChannel(this, true, idx);
                    break;
            }
            BaseChannel prevChannel = Interlocked.CompareExchange(ref _channels[idx], newChannel, null);
            if (prevChannel != null)
                return prevChannel;

            BaseChannel headChannel;
            do
            {
                headChannel = _headChannel;
                newChannel.Next = headChannel;
            }
            while (Interlocked.CompareExchange(ref _headChannel, newChannel, headChannel) != headChannel);

            return newChannel;
        }

        #endregion
        
        #region Process Method
        
        /// <summary>
        /// 处理mtu包
        /// </summary>
        /// <param name="packet"></param>
        private void ProcessMtuPacket(NetPacket packet)
        {
            //header + int
            if (packet.Size < NetConstants.PossibleMtu[0])
                return;

            //first stage check (mtu check and mtu ok)
            int receivedMtu = BitConverter.ToInt32(packet.RawData, 1);
            int endMtuCheck = BitConverter.ToInt32(packet.RawData, packet.Size - 4);
            if (receivedMtu != packet.Size || receivedMtu != endMtuCheck || receivedMtu > NetConstants.MaxPacketSize)
            {
                Logger.Error(string.Format("[MTU] Broken packet. R_MTU {0}, E_MTU {1}, P_SIZE {2}", receivedMtu,
                    endMtuCheck, packet.Size));
                return;
            }

            if (packet.Property == PacketProperty.MtuCheck)
            {
                _mtuCheckAttempts = 0;
                Logger.Error("[MTU] check. send back: " + receivedMtu);
                packet.Property = PacketProperty.MtuOk;
                NetManager.SendRawAndRecycle(packet, EndPoint);
            }
            else if(receivedMtu > _mtu && !_finishMtu) //MtuOk
            {
                //invalid packet
                if (receivedMtu != NetConstants.PossibleMtu[_mtuIdx + 1])
                    return;

                lock (_mtuMutex)
                {
                    _mtuIdx++;
                    SetMtu(_mtuIdx);
                }
                //if maxed -> finish.
                if (_mtuIdx == NetConstants.PossibleMtu.Length - 1)
                    _finishMtu = true;

                Logger.Debug("[MTU] ok. Increase to: " + _mtu);
            }
        }
        
      
        /// <summary>
        /// 处理连接请求
        /// </summary>
        /// <param name="connRequest">连接请求包</param>
        /// <returns>返回新的连接状态</returns>
        internal ConnectRequestResult ProcessConnectRequest(NetConnectRequestPacket connRequest)
        {
            //current or new request
            switch (_connectionState)
            {
                //P2P case
                case ConnectionState.Outgoing:
                    //fast check 请求包的连接时间戳小于当前连接时间戳，说明这个请求包已经过期了
                    if (connRequest.ConnectionTime < _connectTime)
                    {
                        return ConnectRequestResult.P2PLose;
                    }
                    //slow rare case check
                    else if (connRequest.ConnectionTime == _connectTime)
                    {
                        var remoteBytes = EndPoint.Serialize();
                        var localBytes = connRequest.TargetAddress;
                        for (int i = remoteBytes.Size-1; i >= 0; i--)  //判断连接的地址是否一致，不一致的话就直接认为是连接地址丢失了
                        {
                            byte rb = remoteBytes[i];
                            if (rb == localBytes[i])
                                continue;
                            if (rb < localBytes[i])
                                return ConnectRequestResult.P2PLose;
                        }
                    }
                    break;

                case ConnectionState.Connected:
                    //Old connect request
                    if (connRequest.ConnectionTime == _connectTime)
                    {
                        //just reply accept
                        NetManager.SendRaw(_connectAcceptPacket, EndPoint);
                    }
                    //New connect request
                    else if (connRequest.ConnectionTime > _connectTime)
                    {
                        return ConnectRequestResult.Reconnection;
                    }
                    break;

                case ConnectionState.Disconnected:
                case ConnectionState.ShutdownRequested:
                    if (connRequest.ConnectionTime >= _connectTime)
                        return ConnectRequestResult.NewConnection;
                    break;
            }
            return ConnectRequestResult.None;
        }
        
         /// <summary>
         /// Process incoming packet
         /// </summary>
         /// <param name="packet"></param>
        internal void ProcessPacket(NetPacket packet)
        {
            //not initialized
            if (_connectionState == ConnectionState.Outgoing || _connectionState == ConnectionState.Disconnected)
            {
                _packetPool.Recycle(packet);
                return;
            }
            if (packet.Property == PacketProperty.ShutdownOk)       
            {
                if (_connectionState == ConnectionState.ShutdownRequested)
                    _connectionState = ConnectionState.Disconnected;
                _packetPool.Recycle(packet);
                return;
            }
            if (packet.ConnectionNumber != _connectNum)
            {
                Logger.Error("[RR]Old packet");
                _packetPool.Recycle(packet);
                return;
            }
            Interlocked.Exchange(ref _timeSinceLastPacket, 0);

            //Logger.Error($"[RR]PacketProperty: {packet.Property}");
            switch (packet.Property)
            {
                case PacketProperty.Merged:
                    int pos = NetConstants.HeaderSize;
                    while (pos < packet.Size)
                    {
                        ushort size = BitConverter.ToUInt16(packet.RawData, pos);   //包体长度
                        pos += 2;
                        NetPacket mergedPacket = _packetPool.GetPacket(size);       //创建新的分段包
                        if (!mergedPacket.FromBytes(packet.RawData, pos, size))
                        {
                            _packetPool.Recycle(packet);
                            break;
                        }
                        pos += size;
                        ProcessPacket(mergedPacket);
                    }
                    break;
                //If we get ping, send pong
                case PacketProperty.Ping:
                    if (NetUtils.RelativeSequenceNumber(packet.Sequence, _pongPacket.Sequence) > 0)
                    {
                        Logger.Debug("[PP]Ping receive, send pong");
                        FastBitConverter.GetBytes(_pongPacket.RawData, 3, DateTime.UtcNow.Ticks);
                        _pongPacket.Sequence = packet.Sequence;
                        NetManager.SendRaw(_pongPacket, EndPoint);
                    }
                    _packetPool.Recycle(packet);
                    break;

                //If we get pong, calculate ping time and rtt
                case PacketProperty.Pong:
                    if (packet.Sequence == _pingPacket.Sequence)
                    {
                        _pingTimer.Stop();
                        int elapsedMs = (int)_pingTimer.ElapsedMilliseconds; //ms
                        _remoteDelta = BitConverter.ToInt64(packet.RawData, 3) + (elapsedMs * TimeSpan.TicksPerMillisecond ) / 2 - DateTime.UtcNow.Ticks;
                        UpdateRoundTripTime(elapsedMs);
                        NetManager.ConnectionLatencyUpdated(this, elapsedMs / 2);
                        Logger.Debug(string.Format("[PP]Ping，packet.Sequence： {0} - elapsedMs：{1} - remoteDelta：{2}", packet.Sequence, elapsedMs,
                            _remoteDelta));
                    }
                    _packetPool.Recycle(packet);
                    break;

                case PacketProperty.Ack:
                case PacketProperty.Channeled:
                    if (packet.ChannelId > _channels.Length)
                    {
                        _packetPool.Recycle(packet);
                        break;
                    }
                    var channel = _channels[packet.ChannelId] ?? (packet.Property == PacketProperty.Ack ? null : CreateChannel(packet.ChannelId));
                    if (channel != null)
                    {
                        if (!channel.ProcessPacket(packet))
                            _packetPool.Recycle(packet);
                    }
                    break;

                //Simple packet without acks
                case PacketProperty.Unreliable:
                    NetManager.CreateReceiveEvent(packet, DeliveryMethod.Unreliable, NetConstants.HeaderSize, this);
                    return;

                case PacketProperty.MtuCheck:
                case PacketProperty.MtuOk:
                    ProcessMtuPacket(packet);
                    break;

                default:
                    Logger.Error("Error! Unexpected packet type: " + packet.Property);
                    break;
            }
        }
        
         /// <summary>
        /// 同意连接
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        internal bool ProcessConnectAccept(NetConnectAcceptPacket packet)
        {
            if (_connectionState != ConnectionState.Outgoing)
                return false;

            //check connection id
            if (packet.ConnectionId != _connectTime)
            {
                Logger.Error($"[NC] Invalid connectId: {_connectTime}");
                return false;
            }
            //check connect num
            ConnectionNum = packet.ConnectionNumber;

            Logger.Debug($"[NC] Received connection accept");
            Interlocked.Exchange(ref _timeSinceLastPacket, 0);
            _connectionState = ConnectionState.Connected;
            return true;
        }
         
        #endregion

        #region Send Method
        
        /// <summary>
        /// 发送合并后的包
        /// </summary>
        private void SendMerged()
        {
            if (_mergeCount == 0)
                return;
            int bytesSent;
            if (_mergeCount > 1)
            {
                Logger.Debug("[P]Send merged: " + _mergePos + ", count: " + _mergeCount);
                bytesSent = NetManager.SendRaw(_mergeData.RawData, 0, NetConstants.HeaderSize + _mergePos, EndPoint);
            }
            else
            {
                //Send without length information and merging
                bytesSent = NetManager.SendRaw(_mergeData.RawData, NetConstants.HeaderSize + 2, _mergePos - 2, EndPoint);
            }

            if (NetManager.EnableStatistics)
            {
                Statistics.IncrementPacketsSent();
                Statistics.AddBytesSent(bytesSent);
            }

            _mergePos = 0;
            _mergeCount = 0;
        }

        /// <summary>
        /// 发送用户自定义的包
        /// </summary>
        /// <param name="packet"></param>
        internal void SendUserData(NetPacket packet)
        {
            packet.ConnectionNumber = _connectNum;
            int mergedPacketSize = NetConstants.HeaderSize + packet.Size + 2;
            const int sizeThreshold = 20;
            if (mergedPacketSize + sizeThreshold >= _mtu)
            {
                Logger.Debug("[P]SendingPacket: " + packet.Property);;
                int bytesSent = NetManager.SendRaw(packet, EndPoint);

                if (NetManager.EnableStatistics)
                {
                    Statistics.IncrementPacketsSent();
                    Statistics.AddBytesSent(bytesSent);
                }

                return;
            }
            if (_mergePos + mergedPacketSize > _mtu)
                SendMerged();

            FastBitConverter.GetBytes(_mergeData.RawData, _mergePos + NetConstants.HeaderSize, (ushort)packet.Size);
            Buffer.BlockCopy(packet.RawData, 0, _mergeData.RawData, _mergePos + NetConstants.HeaderSize + 2, packet.Size);
            _mergePos += packet.Size + 2;
            _mergeCount++;
        }
        
        /// <summary>
        /// For reliable channel
        /// 派发packet并回收
        /// </summary>
        /// <param name="packet"></param>
        internal void RecycleAndDeliver(NetPacket packet)
        {
            if (packet.UserData != null)
            {
                if (packet.IsFragmented)
                {
                    ushort fragCount;
                    _deliveredFragments.TryGetValue(packet.FragmentId, out fragCount);
                    fragCount++;
                    if (fragCount == packet.FragmentsTotal)
                    {
                        NetManager.MessageDelivered(this, packet.UserData);
                        _deliveredFragments.Remove(packet.FragmentId);
                    }
                    else
                    {
                        _deliveredFragments[packet.FragmentId] = fragCount;
                    }
                }
                else
                {
                    NetManager.MessageDelivered(this, packet.UserData);
                }
                packet.UserData = null;
            }
            _packetPool.Recycle(packet);
        }
        
        #endregion

        #region Update Method
        
        /// <summary>
        /// 轮询
        /// </summary>
        /// <param name="deltaTime">ms</param>
        internal void Update(int deltaTime)
        {
            Interlocked.Add(ref _timeSinceLastPacket, deltaTime);
            switch (_connectionState)
            {
                case ConnectionState.Connected:
                    if (_timeSinceLastPacket > NetManager.DisconnectTimeout)        //长时间没有收到消息，主动断开连接
                    {
                        Logger.Error($"[UPDATE] Disconnect by timeout: {_timeSinceLastPacket} > {NetManager.DisconnectTimeout}");
                        
                        NetManager.DisconnectPeerForce(this, DisconnectReason.Timeout, 0, null);
                        return;
                    }
                    break;

                case ConnectionState.ShutdownRequested:
                    if (_timeSinceLastPacket > NetManager.DisconnectTimeout)
                    {
                        _connectionState = ConnectionState.Disconnected;
                    }
                    else
                    {
                        _shutdownTimer += deltaTime;
                        if (_shutdownTimer >= ShutdownDelay)
                        {
                            _shutdownTimer = 0;
                            NetManager.SendRaw(_shutdownPacket, EndPoint);
                        }
                    }
                    return;

                case ConnectionState.Outgoing:
                    _connectTimer += deltaTime;
                    if (_connectTimer > NetManager.ReconnectDelay)
                    {
                        _connectTimer = 0;
                        _connectAttempts++;
                        if (_connectAttempts > NetManager.MaxConnectAttempts)
                        {
                            NetManager.DisconnectPeerForce(this, DisconnectReason.ConnectionFailed, 0, null);
                            return;
                        }

                        //else send connect again
                        NetManager.SendRaw(_connectRequestPacket, EndPoint);
                    }
                    return;

                case ConnectionState.Disconnected:
                    return;
            }

            //Send ping
            _pingSendTimer += deltaTime;
            if (_pingSendTimer >= NetManager.PingInterval)
            {
                Logger.Debug("[PP] Send ping...");
                //reset timer
                _pingSendTimer = 0;
                //send ping
                _pingPacket.Sequence++;
                //ping timeout
                if (_pingTimer.IsRunning)
                    UpdateRoundTripTime((int)_pingTimer.ElapsedMilliseconds);
                _pingTimer.Reset();
                _pingTimer.Start();
                NetManager.SendRaw(_pingPacket, EndPoint);
            }

            //RTT - round trip time
            _rttResetTimer += deltaTime;
            if (_rttResetTimer >= NetManager.PingInterval * 3)
            {
                _rttResetTimer = 0;
                _rtt = _avgRtt;
                _rttCount = 1;
            }

            UpdateMtuLogic(deltaTime);

            //Pending send
            BaseChannel currentChannel = _headChannel;
            while (currentChannel != null)
            {
                currentChannel.SendNextPackets();
                currentChannel = currentChannel.Next;
            }

            lock (_unreliableChannel)
            {
                while (_unreliableChannel.Count > 0)
                {
                    NetPacket packet = _unreliableChannel.Dequeue();
                    SendUserData(packet);
                    NetManager.NetPacketPool.Recycle(packet);
                }
            }

            SendMerged();
        }

        /// <summary>
        /// 更新mtu的逻辑（发送mtu检查包）
        /// </summary>
        /// <param name="deltaTime"></param>
        private void UpdateMtuLogic(int deltaTime)
        {
            if (_finishMtu)
                return;

            _mtuCheckTimer += deltaTime;
            if (_mtuCheckTimer < MtuCheckDelay)
                return;

            _mtuCheckTimer = 0;
            _mtuCheckAttempts++;
            if (_mtuCheckAttempts >= MaxMtuCheckAttempts)
            {
                _finishMtu = true;
                return;
            }

            lock (_mtuMutex)
            {
                if (_mtuIdx >= NetConstants.PossibleMtu.Length - 1)
                    return;

                //Send increased packet
                int newMtu = NetConstants.PossibleMtu[_mtuIdx + 1];
                var newPacket = _packetPool.GetPacket(newMtu);
                newPacket.Property = PacketProperty.MtuCheck;
                //包的开头和结尾都保存mtu，用来进行校验
                FastBitConverter.GetBytes(newPacket.RawData, 1, newMtu);                    //place into start
                FastBitConverter.GetBytes(newPacket.RawData, newPacket.Size - 4, newMtu);   //and end of packet

                //Must check result for MTU fix
                if (NetManager.SendRawAndRecycle(newPacket, EndPoint) <= 0)
                    _finishMtu = true;
            }
        }
        
        /// <summary>
        /// 更新往返时延
        /// </summary>
        /// <param name="roundTripTime"></param>
        private void UpdateRoundTripTime(int roundTripTime)
        {
            _rtt += roundTripTime;
            _rttCount++;
            _avgRtt = _rtt/_rttCount;
            _resendDelay = 25.0f + _avgRtt * 2.1; // 25 ms + double rtt
        }

        #endregion
    }
}