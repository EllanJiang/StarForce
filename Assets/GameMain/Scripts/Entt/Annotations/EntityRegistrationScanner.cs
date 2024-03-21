/*
  作者：LTH
  文件描述：
  文件名：EntityRegistrationScanner
  创建时间：2023/07/07 21:07:SS
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Entt.Entities.Attributes;

namespace Entt.Annotations
{
    /// <summary>
    /// Entity注册扫描器
    /// </summary>
    public class EntityRegistrationScanner
    {
        private readonly List<IEntityRegistrationHandler> Handlers;

        public EntityRegistrationScanner(params IEntityRegistrationHandler[] handlers)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException(nameof(handlers));
            }

            Handlers = handlers.ToList();
        }
        
        /// <summary>
        /// 向扫描器中添加一个handler
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public EntityRegistrationScanner With(IEntityRegistrationHandler handler)
        {
            Register(handler);
            return this;
        }

        /// <summary>
        /// 向扫描器中添加一个handler
        /// </summary>
        /// <param name="handler"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Register(IEntityRegistrationHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Handlers.Add(handler);
        }

        /// <summary>
        /// 遍历所有程序集中所有类型，找到所有属性为EntityComponentAttribute的EntityComponent，
        /// 然后注册所有找到的EntityComponent的Handler
        /// </summary>
        /// <returns></returns>
        public List<EntityComponentRegistration> RegisterEntitiesFromAllAssemblies()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var retVal = new List<EntityComponentRegistration>();
            foreach (var assembly in allAssemblies)
            {
                retVal.AddRange(RegisterComponentsFromAssembly(assembly));
            }

            return retVal;
        }
        
        /// <summary>
        /// 遍历单个程序集中所有类型，找到所有属性为EntityComponentAttribute的EntityComponent，
        /// 然后注册所有找到的EntityComponent的Handler
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="retval"></param>
        /// <returns></returns>
        public List<EntityComponentRegistration> RegisterComponentsFromAssembly(Assembly assembly,
            List<EntityComponentRegistration>? retval = null)
        {
            retval = retval ?? new List<EntityComponentRegistration>();
            foreach (var typeInfo in assembly.DefinedTypes)
            {
                if (TryRegisterComponent(typeInfo, out var r))
                {
                    retval.Add(r);
                }
            }

            return retval;
        }
        
        /// <summary>
        /// 注册类型为TData的EntityComponent的Handler
        /// </summary>
        /// <param name="result"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public bool TryRegisterComponent<TData>([MaybeNullWhen(false)] out EntityComponentRegistration result)
        {
            var type = typeof(TData);
            var typeInfo = type.GetTypeInfo();
            return TryRegisterComponent(typeInfo, out result);
        }
        
        /// <summary>
        /// 注册类型为typeInfo的EntityComponent的Handler
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryRegisterComponent(TypeInfo typeInfo, [MaybeNullWhen(false)] out EntityComponentRegistration result)
        {
            if (typeInfo.IsAbstract)
            {
                result = default;
                return false;
            }

            if (!typeInfo.IsDefined(typeof(EntityComponentAttribute)))
            {
                result = default;
                return false;
            }

            result = new EntityComponentRegistration(typeInfo);
            foreach (var handler in Handlers)
            {
                handler.Process(result);
            }

            return !result.IsEmpty;
        }

        /// <summary>
        /// 注册类型为TData的EntityKey的Handler
        /// </summary>
        /// <param name="result"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public bool TryRegisterKey<TData>([MaybeNullWhen(false)] out EntityComponentRegistration result)
        {
            var type = typeof(TData);
            var typeInfo = type.GetTypeInfo();
            return TryRegisterKey(typeInfo, out result);
        }
        
        /// <summary>
        /// 注册类型为typeInfo的EntityKey的Handler
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryRegisterKey(TypeInfo typeInfo, [MaybeNullWhen(false)] out EntityComponentRegistration result)
        {
            if (typeInfo.IsAbstract)
            {
                result = default;
                return false;
            }

            if (!typeInfo.IsDefined(typeof(EntityKeyAttribute)))
            {
                result = default;
                return false;
            }

            result = new EntityComponentRegistration(typeInfo);
            foreach (var handler in Handlers)
            {
                handler.Process(result);
            }

            return !result.IsEmpty;
        }
        
    }
}