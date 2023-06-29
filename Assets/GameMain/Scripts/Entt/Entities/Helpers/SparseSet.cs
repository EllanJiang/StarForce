/*
* 文件名：SparseSet
* 文件描述：稀疏集 参考 https://www.geeksforgeeks.org/sparse-set/
          增加一个元素，删除一个元素，查找一个元素是否存在集合中的时间复杂度都是O(1)
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
    /// Sparse：稀疏集，使用元素值作为索引，使用元素值在dense数组中的索引作为值，例如要保存数值7，从上面例子可以看出，数值7在dense中的索引是2，因此在sparse中，sparse[7]=2
    /// Dense：密集集，用于存储元素值
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
            /// <summary>
            /// 该元素在dense数组中的索引
            /// </summary>
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
        
        /// <summary>
        /// 元素总数，当然取的是dense的数量
        /// </summary>
        public int Count => dense.Count;
        
        /// <summary>
        /// 添加一个EntityKey
        /// </summary>
        /// <param name="entityKey"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Add(TEntityKey entityKey)
        {
            if (Contains(entityKey))
            {
                throw new ArgumentException("Entity already exists in this collection");
            }
            
            //使用Key作为sparse的索引，也就是说sparse数组的大小会随着Entity的增加而增大
            var sparseArrayIndex = entityKey.Key; 
            
            //SparseElement的Key为什么是dense.Count，因为dense.Count就是entityKey存储在dense中的位置,即dense[dense.Count]=entityKey
            //到时就可以根据SparseElement的DenseArrayIndex来获取SparseElement在dense中的位置
            sparse.StoreAt(sparseArrayIndex, new SparseElement(dense.Count, entityKey.Age));
            dense.Add(entityKey);
        }
        
        /// <summary>
        /// To delete an element entityKey, we replace it with last EntityKey in dense and update index of last EntityKey in sparse. Finally decrement Count by 1.
        /// </summary>
        public virtual bool Remove(TEntityKey entityKey)
        {
            return RemoveElement(entityKey) != -1;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityKey">in 修饰的参数表示想通过引用来传递参数，跟ref 和 out类似。但是有一点不同，ref修饰的参数可以被修改，out修饰的参数必须由调用的方法进行修改，但是in修饰的参数不能被修改，跟C++中的const& 类似</param>
        /// <returns>返回删除的EntityKey在dense数组中的索引</returns>
        /// <exception cref="ArgumentException"></exception>
        protected int RemoveElement(in TEntityKey entityKey)
        {
            var sparseArrayIndex = entityKey.Key;
            if (sparseArrayIndex >= sparse.Count)
            {
                return -1;
            }

            var sparseElement = sparse[sparseArrayIndex];
            if (!sparseElement.InUse || entityKey.Age != sparseElement.Age)
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
        /// 扩充Sparse容量
        /// </summary>
        /// <param name="capacity"></param>
        public void Reserve(int capacity)
        {
            Capacity = Math.Max(capacity, Capacity);
        }
        
        public virtual int Capacity
        {
            get { return sparse.Capacity; }
            set { sparse.Capacity = value; }
        }
        
        /// <summary>
        /// 稀疏集中是否包含指定EntityKey
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Contains(TEntityKey entity)
        {
            var sparseArrayIndex = entity.Key;
            if (sparseArrayIndex < sparse.Count)
            {
                var sparseElement = sparse[sparseArrayIndex];
                if (!sparseElement.InUse || entity.Age != sparseElement.Age)
                {
                    return false;
                }
                
                return true;
            }

            return false;
        }
       
        /// <summary>
        /// 获取EntityKey在dense中的索引
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int IndexOf(TEntityKey entity)
        {
            var sparseArrayIndex = entity.Key;
            if (sparseArrayIndex >= sparse.Count)
            {
                return -1;
            }

            var sparseElement = sparse[sparseArrayIndex];
            if (!sparseElement.InUse || entity.Age != sparseElement.Age)
            {
                return -1;
            }

            return sparseElement.DenseArrayIndex;
        }
        
        /// <summary>
        /// 跟Remove的功能一样，跟Remove的区别是只有稀疏集中包含有该EntityKey才会进行Remove
        /// </summary>
        /// <param name="entity"></param>
        public void Reset(TEntityKey entity)
        {
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
        /// 将两个索引处的EntityKey进行交换
        /// </summary>
        /// <param name="idxSrc">dense中的源索引</param>
        /// <param name="idxDst">dense中的目标索引</param>
        protected virtual void Swap(int idxSrc, int idxDst)
        {
            var sparseSrc = dense[idxSrc].Key;
            var sparseDst = dense[idxDst].Key;
            dense.Swap(idxSrc, idxDst);
            sparse.Swap(sparseSrc, sparseDst);
        }
        
        /// <summary>
        /// todo 这样做有啥用？
        /// </summary>
        /// <param name="otherSet"></param>
        public void Respect(IEnumerable<TEntityKey> otherSet)
        {
            var targetIndex = dense.Count - 1;
            foreach (var otherEntityKey in otherSet)
            {
                var otherIndex = IndexOf(otherEntityKey);
                if (otherIndex != -1)
                {
                    if (EqualityHandler.Equals(otherEntityKey, dense[targetIndex]))
                    {
                        Swap(targetIndex, otherIndex);
                    }

                    targetIndex -= 1;
                }
            }
        }

        /// <summary>
        /// 清空稀疏集
        /// </summary>
        public virtual void RemoveAll()
        {
            Clear();
        }
        
        /// <summary>
        /// 清空稀疏集
        /// </summary>
        public void Clear()
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
        /// 把所有元素全部复制到entities中
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