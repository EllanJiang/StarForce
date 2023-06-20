/*
* 文件名：SparseSet
* 文件描述：稀疏集 参考 https://www.geeksforgeeks.org/sparse-set/ 
* 作者：aronliang
* 创建时间：2023/06/20 16:27:04
* 修改记录：
*/

/* 这是举例说明
    Let there be a set with two elements {3, 5}, maximum
    value as 10 and capacity as 4. The set would be 
    represented as below.

    Initially:
    maxVal   = 10  // Size of sparse，同时也是能保存到集合中的最大值
    capacity = 4   // Size of dense
    n = 2          // Current number of elements in set

    // Dense Set 存放实际元素
    // dense[] Stores actual elements  
    dense[]  = {3, 5, _, _}

    //sparse Set 存放元素的索引
    // Uses actual elements as index and stores 
    // indexes of dense[]
    sparse[] = {_, _, _, 0, _, 1, _, _, _, _,}

    '_' means it can be any value and not used in sparse set

    Insert 7:
    n        = 3
    dense[]  = {3, 5, 7, _}
    sparse[] = {_, _, _, 0, _, 1, _, 2, _, _,}

    Insert 4:
    n        = 4
    dense[]  = {3, 5, 7, 4}
    sparse[] = {_, _, _, 0, 3, 1, _, 2, _, _,}

    Delete 3:
    n        = 3
    dense[]  = {4, 5, 7, _}
    sparse[] = {_, _, _, _, 0, 1, _, 2, _, _,}

    Clear (Remove All):
    n        = 0
    dense[]  = {_, _, _, _}
    sparse[] = {_, _, _, _, _, _, _, _, _, _,}
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Entt.Entities.Helpers
{
    /// <summary>
    /// 用于存储EntityID的稀疏集
    /// Sparse：稀疏集，用于存储元素在DenseArray中的下标
    /// Dense：密集集，用于存储元素
    /// 具体例子参考上面的举例说明
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public class SparseSet<TEntityKey> :IEnumerable<TEntityKey> where TEntityKey : IEntityKey
    {
        /// <summary>
        /// 稀疏集元素，跟IEntityKey的数据结构相同
        /// </summary>
        readonly struct SparseElement
        {
            //24-31位属于Age
            const uint AgeMask = 0xFF00_0000;
            //0-22位属于Key
            const uint KeyMask = 0x007F_FFFF;
            //23位用于判断是否正在使用中
            const uint ValidMask = 0x0080_0000;

            //原始数据
            readonly uint rawData;
            //密集集数组下标
            public int DenseArrayIndex => (int)(rawData & KeyMask);
            public int Age => (int)(rawData & AgeMask) >> 24;
            public bool InUse => (rawData & ValidMask) != 0;

            public SparseElement(int key, byte age)
            {
                rawData = (uint)(key & KeyMask);
                rawData |= (uint)(age << 24); 
                rawData |= ValidMask;
            }

            public static SparseElement Unused()
            {
                return default; 
            }
        }
        
        static readonly EqualityComparer<TEntityKey> EqualityHandler = EqualityComparer<TEntityKey>.Default;
        
        //按照插入顺序保存EntityKey
        private readonly SegmentedRawList<TEntityKey> dense;
        private readonly RawList<SparseElement> sparse;
        
        public SparseSet()
        {
            dense = new SegmentedRawList<TEntityKey>();
            sparse = new RawList<SparseElement>();
        }
        
        public int Count => dense.Count;
        
        /// <summary>
        /// 添加一个EntityKey
        /// </summary>
        /// <param name="e"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Add(TEntityKey e)
        {
            if (Contains(e))
            {
                throw new ArgumentException("Entity already exists in this collection");
            }
            
            var sparseArrayIndex = e.Key; 
            //SparseElement的Key为什么是dense.Count，因为dense.Count就是e存储在dense中的位置,即dense[dense.Count]=e
            //到时就可以根据SparseElement的DenseArrayIndex来获取Element在dense中的位置
            sparse.StoreAt(sparseArrayIndex, new SparseElement(dense.Count, e.Age));
            dense.Add(e);
        }
        
        public virtual bool Remove(TEntityKey e)
        {
            return RemoveEntry(e) != -1;
        }

        /// <summary>
        /// To delete an element e, we replace it with last EntityKey in dense and update index of last entity in sparse. Finally decrement Count by 1.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        protected int RemoveEntry(in TEntityKey e)
        {
            var sparseArrayIndex = e.Key;
            if (sparseArrayIndex >= sparse.Count)
            {
                return -1;
            }

            var sparseElement = sparse[sparseArrayIndex];
            if (!sparseElement.InUse || e.Age != sparseElement.Age)
            {
                return -1;
            }
            
            if (dense.Count == 0)
            {
                throw new ArgumentException("Inconsistent sparse-set detected");
            }

            var lastFilledEntityKey = dense[dense.Count - 1];
            var lastFilledIndex = lastFilledEntityKey.Key;
            
            //更新最后填充的EntityKey在sparse中指向dense的索引为将要删除的EntityKey的DenseArrayIndex
            var denseArrayIndex = sparseElement.DenseArrayIndex;
            sparse[lastFilledIndex] = new SparseElement(denseArrayIndex, lastFilledEntityKey.Age);
            sparse[sparseArrayIndex] = SparseElement.Unused();
            
            //这里使用最后填充的EntityKey替换掉将要删除的EntityKey
            dense[denseArrayIndex] = lastFilledEntityKey;
            //最后删除最后填充的EntityKey
            dense.RemoveLast();
            return denseArrayIndex;
            
        }
        
        public TEntityKey Last => dense[dense.Count - 1];
        
        /// <summary>
        ///  Increases the capacity of the sparse set. This never reduces the capacity.
        /// </summary>
        /// <param name="cap"></param>
        public void Reserve(int cap)
        {
            Capacity = Math.Max(cap, Capacity);
        }
        
        public virtual int Capacity
        {
            get { return sparse.Capacity; }
            set { sparse.Capacity = value; }
        }
        
        public bool Contains(TEntityKey entity)
        {
            var pos = entity.Key;
            if (pos < sparse.Count)
            {
                var rk = sparse[pos];
                if (!rk.InUse || entity.Age != rk.Age)
                {
                    return false;
                }
                
                return true;
            }

            return false;
        }
       
        /// <summary>
        /// 获取entity在dense中的索引
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int IndexOf(TEntityKey entity)
        {
            var pos = entity.Key;
            if (pos >= sparse.Count)
            {
                return -1;
            }

            var sparseElement = sparse[pos];
            if (!sparseElement.InUse || entity.Age != sparseElement.Age)
            {
                return -1;
            }

            return sparseElement.DenseArrayIndex;
        }
        
        public void Reset(TEntityKey entity)
        {
            // same as remove, but do not fail if not there
            if (Contains(entity))
            {
                Remove(entity);
            }
        }
        
        IEnumerator<TEntityKey> IEnumerable<TEntityKey>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public SegmentedRawList<TEntityKey>.Enumerator GetEnumerator()
        {
            return dense.GetEnumerator();
        }
        
        /// <summary>
        /// 将两个索引处的Entity进行交换
        /// </summary>
        /// <param name="idxSrc">dense中的源索引</param>
        /// <param name="idxTarget">dense中的目标索引</param>
        protected virtual void Swap(int idxSrc, int idxTarget)
        {
            var sparseSrc = dense[idxSrc].Key;
            var sparseTarget = dense[idxTarget].Key;
            dense.Swap(idxSrc, idxTarget);
            sparse.Swap(sparseSrc, sparseTarget);
        }
        
        /// <summary>
        /// 将最后一个Entity和other进行交换
        /// </summary>
        /// <param name="other"></param>
        public void Respect(IEnumerable<TEntityKey> other)
        {
            // where do we drop items that have been moved out of the way ..
            var targetPosition = dense.Count - 1;
            foreach (var otherEntity in other)
            {
                var posLocal = IndexOf(otherEntity);
                if (posLocal != -1)
                {
                    if (EqualityHandler.Equals(otherEntity, dense[targetPosition]))
                    {
                        Swap(targetPosition, posLocal);
                    }

                    targetPosition -= 1;
                }
            }
        }

        /// <summary>
        /// 移除稀疏集中所有Entity
        /// </summary>
        public virtual void RemoveAll()
        {
            sparse.Clear();
            dense.Clear();
        }

        /// <summary>
        /// 从一个迭代器中创建稀疏集
        /// </summary>
        /// <param name="members"></param>
        /// <typeparam name="TEnumerator"></typeparam>
        /// <returns></returns>
        public static SparseSet<TEntityKey> CreateFrom<TEnumerator>(TEnumerator members)
            where TEnumerator : IEnumerator<TEntityKey>
        {
            var set = new SparseSet<TEntityKey>();
            while (members.MoveNext())
            {
                set.Add(members.Current!);
            }

            return set;
        }

        /// <summary>
        /// 把稀疏集中的Entity全部复制到entities中
        /// </summary>
        /// <param name="entities"></param>
        public void CopyTo(RawList<TEntityKey> entities)
        {
            entities.Capacity = Math.Max(entities.Capacity, Count);
            entities.Clear();

            if (dense.Count == 0)
            {
                return;
            }

            var count = 0;
            var toBeCopied = dense.Count;
            var unsafeData = dense.UnsafeData;
            var data = entities.UnsafeData;
            
            for (var segmentIndex = 0; segmentIndex < unsafeData.Length; segmentIndex++)
            {
                var segment = unsafeData[segmentIndex];
                if (segment == null)
                {
                    continue;
                }
                
                for (var dataIndex = 0; dataIndex < segment.Length; dataIndex++)
                {
                    data[count] = segment[dataIndex];
                    count += 1;
                    if (count == toBeCopied)
                    {
                        entities.UnsafeSetCount(dense.Count);
                        return;
                    }
                }
            }
        }
    }
}