/*
* 文件名：NotPool
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/21 19:59:56
* 修改记录：
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Entt.Entities.Helpers;

namespace Entt.Entities.Pools
{
    /// <summary>
    /// 组件缺失对象池
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    public class NotPool<TEntityKey, TComponent>: IReadOnlyPool<TEntityKey, Not<TComponent>> 
        where TEntityKey : IEntityKey
    {
        public event EventHandler<TEntityKey>? Destroyed;
        public event EventHandler<TEntityKey>? Created;
        
        public int Count => registry.Count - entityPool.Count;
        
        private readonly IEntityPoolAccess<TEntityKey> registry;
        private readonly IReadOnlyPool<TEntityKey, TComponent> entityPool;
        
        public NotPool(IEntityPoolAccess<TEntityKey> registry, 
            IReadOnlyPool<TEntityKey, TComponent> entityPool)
        {
            this.registry = registry;
            this.entityPool = entityPool ?? throw new ArgumentNullException(nameof(entityPool));
            this.entityPool.Created += HandleCreated;
            this.entityPool.Destroyed += HandleDestroyed;
        }
        
        void HandleCreated(object sender, TEntityKey entityKey)
        {
            Destroyed?.Invoke(this, entityKey);
        }

        void HandleDestroyed(object sender, TEntityKey entityKey)
        {
            Created?.Invoke(this, entityKey);
        }
        
        public bool TryGet(TEntityKey entityKey, out Not<TComponent> component)
        {
            component = default;
            return Contains(entityKey);
        }
        
        public bool Contains(TEntityKey entityKey)
        {
            return registry.IsValid(entityKey) && !entityPool.Contains(entityKey);
        }
        
        public ref readonly Not<TComponent> TryGetRef(TEntityKey entity, ref Not<TComponent> defaultValue, out bool success)
        {
            success = Contains(entity);
            return ref defaultValue;
        }
        
        public void Reserve(int capacity)
        {
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<TEntityKey> IEnumerable<TEntityKey>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Enumerator GetEnumerator() => new Enumerator(this);
        
        public void CopyTo(RawList<TEntityKey> entities)
        {
            entities.Capacity = Math.Max(entities.Capacity, Count);
            entities.Clear();
            
            var list = EntityKeyListPool.Reserve(registry);
            try
            {
                foreach (var entityKey in list)
                {
                    if (Contains(entityKey))
                    {
                        entities.Add(entityKey);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(list);
            }
        }
        
        public void CopyTo(SparseSet<TEntityKey> entities)
        {
            entities.Capacity = Math.Max(entities.Capacity, Count);
            entities.RemoveAll();
            
            var list = EntityKeyListPool.Reserve(registry);
            try
            {
                foreach (var entityKey in list)
                {
                    if (Contains(entityKey))
                    {
                        entities.Add(entityKey);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(list);
            }
        }

        public struct Enumerator : IEnumerator<TEntityKey>
        {
            readonly NotPool<TEntityKey, TComponent> backend;
            readonly RawList<TEntityKey> contents;
            int index;

            public Enumerator(NotPool<TEntityKey, TComponent> backend)
            {
                this.backend = backend;
                contents = EntityKeyListPool.Reserve(backend.registry);
                Current = default!;
                index = -1;
            }

            public bool MoveNext()
            {
                while (index + 1 < contents.Count)
                {
                    index += 1;
                    if (backend.Contains(contents[index]))
                    {
                        Current = contents[index];
                        return true;
                    }
                }

                Current = default!;
                return false;
            }
            
            public TEntityKey Current { get; private set; }
            object IEnumerator.Current => Current;
            

            public void Reset()
            {
                index = -1;
                Current = default!;
            }
            
            public void Dispose()
            {
                EntityKeyListPool.Release(contents);
            }

        }
    }
}