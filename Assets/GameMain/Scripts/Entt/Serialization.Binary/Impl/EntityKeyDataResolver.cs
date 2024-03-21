/*
  作者：LTH
  文件描述：
  文件名：EntityKeyDataResolver
  创建时间：2023/07/16 21:07:SS
*/

using MessagePack;
using MessagePack.Formatters;

namespace Entt.Serialization.Binary.Impl
{
    public class EntityKeyDataResolver : IFormatterResolver
    {
        /// <summary>
        /// 获取解析器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IMessagePackFormatter<T>? GetFormatter<T>()
        {
            if (typeof(T) == typeof(EntityKeyData))
            {
                return (IMessagePackFormatter<T>)EntityKeyDataFormatter.Instance;
            }

            return null;
        }
    }
}