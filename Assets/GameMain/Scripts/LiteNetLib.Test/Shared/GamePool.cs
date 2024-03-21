/*
* 文件名：GamePool
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:02:06
* 修改记录：
*/

using System;

namespace LiteNetLib.Test.Shared
{
    /// <summary>
    /// 非常简单的对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GamePool<T> where T : class
    {
        private readonly T[] _pool;
        private readonly Func<T> _creator;
        private int _count;

        public GamePool(Func<T> creator) : this(creator, 8)
        {

        }

        public GamePool(Func<T> creator, int capacity)
        {
            _pool = new T[capacity];
            _creator = creator;
        }

        public T Get()
        {
            if (_count > 0)
            {
                _count--;
                var result = _pool[_count];
                _pool[_count] = default;
                return result;
            }
            return _creator();
        }

        public void Put(T gameObject)
        {
            _pool[_count] = gameObject;
            _count++;
        }
    }
}