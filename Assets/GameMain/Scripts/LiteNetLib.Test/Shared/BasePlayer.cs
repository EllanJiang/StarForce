﻿/*
* 文件名：BasePlayer
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:04:08
* 修改记录：
*/

using System;
using LogicShared.LiteNetLib.Utils;
using LogicShared.TrueSync.Math;
using Protos;

namespace LiteNetLib.Test.Shared
{
    /// <summary>
    /// 玩家基类
    /// </summary>
    public abstract class BasePlayer
    {
        public readonly string Name;            //玩家名字

        private float _speed = 3f;              //玩家移动速度
        private GameTimer _shootTimer = new GameTimer(0.2f);  //技能CD=0.2s
        private BasePlayerManager _playerManager;
        
        protected FixVector2 _position;
        protected float _rotation;
        protected byte _health;

        public const float Radius = 0.5f;
        public bool IsAlive => _health > 0;     //玩家血量
        public byte Health => _health;
        public FixVector2 Position => _position;   //玩家位置
        public float Rotation => _rotation;     //旋转角度
        public readonly byte Id;                //玩家Id
        public int Ping;

        protected BasePlayer(BasePlayerManager playerManager, string name, byte id)
        {
            Id = id;
            Name = name;
            _playerManager = playerManager;
        }

        public virtual void Spawn(FixVector2 position)
        {
            _position = position;
            _rotation = 0;
            _health = 100;
        }

        //射击
        private void Shoot()
        {
            const float MaxLength = 20f;
            var dir = new FixVector2(Math.Cos(_rotation), Math.Sin(_rotation));
            var player = _playerManager.CastToPlayer(_position, dir, MaxLength, this);
            var target = _position + dir * (player != null ? FixVector2.Distance(_position, player._position) : MaxLength);
            _playerManager.OnShoot(this, target, player);
        }

        //应用玩家输入
        public virtual void ApplyInput(PlayerInputPacket command, float delta)
        {
            FixVector2 velocity = FixVector2.zero;
            
            if ((command.Keys & (int)MovementKeys.Up) != 0)
                velocity.y = -1f;
            if ((command.Keys & (int)MovementKeys.Down) != 0)
                velocity.y = 1f;
            
            if ((command.Keys & (int)MovementKeys.Left) != 0)
                velocity.x = -1f;
            if ((command.Keys & (int)MovementKeys.Right) != 0)
                velocity.x = 1f;     
            
            _position += velocity.normalized * _speed * delta;
            _rotation = command.Rotation;

            if ((command.Keys & (int)MovementKeys.Fire) != 0)
            {
                //CD到了
                if (_shootTimer.IsTimeElapsed)
                {
                    _shootTimer.Reset();
                    Shoot();  //射击！
                }
            }
            
        }

        /// <summary>
        /// 更新射击的技能CD
        /// </summary>
        /// <param name="delta"></param>
        public virtual void Update(float delta)
        {
            _shootTimer.UpdateAsCooldown(delta);
        }
    }
}