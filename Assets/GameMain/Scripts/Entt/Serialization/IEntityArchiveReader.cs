/*
  作者：LTH
  文件描述：
  文件名：IEntityArchiveReader
  创建时间：2023/07/15 21:07:SS
*/

using Entt.Entities;

namespace Entt.Serialization
{
    /// <summary>
    /// 读取Entity数据
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public interface IEntityArchiveReader<TEntityKey> 
        where TEntityKey: IEntityKey
    {
        /// <summary>
        /// 要读取的Entity的数量
        /// </summary>
        /// <returns></returns>
        int ReadEntityCount();
        /// <summary>
        /// 将entity数据读取到entityMapper中
        /// </summary>
        /// <param name="entityMapper"></param>
        /// <returns></returns>
        TEntityKey ReadEntity(IEntityKeyMapper entityMapper);

        /// <summary>
        /// 要读取的component的数量
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        int ReadComponentCount<TComponent>();
        /// <summary>
        /// 将component数据读取到entityMapper中
        /// </summary>
        /// <param name="entityMapper"></param>
        /// <param name="key"></param>
        /// <param name="component"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        bool TryReadComponent<TComponent>(IEntityKeyMapper entityMapper, out TEntityKey key, out TComponent component);

        /// <summary>
        /// 读取flag（标志）
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        bool ReadTagFlag<TComponent>();
        /// <summary>
        /// 尝试读取tag，如果读取成功，那么返回该tag所属的Entity和component
        /// </summary>
        /// <param name="entityMapper"></param>
        /// <param name="entityKey"></param>
        /// <param name="component"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        bool TryReadTag<TComponent>(IEntityKeyMapper entityMapper, out TEntityKey entityKey, out TComponent component);

        /// <summary>
        /// 读取已经被销毁的Entity的数量
        /// </summary>
        /// <returns></returns>
        int ReadDestroyedCount();
        /// <summary>
        /// 读取已经被销毁的entity到entityMapper中
        /// </summary>
        /// <param name="entityMapper"></param>
        /// <returns></returns>
        TEntityKey ReadDestroyed(IEntityKeyMapper entityMapper);
    }
}