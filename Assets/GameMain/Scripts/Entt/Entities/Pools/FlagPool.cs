/*
* 文件名：FlagPool
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/21 19:47:16
* 修改记录：
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Entt.Entities.Helpers;

namespace Entt.Entities.Pools
{
    /// <summary>
    /// 组件标志对象池，与Pool的主要区别是，FlagPool的内容就是sharedData，backend里面存放的只是EntityKey，不管是哪个Key，从FlagPool中取出的值一定是sharedData
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class FlagPool<TEntityKey, TData> : IPool<TEntityKey, TData> where TEntityKey : IEntityKey
    {
        public event EventHandler<(TEntityKey key, TData old)>? DestroyedNotify;
        public event EventHandler<TEntityKey>? Destroyed;
        public event EventHandler<TEntityKey>? Created;
        public event EventHandler<(TEntityKey key, TData old)>? Updated
        {
            add { }
            remove { }
        }

        
        public int Count => backend.Count;
        
        private readonly TData sharedData;
        private readonly SparseSet<TEntityKey> backend;
        
        public FlagPool(TData sharedData)
        {
            this.sharedData = sharedData;
            backend = new SparseSet<TEntityKey>();
        }
        
        public bool TryGet(TEntityKey entityKey, out TData component)
        {
            component = sharedData;
            return backend.Contains(entityKey);
        }
        
        public void Add(TEntityKey entityKey, in TData component)
        {
            backend.Add(entityKey);
            Created?.Invoke(this, entityKey);
        }
        
        
        public virtual bool WriteBack(TEntityKey entityKey, in TData component)
        {
            return backend.Contains(entityKey);
        }
        
        public bool Contains(TEntityKey entityKey) => backend.Contains(entityKey);
        
        protected TEntityKey Last => backend.Last;
        
        public void Respect(IEnumerable<TEntityKey> otherSet)
        {
            backend.Respect(otherSet);
        }
        
        public void Reserve(int capacity)
        {
            backend.Reserve(capacity);
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<TEntityKey> IEnumerable<TEntityKey>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public SegmentedRawList<TEntityKey>.Enumerator GetEnumerator()
        {
            return backend.GetEnumerator();
        }
        
        public virtual bool Remove(TEntityKey entityKey)
        {
            if (Destroyed == null && DestroyedNotify == null)
            {
                return backend.Remove(entityKey);
            }

            if (DestroyedNotify == null)
            {
                if (backend.Remove(entityKey))
                {
                    Destroyed?.Invoke(this, entityKey);
                    return true;
                }
            }
            else if (TryGet(entityKey, out var old) && backend.Remove(entityKey))
            {
                DestroyedNotify?.Invoke(this, (entityKey, old));
                Destroyed?.Invoke(this, entityKey);
                return true;
            }

            return false;
        }
        
        public virtual void RemoveAll()
        {
            if (Destroyed == null && DestroyedNotify == null)
            {
                backend.RemoveAll();
                return;
            }

            while (backend.Count > 0)
            {
                var k = backend.Last;
                Remove(k);
            }
        }
        
        public ref readonly TData? TryGetRef(TEntityKey entity, ref TData? defaultValue, out bool success)
        {
            if (Contains(entity))
            {
                defaultValue = sharedData;
                success = true;
            }
            else
            {
                success = false;
            }
            return ref defaultValue;
        }

        public ref TData? TryGetModifiableRef(TEntityKey entity, ref TData? defaultValue, out bool success)
        {
            if (Contains(entity))
            {
                defaultValue = sharedData;
                success = true;
            }
            else
            {
                success = false;
            }
            return ref defaultValue;
        }
        
        public void CopyTo(RawList<TEntityKey> entities)
        {
            entities.Capacity = Math.Max(entities.Capacity, Count);
            entities.Clear();
            backend.CopyTo(entities);
        }
    }
}