/*
* 文件名：DestructorPool
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/21 18:03:24
* 修改记录：
*/

using System;

namespace Entt.Entities.Pools
{
    /// <summary>
    /// 析构对象池
    /// </summary>
    public class DestructorPool<TEntityKey, TComponent> : Pool<TEntityKey, TComponent> where TEntityKey : IEntityKey
    {
        readonly IComponentRegistration<TEntityKey, TComponent> componentRegistration;

        internal DestructorPool(IComponentRegistration<TEntityKey, TComponent> componentRegistration)
        {
            this.componentRegistration = componentRegistration ?? throw new ArgumentNullException(nameof(componentRegistration));
        }
        
        public override bool Remove(TEntityKey entityKey)
        {
            if (TryGet(entityKey, out var com))
            {
                if (base.Remove(entityKey))
                {
                    componentRegistration.Destruct(entityKey, com);
                    return true;
                }
            }

            return false;
        }

        
        public override void RemoveAll()
        {
            while (Count > 0)
            {
                var entityKey = Last;
                Remove(entityKey);
            }
        }
        
        public override bool WriteBack(TEntityKey entityKey, in TComponent component)
        {
            if (TryGet(entityKey, out var c))
            {
                var writeBack = base.WriteBack(entityKey, in component);
                if (writeBack)
                {
                    componentRegistration.Destruct(entityKey, c);
                }

                return writeBack;
            }

            return false;
        }
    }
}