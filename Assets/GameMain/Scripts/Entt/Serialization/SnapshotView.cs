/*
  作者：LTH
  文件描述：
  文件名：SnapshotView
  创建时间：2023/07/15 21:07:SS
*/

using System;
using System.Collections.Generic;
using Entt.Entities;

namespace Entt.Serialization
{
    /// <summary>
    /// Entity的快照：Entity本身以及挂载在Entity身上的Components
    /// </summary>
    public class SnapshotView<TEntityKey> :IDisposable  where TEntityKey : IEntityKey
    {
        private readonly IEntityPoolAccess<TEntityKey> registry;
        private readonly List<TEntityKey> destroyedEntities;
        
        public SnapshotView(IEntityPoolAccess<TEntityKey> registry)
        {
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.destroyedEntities = new List<TEntityKey>();

            this.registry.BeforeEntityDestroyed += OnEntityDestroyed;
        }
        
        void OnEntityDestroyed(object sender, TEntityKey e)
        {
            destroyedEntities.Add(e);
        }


        /// <summary>
        /// 每帧结束时将快照写入writer中
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="forceFlush"></param>
        /// <returns></returns>
        public SnapshotView<TEntityKey> WriteEndOfFrame(IEntityArchiveWriter<TEntityKey> writer, bool forceFlush)
        {
            writer.WriteEndOfFrame();
            if (forceFlush)
            {
                writer.FlushFrame();
            }

            return this;
        }

        /// <summary>
        /// 将已经被销毁的Entity写入writer中
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public SnapshotView<TEntityKey> WriteDestroyed(IEntityArchiveWriter<TEntityKey> writer)
        {
            //开始写入
            writer.WriteStartDestroyed(destroyedEntities.Count);
            foreach (var d in destroyedEntities)
            {
                writer.WriteDestroyed(d);
            }
            //结束写入
            writer.WriteEndDestroyed();
            
            destroyedEntities.Clear();
            return this;
        }

        /// <summary>
        /// 将registry中还没有被销毁的entity全部写入writer中
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public SnapshotView<TEntityKey> WriteEntities(IEntityArchiveWriter<TEntityKey> writer)
        {
            //开始写入entity，指定要写入的entity的数量
            writer.WriteStartEntity(registry.Count);
            foreach (var d in registry)
            {
                writer.WriteEntity(d);
            }
            //结束写入
            writer.WriteEndEntity();
            return this;
        }

        /// <summary>
        /// 将指定组件类型写入writer中
        /// </summary>
        /// <param name="writer"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public SnapshotView<TEntityKey> WriteComponent<TComponent>(IEntityArchiveWriter<TEntityKey> writer)
        {
            try
            {
                var pool = registry.GetPool<TComponent>();
                //开始写入component，指定要写入的component的数量
                writer.WriteStartComponent<TComponent>(pool.Count);
                foreach (var entity in pool)
                {
                    if(pool.TryGet(entity,out var component))
                    {
                        writer.WriteComponent(entity, component);
                    }
                }
                //结束写入
                writer.WriteEndComponent<TComponent>();
            }
            catch (Exception e)
            {
                // todo  输出错误日志
                LogicShared.Logger.Error($"在写入该类型{typeof(TComponent)}的组件时发生错误，请检查！");
                throw;
            }
            return this;
        }
        
        /// <summary>
        /// 写入flag（标志）
        /// </summary>
        /// <param name="writer"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public SnapshotView<TEntityKey> WriteTag<TComponent>(IEntityArchiveWriter<TEntityKey> writer)
        {
            if (registry.TryGetTag<TComponent>(out var entity, out var tag))
            {
                writer.WriteTag(entity, tag);
            }
            else
            {
                writer.WriteMissingTag<TComponent>();
            }

            return this;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.registry.BeforeEntityDestroyed -= OnEntityDestroyed;
            }
        }
        
        ~SnapshotView()
        {
            Dispose(false);
        }
    }
}