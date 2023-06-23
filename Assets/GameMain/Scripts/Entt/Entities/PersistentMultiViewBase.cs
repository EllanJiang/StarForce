/*
  作者：LTH
  文件描述：
  文件名：PersistentMultiViewBase
  创建时间：2023/06/24 00:06:SS
*/

using System;
using Entt.Entities.Helpers;
using Entt.Entities.Pools;

namespace Entt.Entities
{
   public abstract class PersistentMultiViewBase<TEntityKey> : MultiViewBase<TEntityKey, PredicateEnumerator<TEntityKey>> 
        where TEntityKey : IEntityKey
    {
        readonly SparseSet<TEntityKey> view;
        protected readonly Func<TEntityKey, bool> FastIsMemberPredicate;

        protected PersistentMultiViewBase(IEntityPoolAccess<TEntityKey> registry,
                                          params IReadOnlyPool<TEntityKey>[] entries) : base(registry, entries)
        {
            view = CreateInitialEntrySet(entries);
            FastIsMemberPredicate = view.Contains;
        }

        public override int EstimatedSize => view.Count;

        protected override void OnDestroyed(object sender, TEntityKey e)
        {
            base.OnDestroyed(sender, e);
            view.Remove(e);
        }

        protected override void OnCreated(object sender, TEntityKey e)
        {
            if (IsMember(e) && !view.Contains(e))
            {
                view.Add(e);
            }

            base.OnCreated(sender, e);
        }

        public int Count => view.Count;

        public override bool Contains(TEntityKey e)
        {
            return view.Contains(e);
        }

        public override PredicateEnumerator<TEntityKey> GetEnumerator()
        {
            return new PredicateEnumerator<TEntityKey>(view, FastIsMemberPredicate);
        }

        public override void Respect<TComponent>()
        {
            view.Respect(Registry.GetPool<TComponent>());
        }

        public override void CopyTo(RawList<TEntityKey> k)
        {
            view.CopyTo(k);
        }

        SparseSet<TEntityKey> CreateInitialEntrySet(IReadOnlyPool<TEntityKey>[] sets)
        {
            var s = FindMinimumEntrySet(sets);
            var result = new SparseSet<TEntityKey>();
            
            var p = EntityKeyListPool.Reserve(s);
            try
            {
                foreach (var e in p)
                {
                    if (IsMember(e))
                    {
                        result.Add(e);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }

            return result;
        }

    }
}