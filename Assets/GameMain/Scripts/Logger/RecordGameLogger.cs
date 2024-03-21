/*
* 文件名：RecordGameLogger
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/18 15:46:16
* 修改记录：
*/

using System;
using System.IO;
using System.Text;
using LogicShared;
using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 保存游戏日志信息到txt文件中
    /// </summary>
    public class RecordGameLogger
    {
        private string[] loggerNames;
        private string LoggerPath;

        private StreamWriter writer;

        public RecordGameLogger()
        {
            LoggerPath = Application.persistentDataPath + "/Logger.txt";
            loggerNames = new[]
            {
                LoggerType.Debug.ToString(),
                LoggerType.Waring.ToString(),
                LoggerType.Error.ToString()
            };

            if (File.Exists(LoggerPath))
            {
                File.Delete(LoggerPath);
            }

            writer = new StreamWriter(LoggerPath, true, Encoding.UTF8);
            writer.WriteLine("++++++++++++++++++++++++++++++++++++ Device Info Start ++++++++++++++++++++++++++++++++++");
            SaveDeviceInfo();
            SaveGameInfo();
            writer.WriteLine("++++++++++++++++++++++++++++++++++++ Device Info End ++++++++++++++++++++++++++++++++++ \r\n\r\n");
            
            writer.WriteLine("++++++++++++++++++++++++++++++++++++ Game Logger Start ++++++++++++++++++++++++++++++++++");
            Application.logMessageReceived += MessageLogCallback;
        }

        //硬件信息
        private void SaveDeviceInfo()
        {
            //设备Id
            writer.WriteLine($"Device Name:{SystemInfo.deviceName}");
            writer.WriteLine($"Device Model:{SystemInfo.deviceModel}");
            writer.WriteLine($"Device Unique Identifier:{SystemInfo.deviceUniqueIdentifier}");
            //CPU
            writer.WriteLine($"Device Processor Type:{SystemInfo.processorType}");
            writer.WriteLine($"Device Processor Count:{SystemInfo.processorCount}");
            writer.WriteLine($"Device Processor Frequency:{SystemInfo.processorFrequency}");
            //OS
            writer.WriteLine($"Device Operating System:{SystemInfo.operatingSystem}");
            
            //GPU
            writer.WriteLine($"Device Graphics Device Name:{SystemInfo.graphicsDeviceName}");
            writer.WriteLine($"Device Graphics Device Type:{SystemInfo.graphicsDeviceType}");
            writer.WriteLine($"Device Graphics Memory Size:{SystemInfo.graphicsMemorySize}");
            writer.WriteLine($"Device Graphics Device Version:{SystemInfo.graphicsDeviceVersion}");
        }
        
        //游戏信息
        private void SaveGameInfo()
        {
            writer.WriteLine($"Game Framework Version: {GameFramework.Version.GameFrameworkVersion}");
            writer.WriteLine($"Game Version: {GameFramework.Version.GameVersion}");
            writer.WriteLine($"Internal Game Version: {GameFramework.Version.InternalGameVersion}");
            
            writer.WriteLine($"Application Version: {Application.version}");
            writer.WriteLine($"Platform: {Application.platform}");
            writer.WriteLine($"System Language: {Application.systemLanguage}");
        }
        
        //记录硬件信息
        void MessageLogCallback(string logString, string stackTrace, LogType logType)
        {
            if (writer != null)
            {
                writer.Write(GetLoggerType(logType));
                writer.Write(logString);
                writer.WriteLine(stackTrace);
            }
            else
            {
                Debug.Log("导出logger失败！");
            }
        }

        private LoggerType GetLoggerType(LogType logType)
        {
            switch (logType)
            {
                case LogType.Log:
                    return LoggerType.Debug;
                case LogType.Warning:
                    return LoggerType.Waring;
                case LogType.Error:
                    return LoggerType.Error;
                case LogType.Assert:
                    return LoggerType.Assert;
                case LogType.Exception:
                    return LoggerType.Exception;
            }

            return LoggerType.Debug;
        }
    }
}