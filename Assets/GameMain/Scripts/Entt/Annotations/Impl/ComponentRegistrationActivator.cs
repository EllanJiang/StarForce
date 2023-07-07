/*
  作者：LTH
  文件描述：
  文件名：ComponentRegistrationActivator
  创建时间：2023/07/07 22:07:SS
*/

using System;
using Entt.Entities;

namespace Entt.Annotations.Impl
{
    /// <summary>
    /// 组件注册响应器
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public class ComponentRegistrationActivator<TEntityKey> : EntityActivatorBase<TEntityKey> where TEntityKey : IEntityKey
    {
        /// <summary>
        /// lazy延迟初始化，仅在使用的时候才初始化，不使用就不初始化，可以省去一部分开销。Lazy是线程安全的
        /// </summary>
        static readonly Lazy<ComponentRegistrationActivator<TEntityKey>> instanceHolder = new Lazy<ComponentRegistrationActivator<TEntityKey>>();
        
        /// <summary>
        /// 单例
        /// </summary>
        public static ComponentRegistrationActivator<TEntityKey> Instance => instanceHolder.Value; 

        /// <summary>
        /// 处理类型方法
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="registry"></param>
        /// <typeparam name="TComponent"></typeparam>
        protected override void ProcessTyped<TComponent>(EntityComponentRegistration registration, IEntityComponentRegistry<TEntityKey> registry)
        {
            //是否有构造函数
            bool hasConstructor = registration.TryGet<ConstructorRegistration<TComponent>>(out var constructor);
            //是否有析构函数
            bool hasDestructor = registration.TryGet<DestructorRegistration<TEntityKey, TComponent>>(out var destructor);

            if (registration.TryGet(out ComponentRegistrationExtensions.FlagMarker _))
            {
                //注册标志
                registry.RegisterFlag<TComponent>();
            }
            else if (hasConstructor && hasDestructor)
            {
                registry.Register(constructor!.ConstructorFn, destructor!.DestructorFn);
            }
            else if (hasConstructor)
            {
                registry.Register(constructor!.ConstructorFn);
            }
            else if (hasDestructor)
            {
                registry.RegisterNonConstructable(destructor!.DestructorFn);
            }
            else
            {
                registry.RegisterNonConstructable<TComponent>();
            }
        }
    }
}