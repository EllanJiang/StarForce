/*
  作者：LTH
  文件描述：
  文件名：EntityKeySurrogateProvider
  创建时间：2023/07/16 17:07:SS
*/

using Entt.Entities;

namespace Entt.Serialization.Xml
{
    /// <summary>
    /// EntityKey序列化解析器
    /// </summary>
    public class EntityKeySurrogateProvider : SerializationSurrogateProviderBase<EntityKey, EntityKeyData>
    {
        readonly IEntityKeyMapper entityMapper;

        public EntityKeySurrogateProvider(IEntityKeyMapper? entityMapper = null)
        {
            this.entityMapper = entityMapper ?? new DefaultEntityKeyMapper().Register(Map);
        }

        /// <summary>
        /// 创建一个新的EntityKey
        /// </summary>
        /// <param name="surrogate"></param>
        /// <returns></returns>
        EntityKey Map(EntityKeyData surrogate)
        {
            return new EntityKey(surrogate.Age, surrogate.Key);
        }

        public override EntityKey GetDeserializedObject(EntityKeyData surrogate)
        {
            return entityMapper.EntityKeyMapper<EntityKey>(surrogate);
        }

        public override EntityKeyData GetObjectToSerialize(EntityKey obj)
        {
            return new EntityKeyData(obj.Age, obj.Key);
        }
    }
}