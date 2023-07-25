/*
* 文件名：ServerPlayerManager
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:08:42
* 修改记录：
*/

using System.Collections.Generic;
using LiteNetLib.Test.Shared;
using UnityEngine;

namespace LiteNetLib.Test.Server
{
    /// <summary>
    /// 服务器玩家管理器
    /// </summary>
    public class ServerPlayerManager : BasePlayerManager
    {
        private readonly ServerLogic _serverLogic;      //服务器处理逻辑
        private readonly ServerPlayer[] _players;       //连入的玩家
        private readonly AntiLagSystem _antiLagSystem;  //抗延迟系统
        
        public readonly PlayerState[] PlayerStates;     //连入的所有玩家的信息（id，位置，旋转等）
        private int _playersCount;                      //连入的玩家数量
        
        
        public override int Count => _playersCount;     

        public ServerPlayerManager(ServerLogic serverLogic)
        {
            _serverLogic = serverLogic;
            _antiLagSystem = new AntiLagSystem(60, ServerLogic.MaxPlayers);
            _players = new ServerPlayer[ServerLogic.MaxPlayers];
            PlayerStates = new PlayerState[ServerLogic.MaxPlayers];
        }

        public bool EnableAntilag(ServerPlayer forPlayer)
        {
            return _antiLagSystem.TryApplyAntiLag(_players, _serverLogic.Tick, forPlayer.AssociatedPeer.Id);
        }

        public void DisableAntilag()
        {
            _antiLagSystem.RevertAntiLag(_players);            
        }

        public override IEnumerator<BasePlayer> GetEnumerator()
        {
            int i = 0;
            while (i < _playersCount)
            {
                yield return _players[i];
                i++;
            }
        }

        //from射击hit玩家
        public override void OnShoot(BasePlayer from, Vector2 to, BasePlayer hit)
        {
            var serverPlayer = (ServerPlayer) from;
            //向其他客户端通知该玩家（from）开火了
            ShootPacket sp = new ShootPacket
            {
                FromPlayer = serverPlayer.Id,
                CommandId = serverPlayer.LastProcessedCommandId,
                ServerTick = _serverLogic.Tick,
                Hit = to
            };
            _serverLogic.SendShoot(ref sp);
        }

        //新增一个客户端
        public void AddPlayer(ServerPlayer player)
        {
            for (int i = 0; i < _playersCount; i++)
            {
                if (_players[i].Id == player.Id)
                {
                    _players[i] = player;
                    return;
                }
            }

            _players[_playersCount] = player;
            _playersCount++;
        }
        
        //移除一个客户端
        public bool RemovePlayer(byte playerId)
        {
            for (int i = 0; i < _playersCount; i++)
            {
                if (_players[i].Id == playerId)
                {
                    _playersCount--;
                    _players[i] = _players[_playersCount];
                    _players[_playersCount] = null;
                    return true;
                }
            }
            return false;
        }
        
        //每帧更新一次逻辑
        public override void LogicUpdate()
        {
            for (int i = 0; i < _playersCount; i++)
            {
                var p = _players[i];
                p.Update(LogicTimer.FixedDelta);
                PlayerStates[i] = p.NetworkState;
            }
        }

    }
}