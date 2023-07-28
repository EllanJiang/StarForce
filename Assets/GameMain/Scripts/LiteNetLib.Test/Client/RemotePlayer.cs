/*
* 文件名：RemotePlayer
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 12:01:54
* 修改记录：
*/

using LiteNetLib.Test.Shared;
using LogicShared.LiteNetLib.Helpers;
using LogicShared.TrueSync.Math;
using UnityEngine;

namespace LiteNetLib.Test.Client
{
    /// <summary>
    /// 远端玩家在本地玩家的代表
    /// </summary>
    public class RemotePlayer : BasePlayer
    {
        private readonly LiteRingBuffer<PlayerState> _buffer = new LiteRingBuffer<PlayerState>(30);
        private float _receivedTime;
        private float _timer;
        private const float BufferTime = 0.1f; //100 milliseconds
        
        public RemotePlayer(ClientPlayerManager manager, string name, PlayerJoinedPacket pjPacket) : base(manager, name, pjPacket.InitialPlayerState.Id)
        {
            _position = pjPacket.InitialPlayerState.Position;
            _health = pjPacket.Health;
            _rotation = pjPacket.InitialPlayerState.Rotation;
            _buffer.Add(pjPacket.InitialPlayerState);
        }

        //生成远端玩家
        public override void Spawn(FixVector2 position)
        {
            _buffer.FastClear();
            base.Spawn(position);
        }

        //更新远端玩家位置
        public void UpdatePosition(float delta)
        {
            if (_receivedTime < BufferTime || _buffer.Count < 2)
                return;
            var dataA = _buffer[0];
            var dataB = _buffer[1];
            
            //插值时间
            float lerpTime = NetworkGeneral.SeqDiff(dataB.Tick, dataA.Tick)*LogicTimer.FixedDelta;
            float t = _timer / lerpTime;
            //位置和旋转插值
            _position = FixVector2.Lerp(dataA.Position, dataB.Position, t);
            _rotation = Mathf.Lerp(dataA.Rotation, dataB.Rotation, t);
            _timer += delta;
            if (_timer > lerpTime)
            {
                _receivedTime -= lerpTime;
                _buffer.RemoveFromStart(1);
                _timer -= lerpTime;
            }
        }

        //设置远端玩家信息
        public void OnPlayerState(PlayerState state)
        {
            //old command
            int diff = NetworkGeneral.SeqDiff(state.Tick, _buffer.Last.Tick);
            if (diff <= 0)
                return;

            _receivedTime += diff * LogicTimer.FixedDelta;
            if (_buffer.IsFull)
            {
                Debug.LogWarning("[C] Remote: Something happened");
                //Lag?
                _receivedTime = 0f;
                _buffer.FastClear();
            }
            _buffer.Add(state);
        }
    }
}