/*
  作者：LTH
  文件描述：
  文件名：DestructorRegistration
  创建时间：2023/07/07 22:07:SS
*/

using System;
using Entt.Entities;

namespace Entt.Annotations.Impl
{
    /// <summary>
    /// 注册组件析构函数
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    class DestructorRegistration<TEntityKey, TComponent> where TEntityKey : IEntityKey
    {
        public readonly Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent> DestructorFn;

        public DestructorRegistration(Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent> destructorFn)
        {
            this.DestructorFn = destructorFn;
        }
    }
}