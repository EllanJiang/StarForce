/*
* 文件名：LogHelper
* 文件描述：日志接口
* 作者：aronliang
* 创建时间：2023/06/19 19:40:01
* 修改记录：
*/

using System;
using Serilog;
using Serilog.Core;

namespace Entt
{
    public class LogHelper
    {
        public static ILogger ForContext<T>()
        {
            return Log.ForContext(Constants.SourceContextPropertyName, NameWithoutGenerics(typeof(T)));
        }
        
        public static ILogger ForContext(Type type)
        {
            return Log.ForContext(Constants.SourceContextPropertyName, NameWithoutGenerics(type));
        }
        
        private static string NameWithoutGenerics(Type t)
        {
            var name = t.FullName ?? t.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
    }
}