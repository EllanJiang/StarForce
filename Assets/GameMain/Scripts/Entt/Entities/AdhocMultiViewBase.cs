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
    /// <summary>
    /// 用来临时显示多个Entity
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public abstract class AdhocMultiViewBase<TEntityKey> : MultiViewBase<TEntityKey, PredicateEnumerator<TEntityKey>> 
        where TEntityKey : IEntityKey
    {
        protected AdhocMultiViewBase(IEntityPoolAccess<TEntityKey> registry,
            params IReadOnlyPool<TEntityKey>[] entries) : base(registry, entries)
        {
        }

        /// <summary>
        /// 预估大小，返回当前集合中Count最小的Pool的Count
        /// </summary>
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

        /// <summary>
        /// 把当前集合中数量最小的pool复制到k中
        /// </summary>
        /// <param name="k"></param>
        public override void CopyTo(RawList<TEntityKey> k)
        {
            k.Clear();
            k.Capacity = Math.Max(k.Capacity, EstimatedSize);

            var s = FindMinimumEntrySet(Sets);
            s.CopyTo(k);
        }

        /// <summary>
        /// 使用当前集合中Count最小的Pool来迭代
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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