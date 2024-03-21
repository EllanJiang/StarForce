/*
  作者：LTH
  文件描述：
  文件名：SnapshotExtensions
  创建时间：2023/07/15 23:07:SS
*/

using System;
using System.Reflection;
using Entt.Entities;

namespace Entt.Serialization
{
    public static class SnapshotExtensions
    {
        /// <summary>
        /// 创建快照
        /// </summary>
        /// <param name="registry"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public static SnapshotView<TEntityKey> CreateSnapshot<TEntityKey>(this EntityRegistry<TEntityKey> registry) 
            where TEntityKey : IEntityKey
        {
            return new SnapshotView<TEntityKey>(registry);
        }

        /// <summary>
        /// 创建异步快照
        /// </summary>
        /// <param name="registry"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public static AsyncSnapshotView<TEntityKey> CreateAsyncSnapshot<TEntityKey>(this IEntityPoolAccess<TEntityKey> registry)
            where TEntityKey : IEntityKey
        {
            return new AsyncSnapshotView<TEntityKey>(registry);
        }

        /// <summary>
        /// 创建快照加载器
        /// </summary>
        /// <param name="registry"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public static SnapshotLoader<TEntityKey> CreateLoader<TEntityKey>(this EntityRegistry<TEntityKey> registry)
            where TEntityKey : IEntityKey
        {
            return new SnapshotLoader<TEntityKey>(registry);
        }

        /// <summary>
        /// 反射写入方法，
        /// 返回SnapshotView中写入组件方法（WriteComponent）和写入标签方法（WriteTag）
        /// </summary>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static (MethodInfo, MethodInfo) ReflectWriteHooks<TEntityKey>() where TEntityKey : IEntityKey
        {
            var writeComponentMethod = typeof(SnapshotView<TEntityKey>).GetMethod(nameof(SnapshotView<TEntityKey>.WriteComponent),
                                                                                  BindingFlags.Instance | BindingFlags.Public,
                                                                                  null,
                                                                                  new[] { typeof(IEntityArchiveWriter<TEntityKey>) },
                                                                                  null) ??
                                       throw new InvalidOperationException($"Unable to find required public method {nameof(SnapshotView<TEntityKey>.WriteComponent)}");

            var writeTagMethod = typeof(SnapshotView<TEntityKey>).GetMethod(nameof(SnapshotView<TEntityKey>.WriteTag),
                                                                            BindingFlags.Instance | BindingFlags.Public,
                                                                            null,
                                                                            new[] { typeof(IEntityArchiveWriter<TEntityKey>) },
                                                                            null) ??
                                 throw new InvalidOperationException($"Unable to find required public method {nameof(SnapshotView<TEntityKey>.WriteTag)}");

            return (writeComponentMethod, writeTagMethod);
        }
    }
}