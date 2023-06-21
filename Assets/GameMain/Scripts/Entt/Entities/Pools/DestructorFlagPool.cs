/*
* 文件名：DestructorFlagPool
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/21 19:57:55
* 修改记录：
*/

using System;

namespace Entt.Entities.Pools
{
    /// <summary>
    /// 可析构的标志对象池
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    public class DestructorFlagPool<TEntityKey, TComponent> : FlagPool<TEntityKey, TComponent> where TEntityKey : IEntityKey
    {
        private readonly IComponentRegistration<TEntityKey, TComponent> componentRegistration;
        
        internal DestructorFlagPool(TComponent sharedData, IComponentRegistration<TEntityKey, TComponent> componentRegistration): base(sharedData)
        {
            this.componentRegistration = componentRegistration ??
                                         throw new ArgumentNullException(nameof(componentRegistration));
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
                var k = Last;
                Remove(k);
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