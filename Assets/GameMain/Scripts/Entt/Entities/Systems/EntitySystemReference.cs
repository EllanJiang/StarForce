/*
* 文件名：EntitySystemReference
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/28 17:40:46
* 修改记录：
*/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Entt.Entities.Systems
{
    /// <summary>
    /// 对System的引用
    /// </summary>
    public readonly struct EntitySystemReference
    {
        /// <summary>
        /// 系统Id
        /// </summary>
        public string SystemId { get; }
        /// <summary>
        /// 对系统引用的回调
        /// </summary>
        public Action System { get; }
        
        public EntitySystemReference(string systemId, Action system)
        {
            SystemId = systemId;
            System = system;
        }
        
        /// <summary>
        /// 将EntitySystemReference隐式转换成Action
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static implicit operator Action(EntitySystemReference r)
        {
            return r.System;
        }
        
        /// <summary>
        /// 创建Entity系统（传入上下文）
        /// </summary>
        /// <param name="action"></param>
        /// <param name="source"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static EntitySystemReference<TContext> Create<TContext>(Action<TContext> action, Delegate source)
        {
            var systemId = CreateSystemDescription(source);
            return new EntitySystemReference<TContext>(systemId, action);
        }
        
        /// <summary>
        /// 创建Entity系统
        /// </summary>
        /// <param name="action"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static EntitySystemReference Create(Action action, Delegate source)
        {
            var systemId = CreateSystemDescription(source);
            return new EntitySystemReference(systemId, action);
        }

        /// <summary>
        /// 生成Entity系统Id
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string CreateSystemDescription(Delegate source)
        {
            var targetType = source.Target?.GetType() ?? source.Method.DeclaringType;
            if (IsClosure(targetType, out var baseType))
            {
                var systemId = $"{NameWithoutGenerics(baseType)}#{ClosureMethodNameWithContext(source)}";
                return systemId;
            }
            else
            {
                var systemId = $"{NameWithoutGenerics(targetType)}#{source.Method.Name}";
                return systemId;
            }
        }
        
        /// <summary>
        /// 获取闭包函数的名字（去掉泛型）
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        static string ClosureMethodNameWithContext(Delegate action)
        {
            var rawName = action.Method.Name;
            var re = new Regex(@"<(?<Source>.*)>.__(?<Method>.*)\|.*");
            var m = re.Match(rawName);
            if (!m.Success)
            {
                return rawName;
            }
            
            var methodName = m.Groups["Method"]?.Value;
            var sourceMethod = m.Groups["Source"]?.Value;
            return $"{sourceMethod}.{methodName}";
        }
        
        /// <summary>
        /// 判断传入的类型t是否是闭包函数
        /// </summary>
        /// <param name="t"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        static bool IsClosure(Type? t, [MaybeNullWhen(false)] out Type baseType)
        {
            if (t == null)
            {
                baseType = default;
                return false;
            }
            
            while (t != null && t.Name.StartsWith("<"))
            {
                // closure class. Lets find the source class that declared this one.
                if (t.DeclaringType == null)
                {
                    break;
                }
                
                t = t.DeclaringType;
            }
            
            baseType = t;
            return baseType != null;
        }
        
        /// <summary>
        /// 获取传入的类型t的名字（去掉泛型）
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        static string NameWithoutGenerics(Type? t)
        {
            if (t == null) return "<??>";

            // var name = t.FullName ?? t.Name;
            var name = t.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
    }

    public readonly struct EntitySystemReference<TContext>
    {
        public string SystemId { get; }
        public Action<TContext> System { get; }
        
        public EntitySystemReference(string systemId, Action<TContext> system)
        {
            SystemId = systemId;
            System = system;
        }

        /// <summary>
        /// 把EntitySystemReference<TContext>隐式转换成 Action<TContext>
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static implicit operator Action<TContext>(EntitySystemReference<TContext> r)
        {
            return r.System;
        }
    }
}