/*
* 文件名：IPool
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/20 15:12:43
* 修改记录：
*/
#nullable enable
//加这句的目的是使用TComponent?时出现如下警告：
//The annotation for nullable reference types should only be used in code within a '#nullable' context
// 具体参考 https://stackoverflow.com/questions/55492214/the-annotation-for-nullable-reference-types-should-only-be-used-in-code-within-a


using System;
using System.Collections.Generic;

namespace Entt.Entities.Pools
{
    /// <summary>
    /// Entity对象池接口，只有一个泛型参数TEntityKey
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public interface IPool<TEntityKey> :IReadOnlyPool<TEntityKey> where TEntityKey : IEntityKey
    {
        /// <summary>
        /// todo 使用其他池子来初始化？
        /// </summary>
        /// <param name="otherSet"></param>
        void Respect(IEnumerable<TEntityKey> otherSet);

        /// <summary>
        /// 从池子中移除指定的Entity
        /// </summary>
        /// <param name="entityKey"></param>
        /// <returns></returns>
        bool Remove(TEntityKey entityKey);

        /// <summary>
        /// 移除池子中所有Entity
        /// </summary>
        void RemoveAll();
    }
    
    /// <summary>
    /// Entity对象池接口，有两个泛型参数，除了TEntityKey，还多了一个TComponent
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    public interface IPool<TEntityKey, TComponent>: IReadOnlyPool<TEntityKey, TComponent>, IPool<TEntityKey> where TEntityKey : IEntityKey
    {
        event EventHandler<(TEntityKey key, TComponent old)>? DestroyedNotify;
        event EventHandler<(TEntityKey key, TComponent old)>? Updated;

        void Add(TEntityKey entityKey, in TComponent component);
        bool WriteBack(TEntityKey entityKey, in TComponent component);
        ref TComponent? TryGetModifiableRef(TEntityKey entity, ref TComponent? defaultValue, out bool success);
    }
}