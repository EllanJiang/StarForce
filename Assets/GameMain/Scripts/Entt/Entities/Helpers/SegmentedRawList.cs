/*
* 文件名：SegmentedRawList
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/20 16:42:30
* 修改记录：
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Entt.Entities.Helpers
{
    /// <summary>
    /// 分段列表
    /// 每一段的大小是通过传入的segmentSize来决定的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SegmentedRawList<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// raw在[0-4]区间内,返回2
        /// raw在[5-8]区间内,返回3
        /// raw在[9-16]区间内,返回4
        /// raw在[17-32]区间内,返回5
        /// 以此类推
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        static int SegmentBits(int raw)
        {
            var exp = (int) Math.Ceiling(Math.Log(raw) / Math.Log(2));
            exp = Math.Min(Math.Max(2, exp), 31);
            return exp;
        }

        private static readonly T[][] empty = Array.Empty<T[]>();
        private readonly int segmentBitShift;
        private readonly int segmentMask;
        private readonly int segmentSize;
        
        private T[]?[] data;
        private int version;

        public SegmentedRawList(int segmentSize = 256, int initSize = 10)
        {
            this.segmentBitShift = SegmentBits(segmentSize);
            this.segmentSize = (1 << segmentBitShift);
            this.segmentMask = segmentSize - 1;

            data = empty;
            EnsureCapacity(initSize);
        }
        
        public int Capacity
        {
            get
            {
                return data.Length * segmentSize;
            }
        }

        public T[]?[] UnsafeData => data;
        
        public void StoreAt(int index, in T input)
        {
            EnsureCapacity(index + 1);
            //假设segmentSize=256，则segmentBitShift=8,segmentMask=0xff
            //index在[0,255]区间内时，segmentIdx=0
            //index在[256,511]区间内时，segmentIdx=1 以此类推
            var segmentIdx = index >> segmentBitShift;
            var segment = this.data[segmentIdx];
            if (segment == null)
            {
                segment = new T[segmentSize];
                this.data[segmentIdx] = segment;
            }

            var dataIdx = index & segmentMask;
            segment[dataIdx] = input;
            this.Count = Math.Max(index + 1, Count);
            this.version += 1;
        }
        
        /// <summary>
        /// 跟StoreAt的位移区别是，如果index不在[0,Count)区间内，那么不用扩容，而是直接报错
        /// </summary>
        /// <param name="index"></param>
        /// <param name="input"></param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        void StoreRaw(int index, in T input)
        {
            if (index >= Count || index < 0) throw new IndexOutOfRangeException($"Index {index} is not within the valid range [0, {Count})");
            
            var segmentIdx = index >> segmentBitShift;
            var segment = this.data[segmentIdx];
            if (segment == null)
            {
                segment = new T[segmentSize];
                this.data[segmentIdx] = segment;
            }

            var dataIdx = index & segmentMask;
            segment[dataIdx] = input;
            this.version += 1;
        }
        
        public int Version
        {
            get { return version; }
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

        public ref T? TryGetRef(int index, ref T? defaultValue, out bool success)
        {
            if (index < 0 || index >= Count)
            {
                success = false;
                return ref defaultValue;
            }

            var segmentIdx = index >> segmentBitShift;
            var segment = this.data[segmentIdx];
            if (segment == null)
            {
                success = false;
                return ref defaultValue;
            }
            
            var dataIdx = index & segmentMask;
            success = true;
            return ref segment[dataIdx]!;
        }
        
        public int Count
        {
            get;
            private set;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, $"Index {index} must be positive and less than {Count}");
                }
                
                var segmentIdx = index >> segmentBitShift;
                var segment = this.data[segmentIdx];
                if (segment == null)
                {
                    return default!;
                }
                
                var dataIdx = index & segmentMask;
                return segment[dataIdx];
            }
            set
            {
                StoreRaw(index, in value);
            }
        }

        public void Swap(int src, int dst)
        {
            T? defaultVal = default;
            ref var srcRef = ref TryGetRef(src, ref defaultVal, out var success1);
            ref var destRef = ref TryGetRef(dst, ref defaultVal, out var success2);
            if (!success1 || !success2)
            {
                (this[src], this[dst]) = (this[dst], this[src]);
            }
            else
            {
                (srcRef, destRef) = (destRef, srcRef);
            }

            this.version += 1;
        }
        
        public void Clear()
        {
            if (Count > 0)
            {
                Array.Clear(data, 0, Count);
                Count = 0;
            }

            this.version += 1;
        }

        public bool TryGet(int index, out T output)
        {
            T? defaultVal = default!;
            ref var retval = ref TryGetRef(index, ref defaultVal, out var success);
            if (success)
            {
                output = retval!;
                return true;
            }

            output = default!;
            return false;
        }
        
        //内联函数
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void EnsureCapacity(int minIndexNeeded)
        {
            if (minIndexNeeded < Capacity)
            {
                return;
            }

            var capacityDynamic = Math.Min(minIndexNeeded + 10000, Capacity * 150 / 100);
            var capacityStatic = Count + 500;
            
            var capacity = Math.Max(capacityStatic, capacityDynamic);
            var segmentsNeeded = (int) Math.Ceiling(capacity / (float) segmentSize);
            Array.Resize(ref data, segmentsNeeded);
        }
        
        public void Add(in T e)
        {
            EnsureCapacity(Count + 1);

            this.StoreAt(Count, e);
            this.version += 1;
        }

        public void RemoveLast()
        {
            if (Count == 0) throw new ArgumentOutOfRangeException();
            
            this[Count - 1] = default!;
            this.Count -= 1;
            this.version += 1;
        }
        
        public struct Enumerator : IEnumerator<T>
        {
            readonly SegmentedRawList<T> contents;
            readonly int versionAtStart;
            int index;
            T current;

            internal Enumerator(SegmentedRawList<T> content) : this()
            {
                this.contents = content;
                this.versionAtStart = content.version;
                index = -1;
                current = default!;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (versionAtStart != contents.version)
                {
                    throw new InvalidOperationException("Concurrent Modification of RawList while iterating.");
                }
                
                if (index + 1 < contents.Count)
                {
                    index += 1;
                    current = contents[index];
                    return true;
                }

                current = default!;
                return false;
            }

            public void Reset()
            {
                index = -1;
                current = default!;
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
        }
    }
}