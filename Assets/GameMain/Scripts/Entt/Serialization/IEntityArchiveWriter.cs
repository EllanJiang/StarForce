/*
  作者：LTH
  文件描述：
  文件名：IEntityArchiveWriter
  创建时间：2023/07/15 21:07:SS
*/

using Entt.Entities;

namespace Entt.Serialization
{
    /// <summary>
    /// 保存Entity数据
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public interface IEntityArchiveWriter<TEntityKey> where TEntityKey : IEntityKey
    {
        /// <summary>
        /// 开始写入entity
        /// </summary>
        /// <param name="entityCount">entity数量</param>
        void WriteStartEntity(in int entityCount);
        /// <summary>
        /// 正式写入entity
        /// </summary>
        /// <param name="entityKey"></param>
        void WriteEntity(in TEntityKey entityKey);
        /// <summary>
        /// 结束写入entity
        /// </summary>
        void WriteEndEntity();

        /// <summary>
        /// 开始写入指定类型的组件
        /// </summary>
        /// <param name="entityCount"></param>
        /// <typeparam name="TComponent"></typeparam>
        void WriteStartComponent<TComponent>(in int entityCount);
        /// <summary>
        /// 正式写入组件
        /// </summary>
        /// <param name="entityKey"></param>
        /// <param name="component"></param>
        /// <typeparam name="TComponent"></typeparam>
        void WriteComponent<TComponent>(in TEntityKey entityKey, in TComponent component);
        /// <summary>
        /// 结束写入组件
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        void WriteEndComponent<TComponent>();

        /// <summary>
        /// 写入flag（标志）
        /// </summary>
        /// <param name="entityKey"></param>
        /// <param name="component"></param>
        /// <typeparam name="TComponent"></typeparam>
        void WriteTag<TComponent>(in TEntityKey entityKey, in TComponent component);
        /// <summary>
        /// flag丢失了，写入丢失信息
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        void WriteMissingTag<TComponent>();

        /// <summary>
        /// 开始写入已经被销毁的entity
        /// </summary>
        /// <param name="entityCount"></param>
        void WriteStartDestroyed(in int entityCount);
        /// <summary>
        /// 正式写入
        /// </summary>
        /// <param name="entityKey"></param>
        void WriteDestroyed(in TEntityKey entityKey);
        /// <summary>
        /// 结束写入
        /// </summary>
        void WriteEndDestroyed();

        /// <summary>
        /// 每帧结束时将快照写入writer中
        /// </summary>
        void WriteEndOfFrame();
        /// <summary>
        /// 刷新帧数据
        /// </summary>
        void FlushFrame();
    }
}