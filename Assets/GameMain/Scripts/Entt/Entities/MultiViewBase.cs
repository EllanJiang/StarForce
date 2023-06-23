/*
  作者：LTH
  文件描述：
  文件名：MultiViewBase
  创建时间：2023/06/24 00:06:SS
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Entt.Entities.Helpers;
using Entt.Entities.Pools;

namespace Entt.Entities
{
    /// <summary>
    /// 用于获取并显示多个entity
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    /// <typeparam name="TEnumerator"></typeparam>
    public abstract class MultiViewBase<TEntityKey, TEnumerator> : IEntityView<TEntityKey> 
        where TEnumerator : IEnumerator<TEntityKey>
        where TEntityKey: IEntityKey
    {
        readonly EventHandler<TEntityKey> onCreated;
        readonly EventHandler<TEntityKey> onDestroyed;
        protected readonly List<IReadOnlyPool<TEntityKey>> Sets;

        /// <summary>
        ///   Use this as a general fallback during the construction of
        ///   subclasses, where it may not yet be safe to use the overloaded
        ///   'Contains' method.
        /// </summary>
        protected readonly Func<TEntityKey, bool> IsMemberPredicate;

        public bool AllowParallelExecution { get; set; }

        protected MultiViewBase(IEntityPoolAccess<TEntityKey> registry,
                                IReadOnlyList<IReadOnlyPool<TEntityKey>> entries)
        {
            this.Registry = registry ?? throw new ArgumentNullException(nameof(registry));
            if (entries == null || entries.Count == 0)
            {
                throw new ArgumentException();
            }

            onCreated = OnCreated;
            onDestroyed = OnDestroyed;
            this.Sets = new List<IReadOnlyPool<TEntityKey>>(entries);
            foreach (var pool in Sets)
            {
                pool.Destroyed += onDestroyed;
                pool.Created += onCreated;
            }

            IsMemberPredicate = IsMember;
        }

        ~MultiViewBase()
        {
            Disposing(false);
        }

        public event EventHandler<TEntityKey>? Destroyed;
        public event EventHandler<TEntityKey>? Created;

        protected virtual void OnCreated(object sender, TEntityKey e)
        {
            if (Contains(e))
            {
                Created?.Invoke(sender, e);
            }
        }

        protected virtual void OnDestroyed(object sender, TEntityKey e)
        {
            var countContained = 0;
            foreach (var pool in Sets)
            {
                if (pool.Contains(e))
                {
                    countContained += 1;
                }
            }

            if (countContained == Sets.Count - 1)
            {
                Destroyed?.Invoke(sender, e);
            }
        }

        public void Reserve(int capacity)
        {
            foreach (var pool in Sets)
            {
                pool.Reserve(capacity);
            }
        }

        protected IEntityPoolAccess<TEntityKey> Registry { get; }

        public void RemoveComponent<TComponent>(TEntityKey entity)
        {
            Registry.RemoveComponent<TComponent>(entity);
        }

        public bool ReplaceComponent<TComponent>(TEntityKey entity, in TComponent c)
        {
            return Registry.ReplaceComponent(entity, in c);
        }

        public virtual void Respect<TComponent>()
        {
            // adhoc views ignore the respect command. 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<TEntityKey> IEnumerable<TEntityKey>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract TEnumerator GetEnumerator();

        public abstract int EstimatedSize { get; }

        protected bool IsMember(TEntityKey e)
        {
            foreach (var set in Sets)
            {
                if (!set.Contains(e))
                {
                    return false;
                }
            }

            return true;
        }

        public void Reset(TEntityKey entity)
        {
            Registry.Reset(entity);
        }

        public virtual bool Contains(TEntityKey e)
        {
            return IsMember(e);
        }

        public bool IsValid(TEntityKey entity)
        {
            return Registry.IsValid(entity);
        }

        public bool IsOrphan(TEntityKey entity)
        {
            return Registry.IsOrphan(entity);
        }

        public void Apply(ViewDelegates.Apply<TEntityKey> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this);
            try
            {
                foreach (var e in p)
                {
                    bulk(this, e);
                }
            }
            finally
            {
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext c, ViewDelegates.ApplyWithContext<TEntityKey, TContext> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this);
            try
            {
                foreach (var e in p)
                {
                    bulk(this, c, e);
                }
            }
            finally
            {
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        public bool GetComponent<TComponent>(TEntityKey entity, [MaybeNullWhen(false)] out TComponent data)
        {
            return Registry.GetComponent(entity, out data);
        }

        public void WriteBack<TComponent>(TEntityKey entity, in TComponent data)
        {
            Registry.WriteBack(entity, in data);
        }

        public void AssignOrReplace<TComponent>(TEntityKey entity)
        {
            Registry.AssignOrReplace<TComponent>(entity);
        }

        public void AssignOrReplace<TComponent>(TEntityKey entity, TComponent c)
        {
            Registry.AssignOrReplace(entity, c);
        }

        public void AssignOrReplace<TComponent>(TEntityKey entity, in TComponent c)
        {
            Registry.AssignOrReplace(entity, in c);
        }

        public bool HasTag<TTag>()
        {
            return Registry.HasTag<TTag>();
        }

        public void AttachTag<TTag>(TEntityKey entity)
        {
            Registry.AttachTag<TTag>(entity);
        }

        public void AttachTag<TTag>(TEntityKey entity, in TTag tag)
        {
            Registry.AttachTag(entity, in tag);
        }

        public void RemoveTag<TTag>()
        {
            Registry.RemoveTag<TTag>();
        }

        public bool TryGetTag<TTag>([MaybeNullWhen(false)] out TEntityKey k, [MaybeNullWhen(false)] out TTag tag)
        {
            return Registry.TryGetTag(out k, out tag);
        }

        public bool HasComponent<TOtherComponent>(TEntityKey entity)
        {
            return Registry.HasComponent<TOtherComponent>(entity);
        }

        public TOtherComponent AssignComponent<TOtherComponent>(TEntityKey entity)
        {
            return Registry.AssignComponent<TOtherComponent>(entity);
        }

        public void AssignComponent<TOtherComponent>(TEntityKey entity, in TOtherComponent c)
        {
            Registry.AssignComponent(entity, in c);
        }

        TOtherComponent IEntityViewControl<TEntityKey>.AssignOrReplace<TOtherComponent>(TEntityKey entity)
        {
            return Registry.AssignOrReplace<TOtherComponent>(entity);
        }

        public virtual void CopyTo(RawList<TEntityKey> k)
        {
            k.Clear();
            k.Capacity = Math.Max(k.Capacity, EstimatedSize);

            foreach (var e in this)
            {
                k.Add(e);
            }
        }

        public void Dispose()
        {
            Disposing(true);
            GC.SuppressFinalize(this);
        }

        protected bool Disposed { get; private set; }

        protected virtual void Disposing(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            Disposed = true;
            foreach (var pool in Sets)
            {
                pool.Destroyed -= onDestroyed;
                pool.Created -= onCreated;
            }
        }
        
        protected static IReadOnlyPool<TEntityKey> FindMinimumEntrySet(IReadOnlyList<IReadOnlyPool<TEntityKey>> sets)
        {
            IReadOnlyPool<TEntityKey>? s = null;
            var count = int.MaxValue;
            foreach (var set in sets)
            {
                if (set.Count < count)
                {
                    s = set;
                    count = s.Count;
                }
            }

            if (s == null)
            {
                throw new ArgumentException();
            }

            return s;
        }
    }
}