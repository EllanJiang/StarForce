/*
* 文件名：SparseDictionary
* 文件描述：使用稀疏集实现的字典
* 作者：aronliang
* 创建时间：2023/06/20 16:26:46
* 修改记录：
*/

using System.Diagnostics.CodeAnalysis;

namespace Entt.Entities.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntityKey">这是字典的Key</typeparam>
    /// <typeparam name="TComponent">这是字典的Value</typeparam>
    public class SparseDictionary<TEntityKey, TComponent> : SparseSet<TEntityKey>, ISortableCollection<TComponent> 
        where TEntityKey:IEntityKey
    {
        //SparseSet存储字典的Key值
        //valueList存储字典的Value值
        private readonly RawList<TComponent> valueList;

        public SparseDictionary()
        {
            valueList = new RawList<TComponent>();
        }

        public override int Capacity
        {
            get
            {
                return base.Capacity;
            }
            set
            {
                base.Capacity = value;
                valueList.Capacity = value;
            }
        }

        /// <summary>
        /// 向字典中添加键值对，按照添加的顺序保存键值对
        /// </summary>
        /// <param name="entityKey"></param>
        /// <param name="component"></param>
        public virtual void Add(TEntityKey entityKey, in TComponent component)
        {
            base.Add(entityKey);
            valueList.Add(component);
        }
        
        public override bool Remove(TEntityKey entityKey)
        {
            var index = RemoveElement(entityKey);
            if (index == -1)
            {
                return false;
            }

            valueList[index] = valueList[valueList.Count - 1];
            valueList.RemoveLast();
            return true;
        }

        public TComponent this[int index]
        {
            get { return valueList[index]; }
        }

        void ISortableCollection<TComponent>.Swap(int idxSrc, int idxTgt)
        {
            Swap(idxSrc, idxTgt);
        }
        
        protected override void Swap(int idxSrc, int idxTgt)
        {
            base.Swap(idxSrc, idxTgt);
            valueList.Swap(idxSrc, idxTgt);
        }
        
        public override void RemoveAll()
        {
            base.RemoveAll();
            valueList.Clear();
        }
        
        /// <summary>
        /// 尝试从字典中根据Key值取出Value值
        /// </summary>
        /// <param name="entityKey"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TryGet(TEntityKey entityKey, [MaybeNullWhen(false)] out TComponent component)
        {
            var idx = IndexOf(entityKey);
            if (idx >= 0)
            {
                component = valueList[idx];
                return true;
            }

            component = default;
            return false;
        }
        
        /// <summary>
        /// 尝试从字典中根据Key值取出Value值的引用，但是不能根据该引用来修改Value值（因为是只读的）
        /// </summary>
        /// <param name="entityKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public ref readonly TComponent? TryGetRef(TEntityKey entityKey, ref TComponent? defaultValue, out bool success)
        {
            var idx = IndexOf(entityKey);
            if (idx >= 0)
            {
                return ref valueList.TryGetRef(idx, ref defaultValue, out success);
            }

            success = false;
            return ref defaultValue;
        }
        
        /// <summary>
        /// 尝试从字典中根据Key值取出Value值的引用，而且可以根据该引用来修改Value值
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="defaultValue"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public ref TComponent? TryGetModifiableRef(TEntityKey entity, ref TComponent? defaultValue, out bool success)
        {
            var idx = IndexOf(entity);
            if (idx >= 0)
            {
                return ref valueList.TryGetRef(idx, ref defaultValue, out success);
            }

            success = false;
            return ref defaultValue;
        }
        
        /// <summary>
        /// 更新EntityKey的Value值
        /// </summary>
        /// <param name="entityKey"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public virtual bool WriteBack(TEntityKey entityKey, in TComponent component)
        {
            var idx = IndexOf(entityKey);
            if (idx == -1)
            {
                return false;
            }

            valueList[idx] = component;
            return true;
        }
    }
}