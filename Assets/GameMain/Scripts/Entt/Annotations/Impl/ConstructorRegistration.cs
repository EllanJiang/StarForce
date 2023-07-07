/*
  作者：LTH
  文件描述：
  文件名：ConstructorRegistration
  创建时间：2023/07/07 22:07:SS
*/

using System;

namespace Entt.Annotations.Impl
{
    /// <summary>
    /// 注册组件构造函数
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    public class ConstructorRegistration<TComponent>
    {
        public readonly Func<TComponent> ConstructorFn;

        public ConstructorRegistration(Func<TComponent> constructorFn)
        {
            this.ConstructorFn = constructorFn;
        }
    }
}