/*
  作者：LTH
  文件描述：
  文件名：EntityActivatorBase
  创建时间：2023/07/07 22:07:SS
*/

using System;
using System.Reflection;
using Entt.Entities;

namespace Entt.Annotations.Impl
{
    /// <summary>
    /// Entity响应器基类
    /// </summary>
    public abstract class EntityActivatorBase<TEntityKey> : IEntityRegistrationActivator<TEntityKey> where TEntityKey : IEntityKey
    {
        private static readonly MethodInfo ProcessMethodInfo;

        static EntityActivatorBase()
        {
            //使用反射创建类型为EntityComponentRegistration和IEntityComponentRegistry<TEntityKey>的MethodInfo
            var method = typeof(EntityActivatorBase<TEntityKey>).GetMethod(nameof(ProcessTypedInternal),
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[] { typeof(EntityComponentRegistration), typeof(IEntityComponentRegistry<TEntityKey>) },
                null);
            
            if (method == null)
            {
                throw new InvalidOperationException("Unable to find private generic method. That really should not happen.");
            }

            ProcessMethodInfo = method;
        }

        /// <summary>
        /// 通过反射响应
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="registry"></param>
        public void Activate(EntityComponentRegistration registration, IEntityComponentRegistry<TEntityKey> registry)
        {
            var typeInfo = registration.TypeInfo;
            var genericMethod = ProcessMethodInfo.MakeGenericMethod(typeInfo);
            genericMethod.Invoke(this, new object[] { registration, registry });
        }

        void ProcessTypedInternal<TComponent>(EntityComponentRegistration registration,
            IEntityComponentRegistry<TEntityKey> registry)
        {
            ProcessTyped<TComponent>(registration, registry);
        }

        /// <summary>
        /// 处理类型抽象方法
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="registry"></param>
        /// <typeparam name="TComponent"></typeparam>
        protected abstract void ProcessTyped<TComponent>(EntityComponentRegistration registration,
            IEntityComponentRegistry<TEntityKey> registry);
    }
}