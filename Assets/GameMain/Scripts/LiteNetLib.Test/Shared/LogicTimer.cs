/*
* 文件名：LogicTimer
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 10:56:16
* 修改记录：
*/

using System;
using System.Diagnostics;

namespace LiteNetLib.Test.Shared
{
    /// <summary>
    /// 逻辑时间
    /// </summary>
    public class LogicTimer
    {
        public const float FramesPerSecond = 30.0f;  //FPS=30
        public const float FixedDelta = 1.0f / FramesPerSecond; //33ms

        private double _accumulator;    //累积计时时间
        private long _lastTime;         //上一次计时时间

        private readonly Stopwatch _stopwatch;
        private readonly Action _action;

        public float LerpAlpha => (float)_accumulator/FixedDelta;   //[0,1]

        public LogicTimer(Action action)
        {
            _stopwatch = new Stopwatch();
            _action = action;
        }

        public void Start()
        {
            _lastTime = 0;
            _accumulator = 0.0;
            _stopwatch.Restart();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        /// <summary>
        /// 更新逻辑时间
        /// </summary>
        public void Update()
        {
            long elapsedTicks = _stopwatch.ElapsedTicks;
            _accumulator += (double)(elapsedTicks - _lastTime)/Stopwatch.Frequency;  
            _lastTime = elapsedTicks;
            //每帧触发一次的回调方法
            while (_accumulator >= FixedDelta)
            {
                _action();
                _accumulator -= FixedDelta; //每帧调用一次回调方法
            }
        }
    }
}