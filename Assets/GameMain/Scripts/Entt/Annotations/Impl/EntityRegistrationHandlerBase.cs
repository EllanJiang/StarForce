/*
  作者：LTH
  文件描述：
  文件名：EntityRegistrationHandlerBase
  创建时间：2023/07/07 22:07:SS
*/

using System;
using System.Reflection;

namespace Entt.Annotations.Impl
{
    public abstract class EntityRegistrationHandlerBase:IEntityRegistrationHandler
    {
        private static readonly MethodInfo ProcessMethodInfo;


        static EntityRegistrationHandlerBase()
        {
            var method = typeof(EntityRegistrationHandlerBase).GetMethod(nameof(ProcessTypedInternal),
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(EntityComponentRegistration) },
                null);
            if (method == null)
            {
                throw new InvalidOperationException(" Unable to find private generic method. That really should not happen.");
            }

            ProcessMethodInfo = method;
        }
        /// <summary>
        /// 通过反射调用process方法
        /// </summary>
        /// <param name="registration"></param>
        public void Process(EntityComponentRegistration registration)
        {
            var typeInfo = registration.TypeInfo;
            var genericMethod = ProcessMethodInfo.MakeGenericMethod(typeInfo);
            genericMethod.Invoke(this, new object[] { registration });
        }

        void ProcessTypedInternal<TComponent>(EntityComponentRegistration registration)
        {
            ProcessTyped<TComponent>(registration);
        }
        
        /// <summary>
        /// 自定义处理类型的方法，需要进行重写
        /// </summary>
        /// <param name="registration"></param>
        /// <typeparam name="TComponent"></typeparam>
        protected abstract void ProcessTyped<TComponent>(EntityComponentRegistration registration);
    }
}