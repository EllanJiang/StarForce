/*
  作者：LTH
  文件描述：
  文件名：OptionalMessagePackFormatter
  创建时间：2023/07/16 21:07:SS
*/

using MessagePack;
using MessagePack.Formatters;

namespace Entt.Serialization.Binary
{
    /// <summary>
    /// 可选的MessagePack解析器
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class OptionalMessagePackFormatter<TValue>: IMessagePackFormatter<Optional<TValue>>
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public void Serialize(ref MessagePackWriter writer, Optional<TValue> value, MessagePackSerializerOptions options)
        {
            if (value.TryGetValue(out var containedValue))
            {
                MessagePackSerializer.Serialize(ref writer, containedValue, options);
            }
            else
            {
                writer.WriteNil();
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Optional<TValue> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return Optional.Empty();
            }

            var v = MessagePackSerializer.Deserialize<TValue>(ref reader, options);
            return Optional.ValueOf(v);
        }
    }
}