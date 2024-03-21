/*
  作者：LTH
  文件描述：
  文件名：BinaryArchiveWriterExtensions
  创建时间：2023/07/16 21:07:SS
*/

using System.Linq;
using Entt.Entities;

namespace Entt.Serialization.Binary
{
    public static class BinaryArchiveWriterExtensions
    {
        /// <summary>
        /// 把快照中所有数据写入二进制文件中
        /// </summary>
        /// <param name="snapshotView"></param>
        /// <param name="writer"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public static SnapshotView<TEntityKey> WriteAll<TEntityKey>(this SnapshotView<TEntityKey> snapshotView,
            BinaryArchiveWriter<TEntityKey> writer)
            where TEntityKey : IEntityKey
        {
            snapshotView.WriteDestroyed(writer);
            snapshotView.WriteEntities(writer);

            var (writeComponent, writeTag) = SnapshotExtensions.ReflectWriteHooks<TEntityKey>();
            var parameters = new object[] { writer };
            var componentHandlers = writer.Registry.Handlers.Where(e => !e.Tag).OrderBy(e => e.TypeId).ToList();
            foreach (var r in componentHandlers)
            {
                writeComponent.MakeGenericMethod(r.TargetType).Invoke(snapshotView, parameters);
            }

            var tagHandlers = writer.Registry.Handlers.Where(e => e.Tag).OrderBy(e => e.TypeId).ToList();
            foreach (var r in tagHandlers)
            {
                writeTag.MakeGenericMethod(r.TargetType).Invoke(snapshotView, parameters);
            }

            snapshotView.WriteEndOfFrame(writer, false);
            return snapshotView;
        }
    }
}