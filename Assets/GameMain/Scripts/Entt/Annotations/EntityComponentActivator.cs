/*
  作者：LTH
  文件描述：
  文件名：EntityComponentActivator
  创建时间：2023/07/07 21:07:SS
*/

using System;
using System.Collections.Generic;
//todo 这里应该避免使用linq
using System.Linq;
using Entt.Entities;

namespace Entt.Annotations
{
    /// <summary>
    /// Entity组件响应器
    /// </summary>
    public class EntityComponentActivator<TEntityKey> where TEntityKey:IEntityKey
    {
        private readonly List<IEntityRegistrationActivator<TEntityKey>> Activators;

        public EntityComponentActivator(params IEntityRegistrationActivator<TEntityKey>[] activators)
        {
            Activators = activators.ToList();
        }

        /// <summary>
        /// 向响应器集合中新增一个activator
        /// </summary>
        /// <param name="activator"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public EntityComponentActivator<TEntityKey> With(IEntityRegistrationActivator<TEntityKey> activator)
        {
            Activators.Add(activator ?? throw new ArgumentNullException(nameof(activator)));
            return this;
        }

        /// <summary>
        /// 响应所有activator
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="components"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void ActivateAll(IEntityComponentRegistry<TEntityKey> registry,
            IEnumerable<EntityComponentRegistration> components)
        {
            registry = registry ?? throw new ArgumentNullException(nameof(registry));
            foreach (var component in components)
            {
                foreach (var activator in Activators)
                {
                    activator.Activate(component,registry);
                }
            }
        }
        
        /// <summary>
        /// 响应单个Component的activator
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="component"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Activate(IEntityComponentRegistry<TEntityKey> registry, EntityComponentRegistration component)
        {
            registry = registry ?? throw new ArgumentNullException(nameof(registry));
            foreach (var activator in Activators)
            {
                activator.Activate(component,registry);
            }
        }
    }

    public static class EntityComponentActivator
    {
        public static EntityComponentActivator<TEntityKey> Create<TEntityKey>(
            params IEntityRegistrationActivator<TEntityKey>[] activators)
            where TEntityKey : IEntityKey
        {
            return new EntityComponentActivator<TEntityKey>(activators);
        }
    }
}