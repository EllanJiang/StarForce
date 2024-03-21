/*
  作者：LTH
  文件描述：
  文件名：NatPunchModule
  创建时间：2023/07/22 11:07:SS
*/

using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LogicShared.LiteNetLib.Utils;

namespace LogicShared.LiteNetLib
{
    /// <summary>
    /// NAT地址类型
    /// </summary>
    public enum NatAddressType
    {
        Internal,   //内部地址或私有地址
        External    //外部地址或合法地址
    }
    
    /// <summary>
    /// NAT打洞接口
    /// </summary>
    public interface INatPunchListener
    {
        /// <summary>
        /// NAT穿透请求
        /// </summary>
        /// <param name="localEndPoint"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="token"></param>
        void OnNatIntroductionRequest(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint, string token);
        /// <summary>
        /// NAT穿透请求成功
        /// </summary>
        /// <param name="targetEndPoint"></param>
        /// <param name="type"></param>
        /// <param name="token"></param>
        void OnNatIntroductionSuccess(IPEndPoint targetEndPoint, NatAddressType type, string token);
    }
    
    public class EventBasedNatPunchListener : INatPunchListener
    {
        public delegate void OnNatIntroductionRequest(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint, string token);
        public delegate void OnNatIntroductionSuccess(IPEndPoint targetEndPoint, NatAddressType type, string token);

        public event OnNatIntroductionRequest NatIntroductionRequest;
        public event OnNatIntroductionSuccess NatIntroductionSuccess;

        void INatPunchListener.OnNatIntroductionRequest(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint, string token)
        {
            if(NatIntroductionRequest != null)
                NatIntroductionRequest(localEndPoint, remoteEndPoint, token);
        }

        void INatPunchListener.OnNatIntroductionSuccess(IPEndPoint targetEndPoint, NatAddressType type, string token)
        {
            if (NatIntroductionSuccess != null)
                NatIntroductionSuccess(targetEndPoint, type, token);
        }
    }
    
    /// <summary>
    /// Module for UDP NAT Hole punching operations. Can be accessed from NetManager
    /// 什么是NAT：Network Address/Port Translation    网络地址转换
    /// NAT是将私有ip地址（一般是局域网内的ip地址）转换为合法ip地址的技术，转换后的合法ip地址可以在网络中传播。
    /// 什么是NAT hole punching？NAT打洞就是使得设备间绕过NAT进行通讯的一种技术，这样就可以在不同NAT下的设备之间进行通讯。
    /// 参考：https://zhuanlan.zhihu.com/p/40816201
    /// </summary>
    public sealed class NatPunchModule
    {
        /// <summary>
        /// 请求NAT打洞
        /// </summary>
        struct RequestEventData
        {
            public IPEndPoint LocalEndPoint;
            public IPEndPoint RemoteEndPoint;
            public string Token;
        }

        /// <summary>
        /// NAT打洞成功
        /// </summary>
        struct SuccessEventData
        {
            public IPEndPoint TargetEndPoint;
            public NatAddressType Type;
            public string Token;
        }

        /// <summary>
        /// Nat穿透 请求包
        /// </summary>
        class NatIntroduceRequestPacket
        {
            public IPEndPoint Internal { get; set; }
            public string Token { get; set; }
        }

        /// <summary>
        ///  Nat穿透 回包
        /// </summary>
        class NatIntroduceResponsePacket
        {
            public IPEndPoint Internal { get; set; }
            public IPEndPoint External { get; set; }
            public string Token { get; set; }
        }

        /// <summary>
        /// NAT打洞包
        /// </summary>
        class NatPunchPacket
        {
            public string Token { get; set; }
            public bool IsExternal { get; set; }
        }
        
        private readonly NetSocket _socket;
        private readonly Queue<RequestEventData> _requestEvents = new Queue<RequestEventData>();
        private readonly Queue<SuccessEventData> _successEvents = new Queue<SuccessEventData>();
        private readonly NetDataReader _cacheReader = new NetDataReader();
        private readonly NetDataWriter _cacheWriter = new NetDataWriter();
        private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor(MaxTokenLength);
        private INatPunchListener _natPunchListener;
        public const int MaxTokenLength = 256;
        
        internal NatPunchModule(NetSocket socket)
        {
            _socket = socket;
            _netPacketProcessor.SubscribeReusable<NatIntroduceResponsePacket>(OnNatIntroductionResponse);
            _netPacketProcessor.SubscribeReusable<NatIntroduceRequestPacket, IPEndPoint>(OnNatIntroductionRequest);
            _netPacketProcessor.SubscribeReusable<NatPunchPacket, IPEndPoint>(OnNatPunch);
        }
        
        /// <summary>
        /// 收到网络包消息后派发出去
        /// </summary>
        /// <param name="senderEndPoint"></param>
        /// <param name="packet"></param>
        internal void ProcessMessage(IPEndPoint senderEndPoint, NetPacket packet)
        {
            lock (_cacheReader)
            {
                _cacheReader.SetSource(packet.RawData, NetConstants.HeaderSize, packet.Size);
                _netPacketProcessor.ReadAllPackets(_cacheReader, senderEndPoint);
            }
        }

        /// <summary>
        /// 初始化NAT打洞接口
        /// </summary>
        /// <param name="listener"></param>
        public void Init(INatPunchListener listener)
        {
            _natPunchListener = listener;
        }
        
        /// <summary>
        /// 向target地址发送网络包
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="target"></param>
        /// <typeparam name="T"></typeparam>
        private void Send<T>(T packet, IPEndPoint target) where T : class, new()
        {
            SocketError errorCode = 0;
            _cacheWriter.Reset();
            _cacheWriter.Put((byte)PacketProperty.NatMessage);
            _netPacketProcessor.Write(_cacheWriter, packet);
            _socket.SendTo(_cacheWriter.Data, 0, _cacheWriter.Length, target, ref errorCode);
        }

