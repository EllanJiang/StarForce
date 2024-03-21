/*
* 文件名：IEntityView
* 文件描述：This is a read-only view over an component pool.
* 作者：aronliang
* 创建时间：2023/06/21 17:35:20
* 修改记录：
*/

using System;
using System.Collections.Generic;
using Entt.Entities.Helpers;

namespace Entt.Entities
{
    /// <summary>
    /// This is a read-only view over an component pool.
    /// </summary>
    public interface IEntityView<TEntityKey> : IEnumerable<TEntityKey>, IEntityViewControl<TEntityKey>, IDisposable
        where TEntityKey : IEntityKey

    {
        /// <summary>
        /// 销毁Entity后会触发该事件
        /// </summary>
        event EventHandler<TEntityKey> Destroyed;
        /// <summary>
        /// 创建Entity后会触发该事件
        /// </summary>
        event EventHandler<TEntityKey> Created;
        
        /// <summary>
        /// 是否允许并行处理
        /// </summary>
        bool AllowParallelExecution { get; set; }
        
        /// <summary>
        /// 触发ViewDelegates.Apply委托
        /// </summary>
        /// <param name="bulk"></param>
        void Apply(ViewDelegates.Apply<TEntityKey> bulk);
        /// <summary>
        /// 触发ViewDelegates.ApplyWithContext委托
        /// </summary>
        /// <param name="t"></param>
        /// <param name="bulk"></param>
        /// <typeparam name="TContext"></typeparam>
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TEntityKey, TContext> bulk);
        
        /// <summary>
        /// todo 这个方法用来干什么？
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        void Respect<TComponent>();
        /// <summary>
        /// 设置容器容量为capacity
        /// </summary>
        /// <param name="capacity"></param>
        void Reserve(int capacity);

        /// <summary>
        /// 容器预估大小
        /// </summary>
        int EstimatedSize { get; }

        /// <summary>
        /// 把容器内的所有元素复制到k列表中
        /// </summary>
        /// <param name="k"></param>
        void CopyTo(RawList<TEntityKey> k);
    }
 
}