/*
* 文件名：GameLogger
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/18 15:10:40
* 修改记录：
*/

using System.Collections.Concurrent;
using LogicShared;
using System;
using System.Diagnostics;
using System.Text;
using UnityGameFramework.Runtime;

namespace GameMain
{
    /// <summary>
    /// 游戏日志入口
    /// </summary>
    public class GameLogger:ILogger
    {
        private int loggerCount;
        private ConcurrentQueue<string>[] LoggerQueues;

        private int mainThreadId;

        bool IsMainThread
        {
            get
            {
#if UNITY_EDITOR
                return true;
#endif
                return System.Threading.Thread.CurrentThread.ManagedThreadId == mainThreadId;
            }
        }

        public GameLogger()
        {
            mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            LoggerQueues = new ConcurrentQueue<string>[(int)LoggerType.Max];
            foreach (int loggerType in Enum.GetValues(typeof(LoggerType)))
            {
                if (loggerType != (int)LoggerType.Max)
                {
                    LoggerQueues[loggerType] = new ConcurrentQueue<string>();
                }
            }
        }
        
        public void Debug(string message)
        {
            if (IsMainThread)
            {
                Log.Debug(message);
            }
            else
            {
                LoggerQueues[(int)LoggerType.Debug].Enqueue(GetStackTrack(message));
                loggerCount++;
            }
        }

        public void Warning(string message)
        {
            if (IsMainThread)
            {
                Log.Warning(message);
            }
            else
            {
                LoggerQueues[(int)LoggerType.Waring].Enqueue(GetStackTrack(message));
                loggerCount++;
            }
        }

        public void Error(string message)
        {
            if (IsMainThread)
            {
                Log.Error(message);
            }
            else
            {
                LoggerQueues[(int)LoggerType.Error].Enqueue(GetStackTrack(message));
                loggerCount++;
            }
        }

        //获取追踪日志
        private string GetStackTrack(string message)
        {
            var sb = new StringBuilder();
            sb.Append(message);
            StackTrace stackTrace = new StackTrace(true);
            StackFrame[] stackFrames = stackTrace.GetFrames();
            if (stackFrames == null)
            {
                return sb.ToString();
            }
            for (int i = 0; i < stackFrames.Length; i++)
            {
                var stackFrame = stackFrames[i];
                sb.Append("\r\n\t\t");
                sb.Append(stackFrame);
            }

            return sb.ToString();
        }
        
        //更新子线程的日志
        public void OnUpdate()
        {
            if (loggerCount <= 0)
            {
                return;
            }

            for (int i = 0; i < LoggerQueues.Length; i++)
            {
                var loggerMessages = LoggerQueues[i];
                if (loggerMessages.Count > 0)
                {
                    string message = null;
                    if (loggerMessages.TryDequeue(out message))
                    {
                        switch ((LoggerType)i)
                        {
                            case LoggerType.Debug:
                                Log.Debug(message);
                                break;
                            case LoggerType.Waring:
                                Log.Warning(message);
                                break;
                            case LoggerType.Error:
                                Log.Error(message);
                                break;
                        }

                        loggerCount--;
                    }
                }
            }
        }
    }
}