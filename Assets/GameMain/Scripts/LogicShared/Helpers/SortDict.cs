/*
* 文件名：SortDict
* 文件描述：
* 作者：aronliang
* 创建时间：2023/08/07 15:10:10
* 修改记录：
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace LogicShared
{
    /// <summary>
    /// 有序字典
    /// </summary>
    /// <typeparam name="K">key</typeparam>
    /// <typeparam name="V">value</typeparam>
    public class SortDict<K,V>: IEnumerable<KeyValuePair<K, V>>
    {
        protected List<K> m_Keys;
        protected List<V> m_Values;
        protected Dictionary<K, int> m_Map;          //key:字典的键，Value：字典的值在m_Values中的下标
        protected KeyCollection m_KeyCollection;     //用于遍历Key
        protected ValueCollection m_ValueCollection; //用于遍历Value
       
        private IEqualityComparer<K> m_Comparer;
        public IEqualityComparer<K> Comparer
        {
            get { return m_Comparer; }
        }
        
        public SortDict():this(0,null)
        {
        }
        
        public SortDict(int capacity):this(capacity,null)
        {
        }
        
        public SortDict(int capacity,IEqualityComparer<K> comparer)
        {
            m_Keys = new List<K>(capacity);
            m_Values = new List<V>(capacity);
            m_Comparer = comparer;
            if (m_Comparer == null)
            {
                m_Comparer = EqualityComparer<K>.Default;
            }
            m_Map = new Dictionary<K, int>(capacity,m_Comparer);
        }
        
        public int Count => m_Values.Count;

        public KeyCollection Keys
        {
            get
            {
                if (m_KeyCollection == null)
                {
                    m_KeyCollection = new KeyCollection(this);
                }
                return m_KeyCollection;
            }
        }
        public ValueCollection Values
        {
            get
            {
                if (m_ValueCollection == null)
                {
                    m_ValueCollection = new ValueCollection(this);
                }
                return m_ValueCollection;
            }
        }
        
        /// <summary>
        /// 根据Key获取Value
        /// </summary>
        /// <param name="key"></param>
        public V this[K key]
        {
            get
            {
                if (!TryGetValue(key, out var value))
                {
                    Logger.Error($"The given key '{key}' was not present int the SortDict.");
                    return default;
                }
                
                return value;
            }
            set
            {
                if (!m_Map.TryGetValue(key, out var index))
                {
                    Add(key, value);
                    return;
                }
                
                m_Values[index] = value;
            }
        }
        
        public K GetKeyByIndex(int index)
        {
            return m_Keys[index];
        }

        public V GetValueByIndex(int index)
        {
            return m_Values[index];
        }

        public bool TryAdd(K key, V value)
        {
            if (ContainsKey(key))
            {
                Logger.Error($"An item with the same key has already been added, Key :{key}");
                return false;
            }
            
            m_Keys.Add(key);
            m_Values.Add(value);
            m_Map.Add(key, m_Keys.Count - 1);
            
            return true;
        }

        public void Add(K key, V value)
        {
            if (ContainsKey(key))
            {
                Logger.Error($"An item with the same key has already been added, Key :{key}");
                return;
            }
            
            m_Keys.Add(key);
            m_Values.Add(value);
            m_Map.Add(key, m_Keys.Count - 1);
        }

        public void Remove(K key, out V value)
        {
            value = Remove(key);
        }
        
        public V Remove(K key)
        {
            V val = default(V);
            if (m_Map.TryGetValue(key, out var index))
            {
                val = m_Values[index];
                m_Values.RemoveAt(index);
                m_Keys.RemoveAt(index);
                int len = m_Keys.Count;
                m_Map.Clear();
                for (int i = 0; i < len; i++)
                {
                    m_Map.Add(m_Keys[i], i);
                }
            }

            return val;
        }
        
        public bool ContainsKey(K key)
        {
            return m_Map.ContainsKey(key);
        }

        public V GetValue(K key)
        {
            V val = default(V);
            if (m_Map.TryGetValue(key, out var index))
            {
                val = m_Values[index];
            }
            
            return val;
        }

        public bool TryGetValue(K key, out V value)
        {
            try
            {
                value = default(V);
                if (m_Map.TryGetValue(key, out var index))
                {
                    value = m_Values[index];
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                throw;
            }

            return false;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void Clear()
        {
            m_Keys.Clear();
            m_Values.Clear();
            m_Map.Clear();
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<KeyValuePair<K, V>>, IDictionaryEnumerator
        {
            private SortDict<K, V> dictionary;
            private int index;
            private KeyValuePair<K, V> current;
            
            internal Enumerator(SortDict<K, V> dictionary)
            {
                this.dictionary = dictionary;
                index = 0;
                current = new KeyValuePair<K, V>();
            }
            
            public bool MoveNext()
            {
                while ((uint)index < (uint)dictionary.Count)
                {
                    current = new KeyValuePair<K, V>(dictionary.m_Keys[index], dictionary.m_Values[index]);
                    index++;
                    return true;
                }

                index = dictionary.Count + 1;
                current = new KeyValuePair<K, V>();
                return false;
            }

            public KeyValuePair<K, V> Current => current;

            public void Dispose()
            {
            }

            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || (index == dictionary.Count + 1))
                    {
                        throw new Exception("[Current] SortDict Key Enumerator Exception");
                    }
                    return new KeyValuePair<K, V>(current.Key, current.Value);
                }
            }

            void IEnumerator.Reset()
            {
                index = 0;
                current = new KeyValuePair<K, V>();
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (index == 0 || (index == dictionary.Count + 1))
                    {
                        throw new Exception("[Entry] SortDict key Enumerator Exception");
                    }

                    return new DictionaryEntry(current.Key, current.Value);
                }
            }

            object IDictionaryEnumerator.Key
            {
                get
                {
                    if (index == 0 || (index == dictionary.Count + 1))
                    {
                        throw new Exception("[Key] SortDict key Enumerator Exception");
                    }

                    return current.Key;
                }
            }

            object IDictionaryEnumerator.Value
            {
                get
                {
                    if (index == 0 || (index == dictionary.Count + 1))
                    {
                        throw new Exception("[Value] SortDict key Enumerator Exception");
                    }

                    return current.Value;
                }
            }
        }
        
        public sealed class KeyCollection : ICollection<K>, ICollection
        {
            private SortDict<K, V> dictionary;
            public KeyCollection(SortDict<K, V> dictionary)
            {
                if (dictionary == null)
                {
                    throw new Exception("dictionary == null exception");
                }
                this.dictionary = dictionary;
            }

            public int Count => dictionary.m_Keys.Count;

            public bool IsSynchronized => true;

            public object SyncRoot =>  ((ICollection) dictionary.m_Map).SyncRoot;

            public bool IsReadOnly => true;

            public void Add(K item)
            {
            }

            public void Clear()
            {
            }

            public bool Contains(K item)
            {
                return this.dictionary.ContainsKey(item);
            }

            public void CopyTo(K[] array, int index)
            {
                int count = dictionary.m_Keys.Count;
                for (int i = 0; i < count; i++)
                {
                    array[index++] = dictionary.m_Keys[i];
                }
            }

            public void CopyTo(Array array, int index)
            {
                K[] keys = array as K[];
                if (keys != null)
                {
                    CopyTo(keys, index);
                }
            }
            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            public bool Remove(K item)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            IEnumerator<K> IEnumerable<K>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            public struct Enumerator : IEnumerator<K>, IEnumerator, IDisposable
            {
                private SortDict<K, V> dictionary;
                private int index;
                private K currentKey;
                public K Current { get { return currentKey; } }

                object IEnumerator.Current => throw new NotImplementedException();
                internal Enumerator(SortDict<K, V> dictionary)
                {
                    this.dictionary = dictionary;
                    index = 0;
                    currentKey = default(K);
                }
                public void Dispose()
                {
                }
                public bool MoveNext()
                {
                    while ((uint)index < (uint)dictionary.Count)
                    {
                        //if (dictionary.mKeys[index].GetHashCode() >= 0)
                        {
                            currentKey = dictionary.m_Keys[index];
                            index++;
                            return true;
                        }
                       // index++;
                    }
                    index = dictionary.Count + 1;
                    currentKey = default(K);
                    return false;
                }

                public void Reset()
                {
                    index = 0;
                    currentKey = default(K);
                }
            }
        }
        public sealed class ValueCollection : ICollection<V>, ICollection
        {
            private SortDict<K, V> dictionary;
            public ValueCollection(SortDict<K, V> dictionary)
            {
                this.dictionary = dictionary;
            }

            public int Count { get { return this.dictionary.m_Values.Count; } }

            public bool IsSynchronized => true;

            public object SyncRoot => ((ICollection)dictionary.m_Map).SyncRoot;

            public bool IsReadOnly => true;

            public void Add(V item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(V item)
            {
                return dictionary.m_Values.Contains(item);
            }

            public void CopyTo(V[] array, int index)
            {
                int count = dictionary.m_Values.Count;
                for (int i = 0; i < count; i++)
                {
                    array[index++] = dictionary.m_Values[i];
                }
            }

            public void CopyTo(Array array, int index)
            {
                V[] keys = array as V[];
                if (keys != null)
                {
                    CopyTo(keys, index);
                }
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            public bool Remove(V item)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            IEnumerator<V> IEnumerable<V>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            public struct Enumerator : IEnumerator<V>, IEnumerator
            {
                private SortDict<K, V> dictionary;
                private V currentValue;
                private int index;
                
                public V Current => currentValue;
                object IEnumerator.Current => currentValue;

                internal Enumerator(SortDict<K, V> dictionary)
                {
                    this.dictionary = dictionary;
                    currentValue = default(V);
                    index = 0;
                }

                public bool MoveNext()
                {
                    while ((uint)index < (uint)dictionary.m_Values.Count)
                    {
                        currentValue = dictionary.m_Values[index];
                        index++;
                        return true;
                    }
                    index = dictionary.m_Values.Count + 1;
                    currentValue = default(V);
                    return false;
                }

                public void Reset()
                {
                    index = 0;
                    currentValue = default(V);
                }

                public void Dispose()
                {
                }
            }
        }
    }
}