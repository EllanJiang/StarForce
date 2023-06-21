/*
* 文件名：EntityRegistry
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/21 20:09:47
* 修改记录：
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Entt.Entities.Pools;

namespace Entt.Entities
{
    public class EntityRegistry<TEntityKey> : IEntityPoolAccess<TEntityKey> where TEntityKey: IEntityKey
    {
        public bool HasTag<TTag>()
        {
            throw new NotImplementedException();
        }

        public bool TryGetTag<TTag>(out TEntityKey k, out TTag tag)
        {
            throw new NotImplementedException();
        }

        public void RemoveTag<TTag>()
        {
            throw new NotImplementedException();
        }

        public void AttachTag<TTag>(TEntityKey entity)
        {
            throw new NotImplementedException();
        }

        public void AttachTag<TTag>(TEntityKey entity, in TTag tag)
        {
            throw new NotImplementedException();
        }

        public bool Contains(TEntityKey entityKey)
        {
            throw new NotImplementedException();
        }

        public bool GetComponent<TComponent>(TEntityKey entity, out TComponent data)
        {
            throw new NotImplementedException();
        }

        public bool HasComponent<TComponent>(TEntityKey entity)
        {
            throw new NotImplementedException();
        }

        public void WriteBack<TComponent>(TEntityKey entity, in TComponent data)
        {
            throw new NotImplementedException();
        }

        public void RemoveComponent<TComponent>(TEntityKey entity)
        {
            throw new NotImplementedException();
        }

        public TComponent AssignComponent<TComponent>(TEntityKey entity)
        {
            throw new NotImplementedException();
        }

        public void AssignComponent<TComponent>(TEntityKey entity, in TComponent c)
        {
            throw new NotImplementedException();
        }

        public TComponent AssignOrReplace<TComponent>(TEntityKey entity)
        {
            throw new NotImplementedException();
        }

        public void AssignOrReplace<TComponent>(TEntityKey entity, in TComponent c)
        {
            throw new NotImplementedException();
        }

        public bool ReplaceComponent<TComponent>(TEntityKey entity, in TComponent c)
        {
            throw new NotImplementedException();
        }

        public bool IsValid(TEntityKey entity)
        {
            throw new NotImplementedException();
        }

        public void Reset(TEntityKey entity)
        {
            throw new NotImplementedException();
        }

        public bool IsOrphan(TEntityKey entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TEntityKey> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count { get; }
        public TEntityKey Create()
        {
            throw new NotImplementedException();
        }

        public IPool<TEntityKey, TComponent> GetWritablePool<TComponent>()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyPool<TEntityKey, TComponent> GetPool<TComponent>()
        {
            throw new NotImplementedException();
        }

        public bool TryGetPool<TComponent>(out IReadOnlyPool<TEntityKey, TComponent> pool)
        {
            throw new NotImplementedException();
        }

        public bool TryGetWritablePool<TComponent>(out IPool<TEntityKey, TComponent> pool)
        {
            throw new NotImplementedException();
        }

        public void AssureEntityState(TEntityKey entity, bool destroyed)
        {
            throw new NotImplementedException();
        }

        public void Destroy(TEntityKey k)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(List<TEntityKey> k)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<TEntityKey>? BeforeEntityDestroyed;
    }
}