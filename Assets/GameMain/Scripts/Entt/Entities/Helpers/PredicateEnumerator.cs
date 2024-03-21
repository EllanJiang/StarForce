/*
* 文件名：PredicateEnumerator
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/21 16:52:22
* 修改记录：
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Entt.Entities.Helpers
{
    /// <summary>
    /// 自定义迭代器，与常规迭代器的主要区别是在MoveNext()中能否继续向下一个元素迭代是通过传入的predicate()方法来判断的
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public struct PredicateEnumerator<TValue>:IEnumerator<TValue>
    {
        private readonly IEnumerable<TValue> contents;
        private Func<TValue, bool> predicate;
        private IEnumerator<TValue> enumerator;

        /// <summary>
        /// 通过调用predicate()来判断能否MoveNext
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="predicate"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal PredicateEnumerator(IEnumerable<TValue> contents,
            Func<TValue, bool> predicate) : this()
        {
            this.contents = contents ?? throw new ArgumentNullException();
            this.predicate = predicate;
            this.enumerator = contents.GetEnumerator();
        }

        public TValue Current
        {
            get
            {
                return enumerator.Current;
            }
        }
        object IEnumerator.Current => Current!;

        public bool MoveNext()
        {
            while (enumerator.MoveNext())
            {
                var entity = enumerator.Current;
                if (predicate(entity))
                {
                    return true;
                }
            }

            return false;
        }
        
        public void Reset()
        {
            enumerator = contents.GetEnumerator();
        }

        public void Dispose()
        {
            
        }
    }

    /// <summary>
    /// 与常规迭代器的主要区别是Current的返回值是通过传入的Mapper方法来返回的
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TEnumerator"></typeparam>
    public struct MappingEnumerator<TValue, TSource, TEnumerator> : IEnumerator<TValue>
        where TEnumerator : IEnumerator<TSource>
    {
        
        // Making this field readonly breaks the enumerator as it seem to prevent state updates in the parent instance.
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private TEnumerator parent;
        private readonly Func<TSource, TValue> mapper;
        
        public MappingEnumerator(TEnumerator parent, Func<TSource, TValue> mapper)
        {
            this.parent = parent;
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public bool MoveNext()
        {
            return parent.MoveNext();
        }

        public void Reset()
        {
            parent.Reset();
        }


        public TValue Current
        {
            get
            {
                var current = parent.Current;
                return mapper(current);
            }
        }

        object IEnumerator.Current => Current!;

        public void Dispose()
        {
            parent.Dispose();
        }
    }
}