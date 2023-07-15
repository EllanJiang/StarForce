/*
  作者：LTH
  文件描述：
  文件名：SnapshotStreamReader
  创建时间：2023/07/15 23:07:SS
*/

using System;
using Entt.Entities;

namespace Entt.Serialization
{
    /// <summary>
    /// 快照流读取器
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public class SnapshotStreamReader<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly ISnapshotLoader<TEntityKey> loader;
        readonly IEntityKeyMapper entityMapper;
        
        public SnapshotStreamReader(ISnapshotLoader<TEntityKey> loader, IEntityKeyMapper mapper)
        {
            this.loader = loader ?? throw new ArgumentNullException(nameof(loader));
            this.entityMapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        /// <summary>
        /// 从reader中读取Entity
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public SnapshotStreamReader<TEntityKey> ReadEntities(IEntityArchiveReader<TEntityKey> reader)
        {
            var count = reader.ReadEntityCount();
            for (int i = 0; i < count; i++)
            {
                var entity = reader.ReadEntity(entityMapper);
                loader.OnEntity(entity);
            }

            return this;
        }

        /// <summary>
        /// 从reader中读取component
        /// </summary>
        /// <param name="reader"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public SnapshotStreamReader<TEntityKey> ReadComponent<TComponent>(IEntityArchiveReader<TEntityKey> reader)
        {
            var count = reader.ReadComponentCount<TComponent>();
            for (int i = 0; i < count; i++)
            {
                if (reader.TryReadComponent(entityMapper, out TEntityKey entity, out TComponent component))
                {
                    loader.OnComponent(entity, component);
                }
            }

            return this;
        }

        /// <summary>
        /// 从reader中读取标签
        /// </summary>
        /// <param name="reader"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public SnapshotStreamReader<TEntityKey> ReadTag<TComponent>(IEntityArchiveReader<TEntityKey> reader)
        {
            if (reader.ReadTagFlag<TComponent>())
            {
                if (reader.TryReadTag(entityMapper, out TEntityKey entity, out TComponent component))
                {
                    loader.OnTag(entity, component);
                }
            }
            else
            {
                loader.OnTagRemoved<TComponent>();
            }

            return this;
        }

        /// <summary>
        /// 从reader中读取已经被销毁的entity
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public SnapshotStreamReader<TEntityKey> ReadDestroyed(IEntityArchiveReader<TEntityKey> reader)
        {
            var count = reader.ReadDestroyedCount();
            for (int i = 0; i < count; i++)
            {
                var entity = reader.ReadDestroyed(entityMapper);
                loader.OnDestroyedEntity(entity);
            }

            return this;
        }
    }
}