/*
* 文件名：Pool
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/20 16:05:19
* 修改记录：
*/

using System;
using Entt.Entities.Helpers;

namespace Entt.Entities.Pools
{
    /// <summary>
    /// Entity和Component关联在一起的对象池
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    public class Pool<TEntityKey, TComponent> : SparseDictionary<TEntityKey, TComponent>, IPool<TEntityKey, TComponent>
        where TEntityKey : IEntityKey
    {
        public event EventHandler<TEntityKey>? Created;
        public event EventHandler<TEntityKey>? Destroyed;
        public event EventHandler<(TEntityKey key, TComponent old)>? DestroyedNotify;
        public event EventHandler<(TEntityKey key, TComponent old)>? Updated;

        internal Pool()
        {
            
        }
        
        public override void Add(TEntityKey entityKey, in TComponent component)
        {
            base.Add(entityKey, in component);
            Created?.Invoke(this, entityKey);
        }
        
        public override bool Remove(TEntityKey entityKey)
        {
            if (Destroyed == null && DestroyedNotify == null)
            {
                return base.Remove(entityKey);
            }

            if (DestroyedNotify == null)
            {
                if (base.Remove(entityKey))
                {
                    Destroyed?.Invoke(this, entityKey);
                    return true;
                }
            }
            else if (TryGet(entityKey, out var old) && base.Remove(entityKey))
            {
                DestroyedNotify?.Invoke(this, (entityKey, old));
                Destroyed?.Invoke(this, entityKey);
                return true;
            }

            return false;
        }
        
        public override void RemoveAll()
        {
            if (Destroyed == null && DestroyedNotify == null)
            {
                base.RemoveAll();
                return;
            }

            while (Count > 0)
            {
                var entityKey = Last;
                Remove(entityKey);
            }
        }
        
        public override bool WriteBack(TEntityKey entityKey, in TComponent component)
        {
            if (Updated == null)
            {
                return base.WriteBack(entityKey, in component);
            }

            if (TryGet(entityKey, out var c))
            {
                if (base.WriteBack(entityKey, in component))
                {
                    Updated?.Invoke(this, (entityKey, c));
                    return true;
                }
            }

            return false;
        }
    }
}