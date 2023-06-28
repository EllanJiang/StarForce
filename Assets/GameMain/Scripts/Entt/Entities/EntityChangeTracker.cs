/*
* 文件名：EntityChangeTracker
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/28 15:09:12
* 修改记录：
*/

using System;

namespace Entt.Entities
{
    /// <summary>
    /// 跟踪Entity的改变（Entity的创建、销毁，更新，组件的 添加组件，移除组件，创建Entity，销毁Entity，更新Entity，更新组件等）
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    public abstract class EntityChangeTracker<TEntityKey, TComponent>: IDisposable where TEntityKey : IEntityKey
    {
        protected readonly EntityRegistry<TEntityKey> Registry;
        
        protected EntityChangeTracker(EntityRegistry<TEntityKey> registry)
        {
            this.Registry = registry;
        }
        
        /// <summary>
        /// 注册Entity的监听事件
        /// </summary>
        public void Install()
        {
            if (Registry.TryGetWritablePool<TComponent>(out var pool))
            {
                pool.Updated += OnPositionUpdated;
                pool.Created += OnPositionCreated;
                pool.DestroyedNotify += OnComplexDestroyed;
            }
            else
            {
                var roPool = Registry.GetPool<TComponent>();
                roPool.Created += OnPositionCreated;
                roPool.Destroyed += OnBasicDestroyed;
            }
        }

        void OnBasicDestroyed(object sender, TEntityKey e)
        {
            OnPositionDestroyed(sender, (e, default));
        }

        void OnComplexDestroyed(object sender, (TEntityKey k, TComponent old) x) => OnPositionDestroyed(sender, (x.k, x.old));
        
        protected virtual void OnPositionDestroyed(object sender, (TEntityKey key, Optional<TComponent> old) e) { }
        protected abstract void OnPositionUpdated(object sender, (TEntityKey key, TComponent old) e);
        protected abstract void OnPositionCreated(object sender, TEntityKey e);
        
       
        public void Dispose()
        {
            Dispose(true);
        }
        
        ~EntityChangeTracker()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Registry.TryGetWritablePool<TComponent>(out var pool))
            {
                pool.Updated -= OnPositionUpdated;
                pool.Created -= OnPositionCreated;
                pool.DestroyedNotify -= OnComplexDestroyed;
            }
            else
            {
                var roPool = Registry.GetPool<TComponent>();
                roPool.Created -= OnPositionCreated;
                roPool.Destroyed -= OnBasicDestroyed;
            }
        }
    }
}