        /// <summary>
        /// NAT穿透
        /// </summary>
        /// <param name="hostInternal"></param>
        /// <param name="hostExternal"></param>
        /// <param name="clientInternal"></param>
        /// <param name="clientExternal"></param>
        /// <param name="additionalInfo"></param>
        public void NatIntroduce(
            IPEndPoint hostInternal,
            IPEndPoint hostExternal,
            IPEndPoint clientInternal,
            IPEndPoint clientExternal,
            string additionalInfo)
        {
            var req = new NatIntroduceResponsePacket
            {
                Token = additionalInfo
            };

            //First packet (server) send to client
            req.Internal = hostInternal;
            req.External = hostExternal;
            Send(req, clientExternal);

            //Second packet (client) send to server
            req.Internal = clientInternal;
            req.External = clientExternal;
            Send(req, hostExternal);
        }

        /// <summary>
        /// 轮训NAT穿透事件
        /// </summary>
        public void PollEvents()
        {
            if (_natPunchListener == null || (_successEvents.Count == 0 && _requestEvents.Count == 0))
                return;
            lock (_successEvents)
            {
                while (_successEvents.Count > 0)
                {
                    var evt = _successEvents.Dequeue();
                    //触发NAT穿透成功事件
                    _natPunchListener.OnNatIntroductionSuccess(
                        evt.TargetEndPoint, 
                        evt.Type,
                        evt.Token);
                }
            }
            lock (_requestEvents)
            {
                while (_requestEvents.Count > 0)
                {
                    var evt = _requestEvents.Dequeue();
                    _natPunchListener.OnNatIntroductionRequest(evt.LocalEndPoint, evt.RemoteEndPoint, evt.Token);
                }
            }
        }

        /// <summary>
        /// 发送NAT穿透请求
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="additionalInfo"></param>
        public void SendNatIntroduceRequest(string host, int port, string additionalInfo)
        {
            SendNatIntroduceRequest(NetUtils.MakeEndPoint(host, port), additionalInfo);
        }

        /// <summary>
        /// 向masterServerEndPoint发送NAT穿透请求
        /// </summary>
        /// <param name="masterServerEndPoint"></param>
        /// <param name="additionalInfo"></param>
        public void SendNatIntroduceRequest(IPEndPoint masterServerEndPoint, string additionalInfo)
        {
            //prepare outgoing data
            string networkIp = NetUtils.GetLocalIp(LocalAddrType.IPv4);
            if (string.IsNullOrEmpty(networkIp))
            {
                networkIp = NetUtils.GetLocalIp(LocalAddrType.IPv6);
            }

            Send(
                new NatIntroduceRequestPacket
                {
                    Internal = NetUtils.MakeEndPoint(networkIp, _socket.LocalPort),
                    Token = additionalInfo
                }, 
                masterServerEndPoint);
        }

        /// <summary>
        /// 收到NAT穿透请求包，然后把请求包放入_requestEvents队列中，等待轮训处理
        /// </summary>
        /// <param name="req">穿透请求包内容</param>
        /// <param name="senderEndPoint">这个穿透请求包是谁（ip和port）发过来</param>
        //We got request and must introduce
        private void OnNatIntroductionRequest(NatIntroduceRequestPacket req, IPEndPoint senderEndPoint)
        {
            lock (_requestEvents)
            {
                _requestEvents.Enqueue(new RequestEventData
                {
                    LocalEndPoint = req.Internal,
                    RemoteEndPoint = senderEndPoint,
                    Token = req.Token
                });
            }
        }

        /// <summary>
        /// 收到NAT穿透请求回包，进行NAT打洞
        /// </summary>
        /// <param name="req"></param>
        //We got introduce and must punch
        private void OnNatIntroductionResponse(NatIntroduceResponsePacket req)
        {
            Logger.Debug("[NAT] introduction received");

            // send internal punch 内部NAT打洞
            var punchPacket = new NatPunchPacket {Token = req.Token};
            Send(punchPacket, req.Internal);
            Logger.Debug("[NAT] internal punch sent to " + req.Internal);

            // hack for some routers 某些路由器需要进行hack（黑掉？）
            SocketError errorCode = 0;
            _socket.Ttl = 2;
            _socket.SendTo(new[] { (byte)PacketProperty.Empty }, 0, 1, req.External, ref errorCode);

            // send external punch 外部NAT打洞
            _socket.Ttl = NetConstants.SocketTTL;
            punchPacket.IsExternal = true;
            Send(punchPacket, req.External);
            Logger.Debug("[NAT] external punch sent to " + req.External);
        }

        /// <summary>
        /// NAT打洞成功，可以进行发送连接请求了
        /// </summary>
        /// <param name="req"></param>
        /// <param name="senderEndPoint">从该地址发送NAT打洞请求</param>
        //We got punch and can connect
        private void OnNatPunch(NatPunchPacket req, IPEndPoint senderEndPoint)
        {
            //Read info
            Logger.Debug($"[NAT] punch received from {senderEndPoint} - additional info: {req.Token}");

            //Release punch success to client; enabling him to Connect() to Sender if token is ok
            lock (_successEvents)
            {
                _successEvents.Enqueue(new SuccessEventData
                {
                    TargetEndPoint = senderEndPoint,
                    Type = req.IsExternal ? NatAddressType.External : NatAddressType.Internal, 
                    Token = req.Token
                });
            }
        }
    }
}