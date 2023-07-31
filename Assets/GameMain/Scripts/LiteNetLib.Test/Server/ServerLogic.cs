/*
* 文件名：ServerLogic
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:09:41
* 修改记录：
*/

using System.Net;
using System.Net.Sockets;
using LiteNetLib.LiteNetLib.Protos;
using LiteNetLib.Test.Shared;
using LogicShared.LiteNetLib;
using LogicShared.LiteNetLib.Utils;
using LogicShared.TrueSync.Math;
using UnityEngine;

namespace LiteNetLib.Test.Server
{
    /// <summary>
    /// 服务器主要实现逻辑
    /// </summary>
    public class ServerLogic : MonoBehaviour, INetEventListener
    {
        private NetManager _netManager;     //网络管理器
        private NetPacketProcessor _packetProcessor; //网络数据包序列化和反序列化处理器

        public const int MaxPlayers = 64;   //最大玩家数量
        private LogicTimer _logicTimer;     //当前逻辑时间
        private readonly NetDataWriter _cachedWriter = new NetDataWriter();
        private ushort _serverTick;         //服务器运行了多少帧（逻辑帧）
        private ServerPlayerManager _playerManager; //服务器玩家管理器

        private PlayerInputPacket _cachedCommand = new PlayerInputPacket();
        private ServerState _serverState;   //当前服务器信息，主要保存当前连入的玩家数量和玩家信息
        public ushort Tick => _serverTick;  //服务器帧id

       
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _logicTimer = new LogicTimer(OnLogicUpdate);
            _packetProcessor = new NetPacketProcessor();
            _playerManager = new ServerPlayerManager(this);
            
            //register auto serializable vector2
            //_packetProcessor.RegisterNestedType<FixVector2>((w, v) => w.Put(v), r => r.GetVector2());
           
            //register auto serializable PlayerState
            //_packetProcessor.RegisterNestedType<PlayerState>();
            
            //注册并监听收到JoinPacket消息
            _packetProcessor.SubscribeNetSerializable<JoinPacket, NetPeer>(OnJoinReceived);
            _packetProcessor.SubscribeNetSerializable<PlayerInputPacket, NetPeer>(OnInputReceived);
            _netManager = new NetManager(this)
            {
                AutoRecycle = true
            };
        }

        /// <summary>
        /// 启动服务器
        /// </summary>
        public void StartServer()
        {
            if (_netManager.IsRunning)
                return;
            _netManager.Start(10515);
            _logicTimer.Start();
        }

    
        //每帧更新一次逻辑s
        private void OnLogicUpdate()
        {
            _serverTick = (ushort)((_serverTick + 1) % NetworkGeneral.MaxGameSequence);
            _playerManager.LogicUpdate();
            if (_serverTick % 2 == 0)
            {
                //每两帧通知客户端 当前房间内所有玩家的信息
                _serverState.Tick = _serverTick;    
                _serverState.PlayerStates = _playerManager.PlayerStates;
                int pCount = _playerManager.Count;  
                
                //遍历所有连入的客户端
                foreach(ServerPlayer p in _playerManager)
                { 
                    //单个网络包能装入的最大字节数
                    int statesMax = p.AssociatedPeer.GetMaxSinglePacketSize(DeliveryMethod.Unreliable) - ServerState.HeaderSize;
                    Debug.Log("_serverState.StartState:" + _serverState.StartState +" pCount:" + pCount +" statesMax:" + statesMax);
                    statesMax /= PlayerState.Size;  //每个客户端能分配到的包体大小
                    for (int s = 0; s < (pCount-1)/statesMax + 1; s++)
                    {
                        //TODO: divide
                        //如果PlayerState.Size很大，超过了statesMax，要怎么解决呢？
                        _serverState.LastProcessedCommand = p.LastProcessedCommandId;
                        _serverState.PlayerStatesCount = pCount;
                        _serverState.StartState = s * statesMax;
                        
                        //给每个Peer都发送当前房间内所有玩家的信息
                        //注意
                        p.AssociatedPeer.Send(WritePacket(_serverState), DeliveryMethod.Unreliable);
                    }
                }
            }
        }
        
        private void Update()
        {
            _netManager.PollEvents();
            _logicTimer.Update();
        }
        
