/*
  作者：LTH
  文件描述：
  文件名：EntityKeyDataFormatter
  创建时间：2023/07/16 21:07:SS
*/

using MessagePack;
using MessagePack.Formatters;

namespace Entt.Serialization.Binary.Impl
{
    public class EntityKeyDataFormatter: IMessagePackFormatter<EntityKeyData>
    {
        public static readonly EntityKeyDataFormatter Instance = new EntityKeyDataFormatter();

        /// <summary>
        /// 序列化EntityKeyData
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public void Serialize(ref MessagePackWriter writer, EntityKeyData value, MessagePackSerializerOptions options)
        {
            writer.Write(value.Age);
            writer.Write(value.Key);
        }

        /// <summary>
        /// 反序列化EntityKeyData
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public EntityKeyData Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var age = reader.ReadByte();
            var key = reader.ReadInt32();
            return new EntityKeyData(age, key);
        }
    }
}