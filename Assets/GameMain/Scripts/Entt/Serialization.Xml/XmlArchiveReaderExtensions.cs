/*
  作者：LTH
  文件描述：
  文件名：XmlArchiveReaderExtensions
  创建时间：2023/07/16 17:07:SS
*/

using System;
using System.Linq;
using System.Reflection;
using Entt.Entities;

namespace Entt.Serialization.Xml
{
    /// <summary>
    /// xml文档reader拓展方法
    /// </summary>
    public static class XmlArchiveReaderExtensions
    {
        /// <summary>
        /// 反射ReadComponent和ReadTag方法
        /// </summary>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns>返回两个方法，第一个是readComponentMethod，第二个是readTagMethod</returns>
        /// <exception cref="InvalidOperationException"></exception>
        static (MethodInfo, MethodInfo) ReflectXmlArchiveReaderExtensions<TEntityKey>() where TEntityKey : IEntityKey
        {
            var readComponentMethod = typeof(SnapshotStreamReader<TEntityKey>).GetMethod(nameof(SnapshotStreamReader<TEntityKey>.ReadComponent),
                                                                                        BindingFlags.Instance | BindingFlags.Public,
                                                                                        null,
                                                                                        new[] { typeof(IEntityArchiveReader<TEntityKey>) },
                                                                                        null) ??
                                  throw new InvalidOperationException($"Unable to find required public method {nameof(SnapshotStreamReader<TEntityKey>.ReadComponent)}");

            var readTagMethod = typeof(SnapshotStreamReader<TEntityKey>).GetMethod(nameof(SnapshotStreamReader<TEntityKey>.ReadTag),
                                                                                   BindingFlags.Instance | BindingFlags.Public,
                                                                                   null,
                                                                                   new[] { typeof(IEntityArchiveReader<TEntityKey>) },
                                                                                   null) ??
                            throw new InvalidOperationException($"Unable to find required public method {nameof(SnapshotStreamReader<TEntityKey>.ReadTag)}");
            return (readComponentMethod, readTagMethod);
        }

        /// <summary>
        /// 从reader中读取所有数据到EntityRegistry中
        /// </summary>
        /// <param name="snapshotStreamReader"></param>
        /// <param name="reader"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public static SnapshotStreamReader<TEntityKey> ReadAll<TEntityKey>(this SnapshotStreamReader<TEntityKey> snapshotStreamReader, XmlEntityArchiveReader<TEntityKey> reader) 
            where TEntityKey : IEntityKey
        {
            snapshotStreamReader.ReadDestroyed(reader);
            snapshotStreamReader.ReadEntities(reader);

            var (readComponent, readTag) = ReflectXmlArchiveReaderExtensions<TEntityKey>();
            //所有非tag的handler
            var parameters = new object[] { reader };
            var handlerRegistrations = reader.Registry.Handlers.Where(e => !e.Tag).OrderBy(e => e.TypeId);
            foreach (var r in handlerRegistrations)
            {
                readComponent.MakeGenericMethod(r.TargetType).Invoke(snapshotStreamReader, parameters);
            }

            //所有tag的handler
            var tagRegistrations = reader.Registry.Handlers.Where(e => e.Tag).OrderBy(e => e.TypeId);
            foreach (var r in tagRegistrations)
            {
                readTag.MakeGenericMethod(r.TargetType).Invoke(snapshotStreamReader, parameters);
            }
            return snapshotStreamReader;
        }
    }
}