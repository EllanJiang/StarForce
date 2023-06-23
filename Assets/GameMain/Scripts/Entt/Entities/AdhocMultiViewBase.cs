/*
  作者：LTH
  文件描述：
  文件名：AdhocMultiViewBase
  创建时间：2023/06/24 00:06:SS
*/

using System;
using Entt.Entities.Helpers;
using Entt.Entities.Pools;

namespace Entt.Entities
{
    public abstract class AdhocMultiViewBase<TEntityKey> : MultiViewBase<TEntityKey, PredicateEnumerator<TEntityKey>> 
        where TEntityKey : IEntityKey
    {
        protected AdhocMultiViewBase(IEntityPoolAccess<TEntityKey> registry,
            params IReadOnlyPool<TEntityKey>[] entries) : base(registry, entries)
        {
        }

        public override int EstimatedSize
        {
            get
            {
                if (Sets.Count == 0)
                {
                    return 0;
                }

                var count = int.MaxValue;
                for (var index = 0; index < Sets.Count; index++)
                {
                    var set = Sets[index];
                    if (set.Count < count)
                    {
                        count = set.Count;
                    }
                }

                return count;
            }
        }

        public override void CopyTo(RawList<TEntityKey> k)
        {
            k.Clear();
            k.Capacity = Math.Max(k.Capacity, EstimatedSize);

            var s = FindMinimumEntrySet(Sets);
            s.CopyTo(k);
        }

        public override PredicateEnumerator<TEntityKey> GetEnumerator()
        {
            IReadOnlyPool<TEntityKey>? s = null;
            var count = int.MaxValue;
            for (var index = 0; index < Sets.Count; index++)
            {
                var set = Sets[index];
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

            return new PredicateEnumerator<TEntityKey>(s, IsMemberPredicate);
        }
    }
}