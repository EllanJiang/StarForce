/*
* 文件名：GameTimer
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:02:31
* 修改记录：
*/

using System;

namespace LiteNetLib.Test.Shared
{
    /// <summary>
    /// 非常简单的计时器
    /// </summary>
    public struct GameTimer
    {
        private float _maxTime;
        private float _time;

        //是否超时
        public bool IsTimeElapsed => _time >= _maxTime;

        public float Time => _time;
        
        public float MaxTime
        {
            get => _maxTime;
            set => _maxTime = value;
        }
        
        public GameTimer(float maxTime)
        {
            _maxTime = maxTime;
            _time = 0f;
        }

        public void Reset()
        {
            _time = 0f;
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        /// <param name="delta"></param>
        public void UpdateAsCooldown(float delta)
        {
            _time += delta;
        }
        
        /// <summary>
        /// 更新时间，超时则调用onUpdate方法
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="onUpdate">超时回调方法</param>
        public void Update(float delta, Action onUpdate)
        {
            _time += delta;
            while (_time >= _maxTime)
            {
                _time -= _maxTime;
                onUpdate();
            }
        }
    }
}