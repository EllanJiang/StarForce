/*
* 文件名：IReadOnlyPool
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/20 15:13:28
* 修改记录：
*/
#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Entt.Entities.Helpers;

namespace Entt.Entities.Pools
{
    /// <summary>
    /// 只读Entity对象池，只有一个泛型参数：EntityKey
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public interface IReadOnlyPool<TEntityKey> : IEnumerable<TEntityKey> where TEntityKey:IEntityKey
    {
        /// <summary>
        /// Entity创建事件
        /// </summary>
        event EventHandler<TEntityKey> Created;

        /// <summary>
        /// Entity销毁事件
        /// </summary>
        event EventHandler<TEntityKey> Destroyed;

        /// <summary>
        /// 池中是否包含指定Entity
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        bool Contains(TEntityKey k);
        
        /// <summary>
        /// 池中Entity总数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 创建池时预定池子的容量
        /// </summary>
        /// <param name="capacity"></param>
        void Reserve(int capacity);

        /// <summary>
        /// 把池子中所有Entity复制到指定列表中
        /// </summary>
        /// <param name="entities"></param>
        void CopyTo(RawList<TEntityKey> entities);
    }
    
    /// <summary>
    /// 只读Entity对象池，有两个泛型参数，多了一个组件：TComponent
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    public interface IReadOnlyPool<TEntityKey, TComponent> : IReadOnlyPool<TEntityKey> where TEntityKey : IEntityKey
    {
        /// <summary>
        /// 尝试根据EntityKey从对象池中取出该EntityKey对应的组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        bool TryGet(TEntityKey entity, [MaybeNullWhen(false)] out TComponent component);
        
        /// <summary>
        ///  尝试根据EntityKey从对象池中取出该EntityKey对应的组件的引用
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="defaultValue"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        ref readonly TComponent? TryGetRef(TEntityKey entity, ref TComponent? defaultValue, out bool success);
    }
}