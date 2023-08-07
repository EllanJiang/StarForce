/*
  作者：LTH
  文件描述：
  文件名：OutsideClientLogic
  创建时间：2023/08/05 22:08:SS
*/

using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib.Test.Shared;
using LogicShared;
using LogicShared.LiteNetLib;
using LogicShared.LiteNetLib.Utils;
using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 局外客户端网络管理器
    /// </summary>
    public class OutsideNetManager:MonoBehaviour,INetEventListener
    {
        private static NetManager _netManager;
        private static NetDataWriter _cachedWriter;
        private static NetPacketProcessor _packetProcessor;    //网络包处理器，用来序列化和反序列化packet
        
        private static NetPeer _server;                        //局外服务器peer

        private static Action _onConnected;
        private static Action<DisconnectInfo> _onDisconnected;

        private static bool _connected;

        public static bool Connected
        {
            get { return _connected; }
        }
        
        private void Awake()
        {
            //设置协议ID获取方式
            var gameLogger = new GameLogger();
            LogicShared.Logger.logger = gameLogger;
            LogicShared.LiteNetLib.Utils.ProtoIDGetter.protoIdGetter = new Protos.ProtoID();
            
            _cachedWriter = new NetDataWriter();
            _packetProcessor = new NetPacketProcessor();
            _netManager = new NetManager(this)
            {
                AutoRecycle = true,
                IPv6Enabled = IPv6Mode.Disabled
            };
            
            _netManager.Start();
        }

        private void Start()
        {
            // todo 不应该放在这里的，需要放到框架的初始化函数
            PlayerInfoManager.Instance.Init();
            RoomManager.Instance.Init();
            BattleManager.Instance.Init();
        }

        //向服务器发起连接请求
        public static void Connect(IPEndPoint endPoint,string key,Action onConnected, Action<DisconnectInfo> onDisconnected)
        {
            var ipAddress = endPoint.Address;
            Connect(ipAddress.ToString(), endPoint.Port, key, onConnected,onDisconnected);
        }
        
        //向服务器发起连接请求
        public static void Connect(string ip,int port,string key, Action onConnected,Action<DisconnectInfo> onDisconnected)
        {
            _onConnected = onConnected;
            _onDisconnected = onDisconnected;
            _netManager.Connect(ip, port, key);
        }
        
        //注册消息回调事件
        public static void Register<T>(
            Action<T> onReceive) where T : INetSerializable, new()
        {
            _packetProcessor.SubscribeNetSerializable<T>(onReceive);
        }
        
        //注册消息回调事件
        public static void Register<T, TUserData>(Action<T, TUserData> onReceive) where T : INetSerializable, new()
        {
            _packetProcessor.SubscribeNetSerializable<T, TUserData>(onReceive);
        }

        //轮询事件
        private void Update()
        {
            _netManager.PollEvents();
        }
        
        //发送消息
        public static void SendPacket<T>(T packet) where T : INetSerializable
        {
            Debug.Log("[Client] 发送消息类型： " + typeof(T));
            if (_server == null)
            {
                return;
            }
            _cachedWriter.Reset();
            _cachedWriter.Put((byte)PacketType.Serialized);
            ulong protoId = ProtoIDGetter.TryGetId<T>();
            _cachedWriter.Put(protoId);
            packet.Serialize(_cachedWriter);
            _server.Send(_cachedWriter);
        }
        
        //成功连接到局外服务器，这里peer就是服务器的peer
        void INetEventListener.OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[C] 连接到局外服务器了，服务器IP和Port是: " + peer.EndPoint);
            _server = peer;
            if (_onConnected != null)
            {
                _onConnected();
                _onConnected = null;
            }

            _connected = true;
        }

        //局外服务器断开连接了
        void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (disconnectInfo.Reason == DisconnectReason.Timeout)
            {
                Connect(_server.EndPoint,"ExampleGame",_onConnected,_onDisconnected);
                Debug.Log("局外服务器断线重连");
                return;
            }
            _server = null;
            Debug.Log("[C] 局外服务器断开连接了，断开原因是: " + disconnectInfo.Reason);
            if (_onDisconnected != null)
            {
                _onDisconnected(disconnectInfo);
                _onDisconnected = null;
            }

            _connected = false;
        }

        //处理消息
        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte packetType = reader.GetByte();
            if (packetType >= NetworkGeneral.PacketTypesCount)
                return;
            PacketType pt = (PacketType) packetType;
            switch (pt)
            {
                case PacketType.Serialized:
                    _packetProcessor.ReadAllPackets(reader);        //收到序列化消息
                    break;
                default:
                    Debug.Log("Unhandled packet: " + pt);
                    break;
            }
        }
        
        void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.Log("[C] NetworkError: " + socketError);
        }

       
        void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
            UnconnectedMessageType messageType)
        {
            
        }

        //局外服务器网络波动了
        private float _ping;
        void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            _ping = latency;
        }

        void INetEventListener.OnConnectionRequest(ConnectionRequest request)
        {
            //客户端不需要连接其他客户端，直接拒绝
            request.Reject();
        }
        
        private void OnDestroy()
        {
            _netManager.Stop();
        }
    }
}