/*
  作者：LTH
  文件描述：
  文件名：EntityKeyFormatter
  创建时间：2023/07/16 21:07:SS
*/

using Entt.Entities;
using MessagePack;
using MessagePack.Formatters;

namespace Entt.Serialization.Binary.Impl
{
    public class EntityKeyFormatter: IMessagePackFormatter<EntityKey>
    {
        readonly IEntityKeyMapper entityMapper;

        public EntityKeyFormatter(IEntityKeyMapper entityMapper)
        {
            this.entityMapper = entityMapper;
        }

        /// <summary>
        /// 序列化EntityKey
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public void Serialize(ref MessagePackWriter writer, EntityKey value, MessagePackSerializerOptions options)
        {
            writer.Write(value.Age);
            writer.Write(value.Key);
        }

        /// <summary>
        /// 反序列化EntityKey
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public EntityKey Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var age = reader.ReadByte();
            var key = reader.ReadInt32();
            var entityKey = entityMapper.EntityKeyMapper<EntityKey>(new EntityKeyData(age, key));
            return entityKey;
        }
    }
}