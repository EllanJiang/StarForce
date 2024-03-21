/*
  作者：LTH
  文件描述：
  文件名：OptionalResolver
  创建时间：2023/07/16 21:07:SS
*/

using System;
using MessagePack;
using MessagePack.Formatters;

namespace Entt.Serialization.Binary
{
    /// <summary>
    /// 可选的解析器
    /// </summary>
    public class OptionalResolver: IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new OptionalResolver();

        OptionalResolver()
        {
        }

        public IMessagePackFormatter<T>? GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        /// <summary>
        /// 缓存解析器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T>? Formatter;

            // generic's static constructor should be minimized for reduce type generation size!
            // use outer helper method.
            static FormatterCache()
            {
                var t = typeof(T);
                if (!t.IsGenericType || t.GetGenericTypeDefinition() != typeof(Optional<>))
                {
                    Formatter = null;
                    return;
                }

                var args = t.GetGenericArguments();
                var formatterType = typeof(OptionalMessagePackFormatter<>).MakeGenericType(args);
                Formatter = (IMessagePackFormatter<T>)Activator.CreateInstance(formatterType);
            }
        }
    }
}