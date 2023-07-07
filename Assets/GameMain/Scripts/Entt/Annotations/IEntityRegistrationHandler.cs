/*
  作者：LTH
  文件描述：
  文件名：IEntityRegistrationHandler
  创建时间：2023/07/07 18:07:SS
*/

using Entt.Entities;

namespace Entt.Annotations
{
    /// <summary>
    /// Entity注册接口
    /// </summary>
    public interface IEntityRegistrationHandler
    {
        /// <summary>
        /// 处理组件注册
        /// </summary>
        /// <param name="registration"></param>
        void Process(EntityComponentRegistration registration);
    }

    /// <summary>
    /// Entity响应接口
    /// </summary>
    /// <typeparam name="TEntityKet"></typeparam>
    public interface IEntityRegistrationActivator<TEntityKet> where TEntityKet : IEntityKey
    {
        /// <summary>
        /// 响应Entity身上的组件
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="registry"></param>
        void Activate(EntityComponentRegistration registration, IEntityComponentRegistry<TEntityKet> registry);
    }
}