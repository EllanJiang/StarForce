/*
  作者：LTH
  文件描述：
  文件名：EntityComponentActivatorExtensions
  创建时间：2023/07/07 21:07:SS
*/

using System.Collections.Generic;
using Entt.Annotations.Impl;
using Entt.Entities;

namespace Entt.Annotations
{
    public static class EntityComponentActivatorExtensions
    {
        /// <summary>
        /// 注册单个组件的响应器
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="componentRegistration"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static EntityRegistry<TEntity> Register<TEntity>(this EntityRegistry<TEntity> registry,
            EntityComponentRegistration componentRegistration)
            where TEntity : IEntityKey
        {
            ComponentRegistrationActivator<TEntity>.Instance.Activate(componentRegistration, registry);
            return registry;
        }
        
        /// <summary>
        /// 注册所有组件的响应器
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="componentRegistration"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static EntityRegistry<TEntity> RegisterAll<TEntity>(this EntityRegistry<TEntity> registry,
            IEnumerable<EntityComponentRegistration> componentRegistration)
            where TEntity : IEntityKey
        {
            foreach (var c in componentRegistration)
            {
                ComponentRegistrationActivator<TEntity>.Instance.Activate(c, registry);
            }

            return registry;
        }
    }
}