/*
* 文件名：BasePlayer
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:04:08
* 修改记录：
*/

using UnityEngine;

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
        
        protected Vector2 _position;
        protected float _rotation;
        protected byte _health;

        public const float Radius = 0.5f;
        public bool IsAlive => _health > 0;     //玩家血量
        public byte Health => _health;
        public Vector2 Position => _position;   //玩家位置
        public float Rotation => _rotation;     //旋转角度
        public readonly byte Id;                //玩家Id
        public int Ping;

        protected BasePlayer(BasePlayerManager playerManager, string name, byte id)
        {
            Id = id;
            Name = name;
            _playerManager = playerManager;
        }

        public virtual void Spawn(Vector2 position)
        {
            _position = position;
            _rotation = 0;
            _health = 100;
        }

        //射击
        private void Shoot()
        {
            const float MaxLength = 20f;
            Vector2 dir = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation));
            var player = _playerManager.CastToPlayer(_position, dir, MaxLength, this);
            Vector2 target = _position + dir * (player != null ? Vector2.Distance(_position, player._position) : MaxLength);
            _playerManager.OnShoot(this, target, player);
        }

        //应用玩家输入
        public virtual void ApplyInput(PlayerInputPacket command, float delta)
        {
            Vector2 velocity = Vector2.zero;
            
            if ((command.Keys & MovementKeys.Up) != 0)
                velocity.y = -1f;
            if ((command.Keys & MovementKeys.Down) != 0)
                velocity.y = 1f;
            
            if ((command.Keys & MovementKeys.Left) != 0)
                velocity.x = -1f;
            if ((command.Keys & MovementKeys.Right) != 0)
                velocity.x = 1f;     
            
            _position += velocity.normalized * _speed * delta;
            _rotation = command.Rotation;

            if ((command.Keys & MovementKeys.Fire) != 0)
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