/*
* 文件名：PredicateEnumerator
* 文件描述：预测迭代器?
* 作者：aronliang
* 创建时间：2023/06/21 16:52:22
* 修改记录：
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Entt.Entities.Helpers
{
    public struct PredicateEnumerator<TValue>:IEnumerator<TValue>
    {
        private readonly IEnumerable<TValue> contents;
        private Func<TValue, bool> predicate;
        private IEnumerator<TValue> enumerator;

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
    /// 映射迭代器
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