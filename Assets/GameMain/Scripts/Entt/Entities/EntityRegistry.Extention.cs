/*
  作者：LTH
  文件描述：
  文件名：EntityRegistry_Extention
  创建时间：2023/06/23 21:06:SS
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Entt.Entities.Pools;

namespace Entt.Entities
{
    public partial class EntityRegistry<TEntityKey> : IEntityViewFactory<TEntityKey>,
                                                        IEntityPoolAccess<TEntityKey>,
                                                        IEntityComponentRegistry<TEntityKey>
                                                        where TEntityKey : IEntityKey
    {
        /// <summary>
        /// 对象池item：包括一个可读可写Pool和一个只读Pool
        /// </summary>
        readonly struct PoolEntry
        {
            readonly IPool<TEntityKey>? pool;
            public IReadOnlyPool<TEntityKey> ReadonlyPool { get; }
            
            public PoolEntry(IReadOnlyPool<TEntityKey> readonlyPool)
            {
                this.ReadonlyPool = readonlyPool ?? throw new ArgumentNullException(nameof(readonlyPool));
                this.pool = null;
            }
            
            public PoolEntry(IPool<TEntityKey> pool)
            {
                this.pool = pool ?? throw new ArgumentNullException(nameof(pool));
                this.ReadonlyPool = pool;
            }
            
            public bool TryGetPool([MaybeNullWhen(false)] out IPool<TEntityKey> poolResult)
            {
                poolResult = this.pool;
                return poolResult != null;
            }
            
            public override string ToString()
            {
                return $"(Pool: {pool})";
            }
        }
        
        /// <summary>
        /// 附加项
        /// </summary>
        readonly struct Attachment
        {
            public readonly TEntityKey Entity;
            public readonly object? Tag;

            public Attachment(TEntityKey entity, object? tag)
            {
                Entity = entity;
                Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            }
        }

        /// <summary>
        /// EntityKey迭代器
        /// </summary>
        public struct EntityKeyEnumerator : IEnumerator<TEntityKey>
        {
            readonly List<TEntityKey> contents;

            int index;
            TEntityKey current;

            internal EntityKeyEnumerator(List<TEntityKey> widget) : this()
            {
                contents = widget;
                index = -1;
                current = default!;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                while (index + 1 < contents.Count)
                {
                    index += 1;
                    var c = contents[index];
                    if (c.Key == index)
                    {
                        current = contents[index];
                        return true;
                    }
                }

                current = default!;
                return false;
            }

            public void Reset()
            {
                index = -1;
                current = default!;
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public TEntityKey Current
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