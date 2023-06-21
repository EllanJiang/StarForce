/*
* 文件名：ComponentRegistration
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/21 19:35:42
* 修改记录：
*/

using System;

namespace Entt.Entities
{
    /// <summary>
    /// 组件注册
    /// </summary>
    public static class ComponentRegistration
    {
        class ComponentRegistration0<TEntityKey,T>:IComponentRegistration<TEntityKey,T> where TEntityKey:IEntityKey
        {
            //Entity容器
            private readonly EntityRegistry<TEntityKey> registry;
            private readonly Func<T> constructor;
            private readonly Action<TEntityKey, EntityRegistry<TEntityKey>, T>? destructor;
            
            public int Index { get; }
            
            public ComponentRegistration0(int index, EntityRegistry<TEntityKey> registry,
                Func<T> constructor,
                Action<TEntityKey, EntityRegistry<TEntityKey>, T>? destructor = null)
            {
                this.registry = registry;
                this.constructor = constructor;
                this.destructor = destructor;
                Index = index;
            }
            
            public T Create()
            {
                return constructor();
            }

            public void Destruct(TEntityKey k, T o)
            {
                destructor?.Invoke(k, registry, o);
            }

            public bool HasDestructor()
            {
                return destructor != null;
            }
        }
        
        public static IComponentRegistration<TEntityKey, T> Create<TEntityKey, T>(int count, EntityRegistry<TEntityKey> registry, 
            Action<TEntityKey, EntityRegistry<TEntityKey>, T>? destructor = null) 
            where TEntityKey : IEntityKey
        {
            return new ComponentRegistration0<TEntityKey, T>(count, registry,
                () => throw new InvalidOperationException($"The component {typeof(T)} has no registered default constructor."),
                destructor);
        }

        public static IComponentRegistration<TEntityKey, T> Create<TEntityKey, T>(int count, 
            EntityRegistry<TEntityKey> registry, 
            Func<T> func, 
            Action<TEntityKey, EntityRegistry<TEntityKey>, T>? destructor = null)
            where TEntityKey : IEntityKey
        {
            return new ComponentRegistration0<TEntityKey, T>(count, registry, func, destructor);
        }
    }
}