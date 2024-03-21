/*
* 文件名：IEntityPoolAccess
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/21 17:06:09
* 修改记录：
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Entt.Entities.Pools;

namespace Entt.Entities
{
    /// <summary>
    /// Entity对象池获取接口
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public interface IEntityPoolAccess<TEntityKey>:IEntityViewControl<TEntityKey>,IReadOnlyCollection<TEntityKey> where TEntityKey:IEntityKey
    {
        /// <summary>
        /// 从对象池中创建一个Entity
        /// </summary>
        /// <returns></returns>
        TEntityKey Create();
        /// <summary>
        /// 获取可读可写的对象池
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        IPool<TEntityKey, TComponent> GetWritablePool<TComponent>();
        /// <summary>
        /// 获取只读不能写的对象池
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        IReadOnlyPool<TEntityKey, TComponent> GetPool<TComponent>();
        /// <summary>
        /// 尝试从对象池中获取只读不可写的对象池
        /// </summary>
        /// <param name="pool"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        bool TryGetPool<TComponent>([MaybeNullWhen(false)] out IReadOnlyPool<TEntityKey, TComponent> pool);
        /// <summary>
        /// 尝试从对象池中获取可读可写的对象池
        /// </summary>
        /// <param name="pool"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        bool TryGetWritablePool<TComponent>([MaybeNullWhen(false)] out IPool<TEntityKey, TComponent> pool);
        /// <summary>
        /// 获取Entity当前状态
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="destroyed"></param>
        void AssureEntityState(TEntityKey entity, bool destroyed);
        /// <summary>
        /// 销毁该Entity（放回对象池）
        /// </summary>
        /// <param name="k"></param>
        void Destroy(TEntityKey k);

        /// <summary>
        /// 把Entity从池子中复制到k列表中
        /// </summary>
        /// <param name="k"></param>
        void CopyTo(List<TEntityKey> k);
        
        /// <summary>
        /// 在Entity被销毁前触发该事件
        /// </summary>
        event EventHandler<TEntityKey>? BeforeEntityDestroyed;
    }
}