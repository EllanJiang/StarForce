/*
* 文件名：LiteRingBuffer
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 10:55:27
* 修改记录：
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace LogicShared.LiteNetLib.Helpers
{
    /// <summary>
    /// 环形缓存
    /// </summary>
    public class LiteRingBuffer<T> : IEnumerable<T>
    {
        private readonly T[] _elements;
        private int _start;
        private int _end;
        private int _count;
        private readonly int _capacity;
        
        public T this[int i] => _elements[(_start + i) % _capacity];

        public LiteRingBuffer(int count)
        {
            _elements = new T[count];
            _capacity = count;
        }

        public void Add(T element)
        {
            if(_count == _capacity)
                throw new ArgumentException();
            _elements[_end] = element;
            _end = (_end + 1) % _capacity;
            _count++;
        }

        public void FastClear()
        {
            _start = 0;
            _end = 0;
            _count = 0;
        }

        public int Count => _count;
        public T First => _elements[_start];
        public T Last => _elements[(_start+_count-1)%_capacity]; //环形buffer 最重要的一点就是last的计算方式 lastIndex=(_start+_count-1)%_capacity
        public bool IsFull => _count == _capacity;

        //从开头移除count个元素
        public void RemoveFromStart(int count)
        {
            if(count > _capacity || count > _count)
                throw new ArgumentException();
            _start = (_start + count) % _capacity;
            _count -= count;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int counter = _start;
            while (counter != _end)
            {
                yield return _elements[counter];
                counter = (counter + 1) % _capacity;
            }           
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}