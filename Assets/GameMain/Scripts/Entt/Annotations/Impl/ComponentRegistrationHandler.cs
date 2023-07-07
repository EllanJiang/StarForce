/*
  作者：LTH
  文件描述：
  文件名：ComponentRegistrationHandler
  创建时间：2023/07/07 22:07:SS
*/

using System;
using System.Reflection;
using Entt.Entities;
using Entt.Entities.Attributes;
using Serilog;

namespace Entt.Annotations.Impl
{
    /// <summary>
    /// 注册组件handler
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public class ComponentRegistrationHandler<TEntityKey> : EntityRegistrationHandlerBase where TEntityKey : IEntityKey
    {
        static readonly ILogger Logger = Log.ForContext<ComponentRegistrationHandler<TEntityKey>>();
        
        protected override void ProcessTyped<TComponent>(EntityComponentRegistration registration)
        {
            if (HasDefaultConstructor<TComponent>())
            {
                if (IsFlag<TComponent>())
                {
                    Logger.Debug("Type {ComponentType} has default constructor", typeof(TComponent));
                    registration.WithFlag();
                }
                else
                {
                    Logger.Debug("Type {ComponentType} has default constructor", typeof(TComponent));
                    registration.WithConstructor(Activator.CreateInstance<TComponent>);
                }
            }
            else
            {
                registration.WithoutConstruction();
            }
            
            var handlerMethods = typeof(TComponent).GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (var m in handlerMethods)
            {
                if (IsDestructor<TComponent>(m))
                {
                    var d = (Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent>)
                        Delegate.CreateDelegate(typeof(Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent>), m);
                    registration.WithDestructor(d);
                }
            }
        }

        /// <summary>
        /// 该组件是否是标志组件
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        bool IsFlag<TComponent>()
        {
            var componentType = typeof(TComponent);
            var attr = componentType.GetCustomAttribute<EntityComponentAttribute>();
            if (attr == null)
            {
                return false;
            }

            return attr.Constructor == EntityConstructor.Flag;
        }
        
        /// <summary>
        /// 判断该类型的组件是否有默认构造函数
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        bool HasDefaultConstructor<TComponent>()
        {
            var componentType = typeof(TComponent);
            var attr = componentType.GetCustomAttribute<EntityComponentAttribute>();
            if (attr == null)
            {
                return false;
            }

            if (attr.Constructor == EntityConstructor.NonConstructable)
            {
                Logger.Debug("Type {ComponentType} opted out of using a default constructor", typeof(TComponent));
                return false;
            }

            //是值类型，大部分是结构体
            if (componentType.IsValueType)
            {
                // Value types that have at least one other constructor are not considered default constructable.
                if (componentType.GetConstructors().Length == 0)
                {
                    Logger.Verbose("Type {ComponentType} is a value type without user constructors", typeof(TComponent));
                    return true;
                }
                
                Logger.Debug("Type {ComponentType} is a value type with user constructors. This type is considered NonConstructable", typeof(TComponent));
                return false;
            }

            var c = componentType.GetConstructor(Type.EmptyTypes);
            if (c != null)
            {
                return true;
            }

            Logger.Debug("Type {ComponentType} has no default constructor", typeof(TComponent));
            return false;

        }

        /// <summary>
        /// 是否是析构函数
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <typeparam name="TComponentType"></typeparam>
        /// <returns></returns>
        bool IsDestructor<TComponentType>(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute<EntityDestructorAttribute>() != null 
                   && methodInfo.IsStatic
                   && methodInfo.IsSameAction(typeof(TEntityKey), 
                                              typeof(IEntityViewControl<TEntityKey>), 
                                              typeof(TComponentType));
        }
    }
}