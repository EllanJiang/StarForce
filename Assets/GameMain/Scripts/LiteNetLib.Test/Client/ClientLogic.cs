/*
* 文件名：ClientLogic
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 12:00:29
* 修改记录：
*/

using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib.LiteNetLib.Protos;
using LiteNetLib.Test.Shared;
using LogicShared.LiteNetLib;
using LogicShared.LiteNetLib.Utils;
using LogicShared.TrueSync.Math;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace LiteNetLib.Test.Client
{
    /// <summary>
    /// 客户端处理逻辑
    /// </summary>
    public class ClientLogic : MonoBehaviour, INetEventListener
    {
        [SerializeField] private ClientPlayerView _clientPlayerViewPrefab;
        [SerializeField] private RemotePlayerView _remotePlayerViewPrefab;
        [SerializeField] private Text _debugText;
        [SerializeField] private ShootEffect _shootEffectPrefab;

        private Action<DisconnectInfo> _onDisconnected;
        private GamePool<ShootEffect> _shootsPool;      //开火特效对象池
        
        private NetManager _netManager;
        private NetDataWriter _writer;
        private NetPacketProcessor _packetProcessor;    //网络包处理器，用来自动序列化和反序列化


        private string _userName;
        // private ServerState _cachedServerState;     //缓存服务器信息
        // private ShootPacket _cachedShootData;       //缓存开火Packet
        private ushort _lastServerTick;             //服务器最后一次下发的帧ID
        private NetPeer _server;                    //服务器peer
        private ClientPlayerManager _playerManager;
        private int _ping;                          //延迟ping值

        public static LogicTimer LogicTimer { get; private set; }

        private ShootEffect ShootEffectContructor()
        {
            var eff = Instantiate(_shootEffectPrefab);
            eff.Init(e => _shootsPool.Put(e));
            return eff;
        }
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Random r = new Random();
            // _cachedServerState = new ServerState();
            // _cachedShootData = new ShootPacket();
            _userName = Environment.MachineName + " " + r.Next(100000); //随机用户名
            LogicTimer = new LogicTimer(OnLogicUpdate);
            _writer = new NetDataWriter();
            _playerManager = new ClientPlayerManager(this);
            _shootsPool = new GamePool<ShootEffect>(ShootEffectContructor, 100);
            _packetProcessor = new NetPacketProcessor();
            //_packetProcessor.RegisterNestedType<FixVector2>((w, v) => w.Put(v), reader => reader.GetVector2());
            //_packetProcessor.RegisterNestedType<PlayerState>();
            _packetProcessor.SubscribeNetSerializable<PlayerJoinedPacket>(OnPlayerJoined);
            _packetProcessor.SubscribeNetSerializable<JoinAcceptPacket>(OnJoinAccept);
            _packetProcessor.SubscribeNetSerializable<PlayerLeavedPacket>(OnPlayerLeaved);
            _packetProcessor.SubscribeNetSerializable<ServerState>(OnServerState);
            _packetProcessor.SubscribeNetSerializable<ShootPacket>(OnShoot);
            _netManager = new NetManager(this)
            {
                AutoRecycle = true,
                IPv6Enabled = IPv6Mode.Disabled
            };
            _netManager.Start();
        }

        private void OnLogicUpdate()
        {
            _playerManager.LogicUpdate();
        }

        private void Update()
        {
            _netManager.PollEvents();
            LogicTimer.Update();
            if (_playerManager.OurPlayer != null)
                _debugText.text =
                    string.Format(
                        $"当前帧ID: {_lastServerTick}\n" + 
                        $"预测命令个数: {_playerManager.OurPlayer.StoredCommands}\n" + 
                        $"当前Ping值: {_ping}");
            else
                _debugText.text = "断开连接";
        }

        private void OnDestroy()
        {
            _netManager.Stop();
        }

        //通知有新的玩家加入房间
        private void OnPlayerJoined(PlayerJoinedPacket packet)
        {
            Debug.Log($"[C] 新玩家加入房间: {packet.UserName}");
            var remotePlayer = new RemotePlayer(_playerManager, packet.UserName, packet);
            var view = RemotePlayerView.Create(_remotePlayerViewPrefab, remotePlayer);      //创建远端玩家实体
            _playerManager.AddPlayer(remotePlayer, view);
        }

        //更新服务器信息
        private void OnServerState(ServerState serverState)
        {
            //skip duplicate or old because we received that packet unreliably
            if (NetworkGeneral.SeqDiff(serverState.Tick, _lastServerTick) <= 0)
                return;
            _lastServerTick = serverState.Tick;
            _playerManager.ApplyServerState(ref serverState);
        }

        //
        private void OnShoot(ShootPacket shootPacket)
        {
            var p = _playerManager.GetById(shootPacket.FromPlayer);    //谁开火
            if (p == null || p == _playerManager.OurPlayer)                          //如果是自己开火，那么不处理（因为本地预测过了）
                return;
            SpawnShoot(p.Position, shootPacket.Hit);
        }

        //生成开火特效
        public void SpawnShoot(FixVector2 from, FixVector2 to)
        {
            var eff = _shootsPool.Get();
            eff.Spawn(from, to);
        }

        //通知玩家离开房间
        private void OnPlayerLeaved(PlayerLeavedPacket packet)
        {
            var player = _playerManager.RemovePlayer(packet.Id);
            if(player != null)
                Debug.Log($"[C] 该玩家离开房间了: {player.Name}");
        }

        //通知同意自己加入房间
        private void OnJoinAccept(JoinAcceptPacket packet)
        {
            Debug.Log("[C] 服务器同意自己加入房间了： " + packet.Id);
            _lastServerTick = packet.ServerTick;
            var clientPlayer = new ClientPlayer(this, _playerManager, _userName, packet.Id);
            var view = ClientPlayerView.Create(_clientPlayerViewPrefab, clientPlayer);
            _playerManager.AddClientPlayer(clientPlayer, view);
        }

        //发送手动序列化的包
        public void WritePacket<T>(T packet, DeliveryMethod deliveryMethod) where T : INetSerializable
        {
            if (_server == null)
                return;
            _writer.Reset();
            _writer.Put((byte)PacketType.Serialized);
            packet.Serialize(_writer);
            _server.Send(_writer, deliveryMethod);
        }

        // //发送自动序列化的包
        // public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new()
        // {
        //     if (_server == null)
        //         return;
        //     _writer.Reset();
        //     _writer.Put((byte) PacketType.Serialized);
        //     _packetProcessor.Write(_writer, packet);
        //     _server.Send(_writer, deliveryMethod);
        // }

        //连接到服务器了，这里peer就是服务器的peer
        void INetEventListener.OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[C] 连接到服务器了，服务器IP和Port是: " + peer.EndPoint);
            _server = peer;
            
            WritePacket(new JoinPacket {UserName = _userName}, DeliveryMethod.ReliableOrdered);
            LogicTimer.Start();
        }

        void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            _playerManager.Clear();
            _server = null;
            LogicTimer.Stop();
            Debug.Log("[C] 服务器断开连接了，断开原因是: " + disconnectInfo.Reason);
            if (_onDisconnected != null)
            {
                _onDisconnected(disconnectInfo);
                _onDisconnected = null;
            }
        }

        void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.Log("[C] NetworkError: " + socketError);
        }

        //客户端收到服务器发来的消息
        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte packetType = reader.GetByte();
            if (packetType >= NetworkGeneral.PacketTypesCount)
                return;
            PacketType pt = (PacketType) packetType;
            switch (pt)
            {
                // case PacketType.Spawn:
                //     break;
                // case PacketType.ServerState:
                //     _cachedServerState.Deserialize(reader);     //收到新的服务器信息
                //     OnServerState();
                //     break;
                case PacketType.Serialized:
                    _packetProcessor.ReadAllPackets(reader);    //收到序列化消息
                    break;
                // case PacketType.Shoot:
                //     _cachedShootData.Deserialize(reader);       //收到玩家射击消息
                //     OnShoot();
                    break;
                default:
                    Debug.Log("Unhandled packet: " + pt);
                    break;
            }
        }

        void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
            UnconnectedMessageType messageType)
        {

        }

        void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            _ping = latency;
        }

        void INetEventListener.OnConnectionRequest(ConnectionRequest request)
        {
            //客户端不需要连接其他客户端，直接拒绝
            request.Reject();
        }

        //向服务器发起连接请求
        public void Connect(string ip, Action<DisconnectInfo> onDisconnected)
        {
            _onDisconnected = onDisconnected;
            _netManager.Connect(ip, 10515, "ExampleGame");
        }
    }
}