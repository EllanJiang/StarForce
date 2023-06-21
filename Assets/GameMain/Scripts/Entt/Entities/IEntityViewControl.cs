/*
* 文件名：IEntityViewControl
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/21 17:07:52
* 修改记录：
*/

using System.Diagnostics.CodeAnalysis;

namespace Entt.Entities
{
    /// <summary>
    /// Entity相关的控制接口
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public interface IEntityViewControl<TEntityKey> where TEntityKey: IEntityKey
    {
        /// <summary>
        /// 该Entity是否拥有该标签（Tag）
        /// </summary>
        /// <typeparam name="TTag"></typeparam>
        /// <returns></returns>
        bool HasTag<TTag>();
        
        /// <summary>
        /// 尝试获取该Entity的标签
        /// </summary>
        /// <param name="k"></param>
        /// <param name="tag"></param>
        /// <typeparam name="TTag"></typeparam>
        /// <returns></returns>
        bool TryGetTag<TTag>([MaybeNullWhen(false)] out TEntityKey k, [MaybeNullWhen(false)] out TTag tag);
        
        /// <summary>
        /// 移除该Entity的标签
        /// </summary>
        /// <typeparam name="TTag"></typeparam>
        void RemoveTag<TTag>();
        /// <summary>
        /// 给Entity添加默认标签
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TTag"></typeparam>
        void AttachTag<TTag>(TEntityKey entity);
        /// <summary>
        /// 给该Entity添加指定标签
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tag"></param>
        /// <typeparam name="TTag"></typeparam>
        void AttachTag<TTag>(TEntityKey entity, in TTag tag);
        
        /// <summary>
        /// 当前World中是否包含该EntityKey
        /// </summary>
        /// <param name="entityKey"></param>
        /// <returns></returns>
        bool Contains(TEntityKey entityKey);
        /// <summary>
        /// 获取该Entity的Component
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="data"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        bool GetComponent<TComponent>(TEntityKey entity, [MaybeNullWhen(false)] out TComponent data);
        /// <summary>
        /// 判断该Entity身上是否有指定Component
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        bool HasComponent<TComponent>(TEntityKey entity);
        
        /// <summary>
        /// 更新该Entity身上的Component
        /// Writeback is only necessary for structs. Classes are by-ref and all references point to the same instance anyway.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="data"></param>
        /// <typeparam name="TComponent"></typeparam>
        void WriteBack<TComponent>(TEntityKey entity, in TComponent data);
        /// <summary>
        /// 移除Entity身上指定的Component
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TComponent"></typeparam>
        void RemoveComponent<TComponent>(TEntityKey entity);
        
        /// <summary>
        /// 添加指定Component到该Entity身上
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        TComponent AssignComponent<TComponent>(TEntityKey entity);
        /// <summary>
        /// 添加指定Component到该Entity身上
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="c"></param>
        /// <typeparam name="TComponent"></typeparam>
        void AssignComponent<TComponent>(TEntityKey entity, in TComponent c);
        
        /// <summary>
        /// 添加或更新该Entity身上的Component
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        TComponent AssignOrReplace<TComponent>(TEntityKey entity);
        /// <summary>
        /// 添加或更新该Entity身上的Component
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="c"></param>
        /// <typeparam name="TComponent"></typeparam>
        void AssignOrReplace<TComponent>(TEntityKey entity, in TComponent c);
        /// <summary>
        /// 替换该Entity身上的Component
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="c"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        bool ReplaceComponent<TComponent>(TEntityKey entity, in TComponent c);
        
        /// <summary>
        /// 判断当前World中该Entity是否还在生效
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsValid(TEntityKey entity);
        /// <summary>
        /// 重置该Entity
        /// </summary>
        /// <param name="entity"></param>
        void Reset(TEntityKey entity);
        /// <summary>
        /// 是否是单例Entity？如果是用来判断是否是单例Entity，是否改成IsSingleton更合适？
        /// Orphan是孤儿的意思
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsOrphan(TEntityKey entity);
    }
}