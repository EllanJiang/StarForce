/*
* 文件名：ILogger
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/18 14:59:15
* 修改记录：
*/

namespace LogicShared
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILogger
    {
        void Debug(string message);
        void Warning(string message);
        void Error(string message);
    }

    public enum LoggerType
    {
        Debug = 0,
        Waring,
        Error,
        Assert,
        Exception,
        Max
    }
}