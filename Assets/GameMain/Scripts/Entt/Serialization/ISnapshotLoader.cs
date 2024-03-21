/*
  作者：LTH
  文件描述：
  文件名：ISnapshotLoader
  创建时间：2023/07/15 22:07:SS
*/

using Entt.Entities;

namespace Entt.Serialization
{
    /// <summary>
    /// 快照加载器接口，用于通知EntityRegistry有entity数据发生了变化
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public interface ISnapshotLoader<TEntityKey>
        where TEntityKey : IEntityKey
    {
        /// <summary>
        /// 添加entity到EntityRegistry中
        /// </summary>
        /// <param name="entity"></param>
        void OnEntity(TEntityKey entity);
        /// <summary>
        ///  添加已经销毁的entity到EntityRegistry中
        /// </summary>
        /// <param name="entity"></param>
        void OnDestroyedEntity(TEntityKey entity);
        /// <summary>
        ///  添加component到EntityRegistry中
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        /// <typeparam name="TComponent"></typeparam>
        void OnComponent<TComponent>(TEntityKey entity, in TComponent component);
        /// <summary>
        /// 添加tag到EntityRegistry中
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        /// <typeparam name="TComponent"></typeparam>
        void OnTag<TComponent>(TEntityKey entity, in TComponent component);
        /// <summary>
        /// input数据映射到EntityRegistry中？
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        TEntityKey Map(EntityKeyData input);
        /// <summary>
        /// 清除所有没有挂载任何Component的entity
        /// </summary>
        void CleanOrphans();
        /// <summary>
        /// 通知Tag被移除了
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        void OnTagRemoved<TComponent>();
    }
}