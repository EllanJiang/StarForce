/*
  作者：LTH
  文件描述：
  文件名：ComponentRegistrationExtensions
  创建时间：2023/07/07 22:07:SS
*/

using System;
using System.Reflection;
using Entt.Entities;

namespace Entt.Annotations.Impl
{
    public static class ComponentRegistrationExtensions
    {
        /// <summary>
        /// 没有构造函数标记
        /// </summary>
        public struct NotConstructableMarker
        {
        }
        
        /// <summary>
        /// 标志构造函数标记
        /// </summary>
        public struct FlagMarker
        {
        }
        
        /// <summary>
        /// 使用构造函数创建组件注册器
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="constructorFn"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static EntityComponentRegistration WithConstructor<TComponent>(this EntityComponentRegistration reg, Func<TComponent> constructorFn)
        {
            reg.Store(new ConstructorRegistration<TComponent>(constructorFn));
            return reg;
        }

        /// <summary>
        /// 使用析构函数创建组件注册器
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="destructorFn"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static EntityComponentRegistration WithDestructor<TEntityKey, TComponent>(this EntityComponentRegistration reg,
            Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent> destructorFn) 
            where TEntityKey : IEntityKey
        {
            reg.Store(new DestructorRegistration<TEntityKey, TComponent>(destructorFn));
            return reg;
        }

        /// <summary>
        /// 不使用构造函数创建组件注册器
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public static EntityComponentRegistration WithoutConstruction(this EntityComponentRegistration reg)
        {
            reg.Store(new NotConstructableMarker());
            return reg;
        }

        /// <summary>
        /// 使用标志构造函数类型创建组件注册器
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public static EntityComponentRegistration WithFlag(this EntityComponentRegistration reg)
        {
            reg.Store(new FlagMarker());
            return reg;
        }

        /// <summary>
        /// 判断（m表示的函数）和（returnType与parameter表示的函数）是否是同一个函数
        /// </summary>
        /// <param name="m"></param>
        /// <param name="returnType"></param>
        /// <param name="parameter"></param>
        /// <returns>只有返回类型相同，参数类型相同才返回True</returns>
        public static bool IsSameFunction(this MethodInfo m, Type returnType, params Type[] parameter)
        {
            if (m.ReturnType != returnType)
            {
                return false;
            }

            var p = m.GetParameters();
            if (p.Length != parameter.Length)
            {
                return false;
            }

            for (var i = 0; i < p.Length; i++)
            {
                var pi = p[i];
                if (pi.ParameterType != parameter[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断是否是同一个Action委托
        /// </summary>
        /// <param name="m"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static bool IsSameAction(this MethodInfo m, params Type[] parameter)
        {
            return IsSameFunction(m, typeof(void), parameter);
        }
    }
}