/*
* 文件名：RawList
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/20 15:18:22
* 修改记录：
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Entt.Entities.Helpers
{
    /// <summary>
    /// 使用数组来自定义列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RawList<T>:IReadOnlyList<T>
    {
        /// <summary>
        /// 使用数组来实现List
        /// </summary>
        private T[] data;
        /// <summary>
        /// 当前列表所处的版本，用来判断当前列表是否有被修改，如果被修改（存储，扩充容量,在列表末端添加/删除一个元素等），version++
        /// </summary>
        private int version;

        public RawList(int initSize = 10)
        {
            data = new T[Math.Max(0, initSize)];
        }

        /// <summary>
        /// 列表容量
        /// </summary>
        public int Capacity
        {
            get
            {
                return data.Length;
            }
            set
            {
                if (data.Length != value)
                {
                    Array.Resize(ref data,value);
                    AddVersion();
                }
            }
        }

        /// <summary>
        /// 存储在指定索引处
        /// </summary>
        /// <param name="index"></param>
        /// <param name="input"></param>
        public void StoreAt(int index, in T input)
        {
            EnsureCapacity(index + 1);
            data[index] = input;
            Count = Math.Max(index + 1, Count);
            AddVersion();
        }
        
        /// <summary>
        /// 获取列表所处版本 
        /// </summary>
        public int Version
        {
            get { return version; }
        }
        
        /// <summary>
        /// 版本号+1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddVersion()
        {
            version++;
        }

        /// <summary>
        /// 获取原始数组
        /// </summary>
        public T[] UnsafeData => data;

        /// <summary>
        /// 直接指定原始数组中元素数量
        /// </summary>
        /// <param name="c"></param>
        public void UnsafeSetCount(int c) => Count = c;
        
       

        /// <summary>
        /// 尝试获取指定索引对应的值的引用
        /// </summary>
        /// <param name="index"></param>
        /// <param name="defaultValue">如果没有找到，那么返回该默认值</param>
        /// <param name="success"></param>
        /// <returns></returns>
        public ref T? TryGetRef(int index, ref T? defaultValue, out bool success)
        {
            if (index < 0 || index >= Count)
            {
                success = false;
                return ref defaultValue;
            }

            success = true;
            return ref data[index]!;
        }
        
        /// <summary>
        /// 获取列表中元素总数
        /// </summary>
        public int Count { get;private set; }

        /// <summary>
        /// 通过下标获取对应元素
        /// </summary>
        /// <param name="index"></param>
        public T this[int index]
        {
            get { return data[index]; }
            set
            {
                StoreAt(index,in  value);
            }
        }

        /// <summary>
        /// 互换src和dst索引处的值
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public void Swap(int src, int dst)
        {
            (data[src], data[dst]) = (data[dst], data[src]);
            AddVersion();
        }

        public void Clear()
        {
            if (Count > 0)
            {
                Array.Clear(data,0,Count);
                Count = 0;
            }

            AddVersion();
        }

        /// <summary>
        /// 获取指定索引处的值的引用
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ref T ReferenceOf(int index)
        {
            EnsureCapacity(index + 1);
            return ref data[index];
        }

        /// <summary>
        /// 尝试获取指定索引处的值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool TryGet(int index,[MaybeNullWhen(false)] out T output)
        {
            if (index >= 0 && index < Count)
            {
                output = data[index];
                return true;
            }

            output = default;
            return false;
        }

        /// <summary>
        /// 确保数组容量足够
        /// 该方法使用内联方式实现
        /// </summary>
        /// <param name="minIndexNeeded"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void EnsureCapacity(int minIndexNeeded)
        {
            if (minIndexNeeded < Capacity)
            {
                return;
            }

            var capacityDynamic = Math.Min(minIndexNeeded + 10000, Capacity * 150 / 100);
            var CapacityStatic = Count + 500;
            Capacity = Math.Max(capacityDynamic, CapacityStatic);
        }

        /// <summary>
        /// 在列表末端添加一个元素
        /// </summary>
        /// <param name="element"></param>
        public void Add(in T element)
        {
            EnsureCapacity(Count + 1);
            data[Count] = element;
            Count++;
            AddVersion();
        }

        /// <summary>
        /// 删除列表末端的元素
        /// </summary>
        public void RemoveLast()
        {
            data[Count - 1] = default!;
            Count--;
            AddVersion();
        }
        
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
         
        /// <summary>
        /// 列表内部的迭代器
        /// </summary>
        public struct Enumerator:IEnumerator<T>
        {
            private readonly RawList<T> contents;
            private readonly int versionAtStart;
            private int index;
            private T current;

            internal Enumerator(RawList<T> content) : this()
            {
                contents = content;
                versionAtStart = content.version;
                index = -1;
                //default后面加一个感叹号是因为，C#编译器开了nullable开关后，直接给成员对象赋值default(T)会被警告（可能存在错误），加了感叹号就是告诉（欺骗）编译器这个不是null，以去除这个警告
                current = default!;  
            }

            public bool MoveNext()
            {
                if (versionAtStart != contents.version)
                {
                    throw new InvalidOperationException("不能在遍历RawList的同时修改RawList！");
                }

                if (index + 1 < contents.Count)
                {
                    index++;
                    current = contents[index];
                    return true;
                }

                current = default!;
                return false;
            }

           
            object IEnumerator.Current => Current!;
            
            public T Current
            {
                get
                {
                    if (index < 0 || index >= contents.Count)
                    {
                        throw new InvalidOperationException();
                    }

                    return current;
                }
            }
            
            public void Reset()
            {
                index = -1;
                current = default!;
            }
            
            public void Dispose()
            {
                
            }
        }

    }
}