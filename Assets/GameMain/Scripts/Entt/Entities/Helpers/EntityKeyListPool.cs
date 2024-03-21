/*
* 文件名：EntityKeyListPool
* 文件描述：EntityKey列表对象池
* 作者：aronliang
* 创建时间：2023/06/21 17:03:42
* 修改记录：
*/

using System;
using System.Collections.Concurrent;
using Entt.Entities.Pools;

namespace Entt.Entities.Helpers
{
    /// <summary>
    /// EntityKey列表对象池
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public static class EntityKeyListPool<TEntityKey> where TEntityKey:IEntityKey
    {
        private static readonly ConcurrentStack<RawList<TEntityKey>> Pools;

        static EntityKeyListPool()
        {
            Pools = new ConcurrentStack<RawList<TEntityKey>>();
        }

        /// <summary>
        /// 从列表对象池中返回可用的RawList，并用src来填充该RawList
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static RawList<TEntityKey> Reserve(IEntityPoolAccess<TEntityKey> src)
        {
            if (!Pools.TryPop(out var result))
            {
                result = new RawList<TEntityKey>();
            }
            else
            {
                result.Clear();
                result.Capacity = Math.Max(result.Capacity, src.Count);
            }

            foreach (var entityKey in src)
            {
                result.Add(entityKey);
            }

            return result;
        }

        /// <summary>
        ///  从列表对象池中返回可用的RawList，并用src来填充该RawList
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static RawList<TEntityKey> Reserve(IEntityView<TEntityKey> src)
        {
            if (!Pools.TryPop(out var result))
            {
                result = new RawList<TEntityKey>(src.EstimatedSize);
            }
            else
            {
                result.Clear();
                result.Capacity = Math.Max(result.Capacity, src.EstimatedSize);
            }

            src.CopyTo(result);
            return result;
        }
        
        /// <summary>
        /// 从列表对象池中返回可用的RawList，并用src来填充该RawList
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static RawList<TEntityKey> Reserve(IReadOnlyPool<TEntityKey> src) 
        {
            if (!Pools.TryPop(out var result))
            {
                result = new RawList<TEntityKey>(src.Count);
            }
            else
            {
                result.Clear();
                result.Capacity = Math.Max(result.Capacity, src.Count);
            }

            src.CopyTo(result);
            return result;
        }
        
        /// <summary>
        /// 把使用完的RawList放回对象池
        /// </summary>
        /// <param name="list"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Release(RawList<TEntityKey> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));

            list.Clear();
            Pools.Push(list);
        }
    }


    public static class EntityKeyListPool
    {
        /// <summary>
        /// 从列表对象池中返回可用的RawList，并用src来填充该RawList
        /// </summary>
        /// <param name="src"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public static RawList<TEntityKey> Reserve<TEntityKey>(IEntityPoolAccess<TEntityKey> src) where TEntityKey : IEntityKey
        {
            return EntityKeyListPool<TEntityKey>.Reserve(src);
        }

        /// <summary>
        /// 从列表对象池中返回可用的RawList，并用src来填充该RawList
        /// </summary>
        /// <param name="src"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public static RawList<TEntityKey> Reserve<TEntityKey>(IEntityView<TEntityKey> src) where TEntityKey : IEntityKey
        {
            return EntityKeyListPool<TEntityKey>.Reserve(src);
        }

        /// <summary>
        /// 从列表对象池中返回可用的RawList，并用src来填充该RawList
        /// </summary>
        /// <param name="src"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public static RawList<TEntityKey> Reserve<TEntityKey>(IReadOnlyPool<TEntityKey> src) where TEntityKey : IEntityKey
        {
            return EntityKeyListPool<TEntityKey>.Reserve(src);
        }

        /// <summary>
        /// 把使用完的RawList放回对象池
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        public static void Release<TEntityKey>(RawList<TEntityKey> list) where TEntityKey : IEntityKey
        {
            EntityKeyListPool<TEntityKey>.Release(list);
        }
    }
}