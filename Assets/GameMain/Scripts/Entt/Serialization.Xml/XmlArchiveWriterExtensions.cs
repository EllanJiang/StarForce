/*
  作者：LTH
  文件描述：
  文件名：XmlArchiveWriterExtensions
  创建时间：2023/07/16 18:07:SS
*/

using System.Linq;
using Entt.Entities;

namespace Entt.Serialization.Xml
{
    public static class XmlArchiveWriterExtensions
    {
        
        /// <summary>
        /// 把快照中所有数据写入writer中（但没有创建新的xml文档，而是在之前的基础上继续写入）
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="writer"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public static SnapshotView<TEntityKey> WriteAllAsFragment<TEntityKey>(this SnapshotView<TEntityKey> snapshot,
                                                                              XmlArchiveWriter<TEntityKey> writer)
            where TEntityKey : IEntityKey
        {
            snapshot.WriteDestroyed(writer);
            snapshot.WriteEntities(writer);

            var (writeComponent, writeTag) = SnapshotExtensions.ReflectWriteHooks<TEntityKey>();
            var parameters = new object[] {writer};
            var handlerRegistrations = writer.Registry.Handlers.Where(e => !e.Tag).OrderBy(e => e.TypeId);
            foreach (var r in handlerRegistrations)
            {
                writeComponent.MakeGenericMethod(r.TargetType).Invoke(snapshot, parameters);
            }

            var tagRegistrations = writer.Registry.Handlers.Where(e => e.Tag).OrderBy(e => e.TypeId);
            foreach (var r in tagRegistrations)
            {
                writeTag.MakeGenericMethod(r.TargetType).Invoke(snapshot, parameters);
            }

            return snapshot;
        }

        /// <summary>
        /// 创建新的xml文档，然后把快照中所有数据写入xml writer中
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="writer"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public static SnapshotView<TEntityKey> WriteAll<TEntityKey>(this SnapshotView<TEntityKey> snapshot,
                                                                    XmlArchiveWriter<TEntityKey> writer)
            where TEntityKey : IEntityKey
        {
            writer.WriteDefaultSnapshotDocumentHeader();        //创建xml文档

            snapshot.WriteDestroyed(writer);
            snapshot.WriteEntities(writer);

            var (writeComponent, writeTag) = SnapshotExtensions.ReflectWriteHooks<TEntityKey>();
            var parameters = new object[] {writer};
            var handlerRegistrations = writer.Registry.Handlers.Where(e => !e.Tag).OrderBy(e => e.TypeId);
            foreach (var r in handlerRegistrations)
            {
                writeComponent.MakeGenericMethod(r.TargetType).Invoke(snapshot, parameters);
            }

            var tagRegistrations = writer.Registry.Handlers.Where(e => e.Tag).OrderBy(e => e.TypeId);
            foreach (var r in tagRegistrations)
            {
                writeTag.MakeGenericMethod(r.TargetType).Invoke(snapshot, parameters);
            }

            writer.WriteDefaultSnapshotDocumentFooter();
            return snapshot;
        }
    }
}