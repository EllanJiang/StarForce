/*
* 文件名：LogUtil
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/30 19:47:59
* 修改记录：
*/
using UnityEngine;
namespace Entt.Test
{
    public static class LogUtil
    {
        public static void Log(object message)
        {
            Debug.Log(message);
        }
        
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }
        
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }
        
        public static void LogAssertion(object message)
        {
            Debug.LogAssertion(message);
        }
        
        public static void Log(string message,params object[] args)
        {
            Debug.LogFormat(message,args);
        }
        
        public static void LogWaring(string message,params object[] args)
        {
            Debug.LogWarningFormat(message,args);
        }
     
        public static void LogError(string message,params object[] args)
        {
            Debug.LogErrorFormat(message);
        }
        
        public static void LogAssertion(string message,params object[] args)
        {
            Debug.LogAssertionFormat(message,args);
        }
    }
}