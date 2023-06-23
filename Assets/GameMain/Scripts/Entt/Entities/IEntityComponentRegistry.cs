/*
  作者：LTH
  文件描述：
  文件名：IEntityComponentRegistry
  创建时间：2023/06/23 21:06:SS
*/

using System;

namespace Entt.Entities
{
    /// <summary>
    /// Entity组件注册接口
    /// </summary>
    public interface IEntityComponentRegistry<TEntityKey> where TEntityKey : IEntityKey
    {
        /// <summary>
        /// 注册组件标志
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        void RegisterFlag<TComponent>();
        
        /// <summary>
        /// 注册组件，需要提供组件构造函数和析构函数
        /// </summary>
        /// <param name="constructorFn"></param>
        /// <param name="destructorFn"></param>
        /// <typeparam name="TComponent"></typeparam>
        void Register<TComponent>(Func<TComponent> constructorFn,
            Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent>? destructorFn = null);
        
        /// <summary>
        /// 注册组件，只需要提供组件析构函数
        /// </summary>
        /// <param name="destructorFn"></param>
        /// <typeparam name="TComponent"></typeparam>
        void RegisterNonConstructable<TComponent>(Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent>? destructorFn = null);
    }
}