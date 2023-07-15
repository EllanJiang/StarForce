/*
  作者：LTH
  文件描述：
  文件名：SnapshotLoader
  创建时间：2023/07/15 22:07:SS
*/

using System;
using System.Collections.Generic;
using Entt.Entities;
using Entt.Entities.Helpers;

namespace Entt.Serialization
{
    /// <summary>
    /// 快照加载器
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public class SnapshotLoader<TEntityKey> : ISnapshotLoader<TEntityKey>, IDisposable 
        where TEntityKey : IEntityKey
    {
        /// <summary>
        /// 远端映射，EntityKeyData到EntityKey的映射
        /// </summary>
        private readonly Dictionary<EntityKeyData, TEntityKey> remoteMapping;
        /// <summary>
        /// 本地映射，EntityKey到EntityKeyData的映射
        /// </summary>
        private readonly Dictionary<TEntityKey, EntityKeyData> localMapping;
        
        protected IEntityPoolAccess<TEntityKey> Registry { get; }
        
        public SnapshotLoader(IEntityPoolAccess<TEntityKey> registry)
        {
            this.Registry = registry ?? throw new ArgumentNullException(nameof(registry));
            registry.BeforeEntityDestroyed += OnLocalEntityDestroyed;
            remoteMapping = new Dictionary<EntityKeyData, TEntityKey>();
            localMapping = new Dictionary<TEntityKey, EntityKeyData>();
        }
        
        public void OnEntity(TEntityKey entity)
        {
            this.Registry.AssureEntityState(entity, false);
        }
        
        public void OnDestroyedEntity(TEntityKey entity)
        {
            this.Registry.AssureEntityState(entity, true);
        }
        
        public void OnComponent<TComponent>(TEntityKey entity, in TComponent component)
        {
            this.Registry.AssignComponent(entity, component);
        }
        
        public void OnTag<TComponent>(TEntityKey entity, in TComponent component)
        {
            this.Registry.AttachTag(entity, component);
        }

        public void OnTagRemoved<TComponent>()
        {
            this.Registry.RemoveTag<TComponent>();
        }

        public TEntityKey Map(TEntityKey input)
        {
            return Map(new EntityKeyData(input.Age, input.Key));
        }
        
        public virtual TEntityKey Map(EntityKeyData input)
        {
            if (remoteMapping.TryGetValue(input, out var mapped))
            {
                return mapped;
            }

            var local = Registry.Create();
            remoteMapping[input] = local;
            localMapping[local] = input;
            return local;
        }
        
        public void CleanOrphans()
        {
            var entityKeys = EntityKeyListPool<TEntityKey>.Reserve(Registry);
            try
            {
                foreach (var ek in entityKeys)
                {
                    if (Registry.IsOrphan(ek))
                    {
                        Registry.Destroy(ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool<TEntityKey>.Release(entityKeys);
            }
        }
        
        void OnLocalEntityDestroyed(object sender, TEntityKey entityKey)
        {
            if (localMapping.TryGetValue(entityKey, out var remoteKey))
            {
                remoteMapping.Remove(remoteKey);
                localMapping.Remove(entityKey);
            }
        }
        public bool TryLookupMapping(TEntityKey input, out TEntityKey mapped)
        {
            return TryLookupMapping(new EntityKeyData(input.Age, input.Key), out mapped);
        }

        public bool TryLookupMapping(EntityKeyData input, out TEntityKey mapped)
        {
            return remoteMapping.TryGetValue(input, out mapped);
        }

        public void Dispose()
        {
            Registry.BeforeEntityDestroyed -= OnLocalEntityDestroyed;
            GC.SuppressFinalize(this);
        }
        
        ~SnapshotLoader()
        {
            Registry.BeforeEntityDestroyed -= OnLocalEntityDestroyed;
        }
    }
}