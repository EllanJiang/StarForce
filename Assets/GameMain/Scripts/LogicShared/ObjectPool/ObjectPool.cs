/*
* 文件名：ObjectPool
* 文件描述：
* 作者：aronliang
* 创建时间：2023/08/07 14:46:50
* 修改记录：
*/

using System;
using System.Collections.Concurrent;


namespace LogicShared
{
    public class ObjectPool
    {
        private static ConcurrentDictionary<Type, ConcurrentQueue<IObjectPool>> PoolDict =
            new ConcurrentDictionary<Type, ConcurrentQueue<IObjectPool>>();

        /// <summary>
        /// 放回对象池
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        public static void PutBackPool<T>(T obj) where T : IObjectPool
        {
            if (obj == null)
            {
                return;
            }
            
            obj.PutBackPool();
            ConcurrentQueue<IObjectPool> pool = GetOrCreatePool<T>();
            pool.Enqueue(obj);
        }
        
        /// <summary>
        /// 从对象池中取出一个可用的对象
        /// </summary>
        /// <param name="createIfNotInPool">如果对象池中没有，就创建(new)新的对象</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetFromPool<T>(bool createIfNotInPool = true) where T : IObjectPool, new()
        {
            var pool = GetPool<T>();
            if (pool != null && pool.Count > 0)
            {
                if (pool.TryDequeue(out var obj))
                {
                    return (T)obj;
                }
            }

            if (createIfNotInPool)
            {
                return new T();
            }

            return default(T);
        }
        
        /// <summary>
        /// 清空所有对象池
        /// </summary>
        public static void ClearAllPool()
        {
            PoolDict.Clear();
        }
        
        /// <summary>
        /// 清空指定对象池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void ClearPool<T>() where T : IObjectPool
        {
            var key = typeof(T);
            if (PoolDict.ContainsKey(key))
            {
                PoolDict.TryRemove(key, out var _);
            }
        }
        
        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static ConcurrentQueue<IObjectPool> GetPool<T>() where T : IObjectPool
        {
            Type type = typeof(T);
            if (PoolDict.TryGetValue(type, out var pool))
            {
                return pool;
            }
            
            return pool;
        }
        
        /// <summary>
        /// 获取对象池，如果没有，则创建新的对象池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static ConcurrentQueue<IObjectPool> GetOrCreatePool<T>() where T : IObjectPool
        {
            Type type = typeof(T);
            if (PoolDict.TryGetValue(type, out var pool))
            {
                return pool;
            }

            pool = new ConcurrentQueue<IObjectPool>();
            PoolDict[type] = pool;
            
            return pool;
        }
    }
}