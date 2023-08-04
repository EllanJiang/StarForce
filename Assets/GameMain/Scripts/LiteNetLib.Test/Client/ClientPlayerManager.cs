/*
* 文件名：ClientPlayerManager
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:59:59
* 修改记录：
*/

using System.Collections.Generic;
using LiteNetLib.Test.Shared;
using LogicShared.TrueSync.Math;
using Protos;
using UnityEngine;

namespace LiteNetLib.Test.Client
{
    public struct PlayerHandler
    {
        public readonly BasePlayer Player;
        public readonly IPlayerView View;

        public PlayerHandler(BasePlayer player, IPlayerView view)
        {
            Player = player;
            View = view;
        }

        public void Update(float delta)
        {
            Player.Update(delta);
        }
    }

    public class ClientPlayerManager : BasePlayerManager
    {
        private readonly Dictionary<byte, PlayerHandler> _players;
        private readonly ClientLogic _clientLogic;      //客户端逻辑
        private ClientPlayer _clientPlayer;

        public ClientPlayer OurPlayer => _clientPlayer; //自己
        public override int Count => _players.Count;    //房间玩家数量

        public ClientPlayerManager(ClientLogic clientLogic)
        {
            _clientLogic = clientLogic;
            _players = new Dictionary<byte, PlayerHandler>();
        }
        
        public override IEnumerator<BasePlayer> GetEnumerator()
        {
            foreach (var kv in _players)
                yield return kv.Value.Player;
        }

        //设置服务器信息
        public void ApplyServerState(ref ServerState serverState)
        {
            for (int i = 0; i < serverState.PlayerStatesCount; i++)
            {
                var state = serverState.PlayerStates[i];
                if(!_players.TryGetValue(state.Id, out var handler))
                    return;

                if (handler.Player == _clientPlayer)  //如果是自己，那么处理预测命令
                {
                    _clientPlayer.ReceiveServerState(serverState, state);
                }
                else
                {
                    var rp = (RemotePlayer)handler.Player;
                    rp.OnPlayerState(state);
                }
            }
        }

        //自己射击
        public override void OnShoot(BasePlayer from, FixVector2 to, BasePlayer hit)
        {
            if(from == _clientPlayer)
                _clientLogic.SpawnShoot(from.Position, to);
        }

        public BasePlayer GetById(byte id)
        {
            return _players.TryGetValue(id, out var ph) ? ph.Player : null;
        }

        public BasePlayer RemovePlayer(byte id)
        {
            if (_players.TryGetValue(id, out var handler))
            {
                _players.Remove(id);
                handler.View.Destroy();
            }
        
            return handler.Player;
        }

        public override void LogicUpdate()
        {
            foreach (var kv in _players)
                kv.Value.Update(LogicTimer.FixedDelta);
        }

        public void AddClientPlayer(ClientPlayer player, ClientPlayerView view)
        {
            _clientPlayer = player;
            _players.Add(player.Id, new PlayerHandler(player, view));
        }
        
        public void AddPlayer(RemotePlayer player, RemotePlayerView view)
        {
            _players.Add(player.Id, new PlayerHandler(player, view));
        }

        public void Clear()
        {
            foreach (var p in _players.Values)
                p.View.Destroy();
            _players.Clear();
        }
    }
}