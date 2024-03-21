/*
  作者：LTH
  文件描述：
  文件名：AsyncSnapshotView
  创建时间：2023/07/15 22:07:SS
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entt.Entities;
using Entt.Entities.Helpers;

namespace Entt.Serialization
{
    /// <summary>
    /// 异步快照
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public class AsyncSnapshotView<TEntityKey> where TEntityKey : IEntityKey
    {
        private readonly IEntityPoolAccess<TEntityKey> registry;
        private readonly List<TEntityKey> destroyedEntities;
        
        public AsyncSnapshotView(IEntityPoolAccess<TEntityKey> registry)
        {
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.destroyedEntities = new List<TEntityKey>();

            this.registry.BeforeEntityDestroyed += OnEntityDestroyed;
        }

        void OnEntityDestroyed(object sender, TEntityKey e)
        {
            destroyedEntities.Add(e);
        }

        /// <summary>
        /// 异步写入已经销毁的entity
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public async Task<AsyncSnapshotView<TEntityKey>> WriteDestroyed(IAsyncEntityArchiveWriter<TEntityKey> writer)
        {
            await writer.WriteStartDestroyedAsync(destroyedEntities.Count);
            foreach (var destroyedEntity in destroyedEntities)
            {
                await writer.WriteDestroyedAsync(destroyedEntity);
            }

            await writer.WriteEndDestroyedAsync();
            destroyedEntities.Clear();
            return this;
        }
        
        public async Task<AsyncSnapshotView<TEntityKey>> WriteEntities(IAsyncEntityArchiveWriter<TEntityKey> writer)
        {
            await writer.WriteStartEntityAsync(destroyedEntities.Count);
            //因为是异步写入，所以需要先复制一份数据，用来写入
            var entityKeys = EntityKeyListPool<TEntityKey>.Reserve(registry);
            try
            {
                foreach (var entityKey in entityKeys)
                {
                    await writer.WriteEntityAsync(entityKey);
                }

                await writer.WriteEndEntityAsync();
            }
            finally
            {
                EntityKeyListPool.Release(entityKeys);
            }
            
            return this;
        }

        public async Task<AsyncSnapshotView<TEntityKey>> WriteComponent<TComponent>(IAsyncEntityArchiveWriter<TEntityKey> writer)
        {
            var pool = registry.GetPool<TComponent>();
            await writer.WriteStartComponentAsync<TComponent>(pool.Count);
            var entityKeys = EntityKeyListPool.Reserve(pool);
            try
            {
                foreach (var entity in entityKeys)
                {
                    if (pool.TryGet(entity, out var component))
                    {
                        await writer.WriteComponentAsync(entity, component);
                    }
                }

                await writer.WriteEndComponentAsync<TComponent>();
            }
            finally
            {
                EntityKeyListPool.Release(entityKeys);
            }

            return this;
        }

        public async Task<AsyncSnapshotView<TEntityKey>> WriteTag<TComponent>(IAsyncEntityArchiveWriter<TEntityKey> writer)
        {
            if (registry.TryGetTag<TComponent>(out var entity, out var tag))
            {
                await writer.WriteTagAsync(entity, tag);
            }
            else
            {
                await writer.WriteMissingTagAsync<TComponent>();
            }

            return this;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.registry.BeforeEntityDestroyed -= OnEntityDestroyed;
            }
        }

        ~AsyncSnapshotView()
        {
            Dispose(false);
        }
    }
}