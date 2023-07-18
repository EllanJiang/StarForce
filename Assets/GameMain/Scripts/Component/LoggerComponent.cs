/*
* 文件名：LoggerComponent
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/18 15:18:20
* 修改记录：
*/

using System;
using LogicShared;
using UnityGameFramework.Runtime;

namespace GameMain
{
    /// <summary>
    /// 日志组件
    /// </summary>
    public class LoggerComponent:GameFrameworkComponent
    {
        private GameLogger gameLogger;
        private RecordGameLogger recordGameLogger;

        protected override void Awake()
        {
            base.Awake();
            //初始化日志
            gameLogger = new GameLogger();
            Logger.logger = gameLogger;

            //记录日志
            recordGameLogger = new RecordGameLogger();
        }

        private void Update()
        {
            gameLogger.OnUpdate();
        }
    }
}