        //填充数据到DataWriter中，为发送做准备
        private NetDataWriter WritePacket<T>(T packet) where T : struct, INetSerializable
        {
            _cachedWriter.Reset();
            _cachedWriter.Put((byte) PacketType.Serialized);
            packet.Serialize(_cachedWriter);
            return _cachedWriter;
        }

        // //填充数据到DataWriter中，为发送做准备
        // private NetDataWriter WritePacket<T>(T packet) where T : class, new()
        // {
        //     _cachedWriter.Reset();
        //     _cachedWriter.Put((byte) PacketType.Serialized);
        //     _packetProcessor.Write(_cachedWriter, packet);
        //     return _cachedWriter;
        // }

        //收到新的客户端(peer)加入房间请求的消息
        private void OnJoinReceived(JoinPacket joinPacket, NetPeer peer)
        {
            Debug.Log("[S] Join packet received: " + joinPacket.UserName);
            var player = new ServerPlayer(_playerManager, joinPacket.UserName, peer);
            _playerManager.AddPlayer(player);

            player.Spawn(new FixVector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f)));

            //Send join accept  向连入的客户端发送同意连接Packet
            var ja = new JoinAcceptPacket { Id = player.Id, ServerTick = _serverTick };
            peer.Send(WritePacket(ja), DeliveryMethod.ReliableOrdered);

            //Send to old players info about new player 
            var pj = new PlayerJoinedPacket
            {
                UserName = joinPacket.UserName,
                NewPlayer = true,
                InitialPlayerState = player.NetworkState,
                ServerTick = _serverTick
            };
            //向其他已经连入的客户端发送有新的客户端连入的消息
            _netManager.SendToAll(WritePacket(pj), DeliveryMethod.ReliableOrdered, peer);

            //Send to new player info about old players
            pj.NewPlayer = false;
            foreach(ServerPlayer otherPlayer in _playerManager)
            {
                if(otherPlayer == player)
                    continue;
                pj.UserName = otherPlayer.Name;
                pj.InitialPlayerState = otherPlayer.NetworkState;
                peer.Send(WritePacket(pj), DeliveryMethod.ReliableOrdered);
            }
        }

        //收到玩家输入消息（移动，开火）
        private void OnInputReceived(PlayerInputPacket inputPacket, NetPeer peer)
        {
            if (peer.Tag == null)
                return;
            //_cachedCommand.Deserialize(reader);
            var player = (ServerPlayer) peer.Tag;
            
            bool antilagApplied = _playerManager.EnableAntilag(player);
            player.ApplyInput(inputPacket, LogicTimer.FixedDelta);
            if(antilagApplied)
                _playerManager.DisableAntilag();
        }

        //通知其他客户端有玩家开火了
        public void SendShoot(ref ShootPacket sp)
        {
            _netManager.SendToAll(WritePacket(sp), DeliveryMethod.ReliableUnordered);
        }

        void INetEventListener.OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[S] Player connected: " + peer.EndPoint);
        }

        //某个客户端断开连接了，需要通知其他客户端
        void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[S] Player disconnected: " + disconnectInfo.Reason);

            if (peer.Tag != null)
            {
                byte playerId = (byte)peer.Id;
                //服务器先删除断开连接的玩家，然后再通知其他客户端有个客户端断开连接了，你们看着办吧
                if (_playerManager.RemovePlayer(playerId))
                {
                    var plp = new PlayerLeavedPacket { Id = (byte)peer.Id };
                    _netManager.SendToAll(WritePacket(plp), DeliveryMethod.ReliableOrdered);
                }
            }
        }

        void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.Log("[S] NetworkError: " + socketError);
        }

        //客户端连接成功后，接收客户端消息接口
        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte packetType = reader.GetByte();
            if (packetType >= NetworkGeneral.PacketTypesCount)
                return;
            PacketType pt = (PacketType) packetType;
            switch (pt)
            {
                //不同协议类型，有不同的处理方法
                // case PacketType.Movement:
                //     OnInputReceived(reader, peer);
                //     break;
                case PacketType.Serialized:
                    _packetProcessor.ReadAllPackets(reader, peer);
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

        //网络有延迟，输出该延迟时间
        void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            if (peer.Tag != null)
            {
                var p = (ServerPlayer) peer.Tag;  
                p.Ping = latency;
            }
        }

        //接受客户端的连接请求
        void INetEventListener.OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey("ExampleGame");
        }
        
            
        private void OnDestroy()
        {
            _netManager.Stop();
            _logicTimer.Stop();
        }

    }
}