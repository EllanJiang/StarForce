/*
* 文件名：Logger
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/18 15:02:05
* 修改记录：
*/

using System.Linq;

namespace LogicShared
{
    public class Logger
    {
        public static ILogger logger;
        public static void Debug(string message)
        {
            logger?.Debug(message);
        }

        public static void Warning(string message)
        {
            logger?.Warning(message);
        }

        public static void Error(string message)
        {
            logger?.Error(message);
        }
    }
}