/*
  作者：LTH
  文件描述：
  文件名：EntityKeyResolver
  创建时间：2023/07/16 21:07:SS
*/

using Entt.Entities;
using MessagePack;
using MessagePack.Formatters;

namespace Entt.Serialization.Binary.Impl
{
    public class EntityKeyResolver : IFormatterResolver
    {
        readonly IEntityKeyMapper entityMapper;
        EntityKeyFormatter? keyFormatterInstance;

        /// <summary>
        /// 获取EntityKey的解析器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IMessagePackFormatter<T>? GetFormatter<T>()
        {
            if (typeof(T) == typeof(EntityKey))
            {
                if (keyFormatterInstance == null)
                {
                    keyFormatterInstance = new EntityKeyFormatter(entityMapper);
                }

                return (IMessagePackFormatter<T>)keyFormatterInstance;
            }

            return null;
        }

        public EntityKeyResolver(IEntityKeyMapper? mapper = null)
        {
            this.entityMapper = mapper ?? new DefaultEntityKeyMapper().Register(Map);
        }
        
        EntityKey Map(EntityKeyData data)
        {
            return new EntityKey(data.Age, data.Key);
        }
    }
}