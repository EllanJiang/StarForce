/*
  作者：LTH
  文件描述：
  文件名：BinaryArchiveWriter
  创建时间：2023/07/16 20:07:SS
*/

using System;
using System.IO;
using Entt.Entities;
using Entt.Serialization.Binary.Impl;
using MessagePack;

namespace Entt.Serialization.Binary
{
    /// <summary>
    /// 二进制文档writer
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public class BinaryArchiveWriter<TEntityKey>: IEntityArchiveWriter<TEntityKey> where TEntityKey : IEntityKey
    {
        private readonly Stream writer;
        private readonly MessagePackSerializerOptions? options;
        public BinaryWriteHandlerRegistry Registry { get; }
        
        public BinaryArchiveWriter(BinaryWriteHandlerRegistry registry, 
            Stream writer, 
            MessagePackSerializerOptions? options = null)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (!writer.CanWrite)
            {
                throw new ArgumentException("The given stream must be writeable");
            }

            this.Registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.writer = writer;
            this.options = options;
        }
        
        public void WriteStartEntity(in int entityCount)
        {
            MessagePackSerializer.Serialize(writer, BinaryControlObjects.BinaryStreamState.Entities, options);
            MessagePackSerializer.Serialize(writer, entityCount, options);
        }

        public void WriteEntity(in TEntityKey entityKey)
        {
            var entityData = new EntityKeyData(entityKey.Age, entityKey.Key);
            MessagePackSerializer.Serialize(writer, entityData, options);
        }

        public void WriteEndEntity()
        {
        }

        public void WriteStartComponent<TComponent>(in int entityCount)
        {
            var handler = Registry.QueryHandler<TComponent>();
            MessagePackSerializer.Serialize(writer, BinaryControlObjects.BinaryStreamState.Component, options);
            MessagePackSerializer.Serialize(writer, new BinaryControlObjects.StartComponentRecord(entityCount, handler.TypeId));
        }

        public void WriteComponent<TComponent>(in TEntityKey entityKey, in TComponent component)
        {
            var handler = Registry.QueryHandler<TComponent>();
            var entityData = new EntityKeyData(entityKey.Age, entityKey.Key);
            MessagePackSerializer.Serialize(writer, entityData, options);
            if (handler.TryGetPreProcessor<TComponent>(out var processor))
            {
                MessagePackSerializer.Serialize(writer, processor(component), options);
            }
            else
            {
                MessagePackSerializer.Serialize(writer, component, options);
            }
        }

        public void WriteEndComponent<TComponent>()
        {
        }

        public void WriteTag<TComponent>(in TEntityKey entityKey, in TComponent c)
        {
            var handler = Registry.QueryHandler<TComponent>();
            MessagePackSerializer.Serialize(writer, BinaryControlObjects.BinaryStreamState.Tag, options);
            MessagePackSerializer.Serialize(new BinaryControlObjects.StartTagRecord(true, handler.TypeId), options);
            var entityData = new EntityKeyData(entityKey.Age, entityKey.Key);
            MessagePackSerializer.Serialize(writer, entityData, options);
            if (handler.TryGetPreProcessor<TComponent>(out var processor))
            {
                MessagePackSerializer.Serialize(writer, processor(c), options);
            }
            else
            {
                MessagePackSerializer.Serialize(writer, c, options);
            }
        }

        public void WriteMissingTag<TComponent>()
        {
            var handler = Registry.QueryHandler<TComponent>();
            MessagePackSerializer.Serialize(writer, BinaryControlObjects.BinaryStreamState.Tag, options);
            MessagePackSerializer.Serialize(new BinaryControlObjects.StartTagRecord(false, handler.TypeId), options);
        }

        public void WriteStartDestroyed(in int entityCount)
        {
            MessagePackSerializer.Serialize(writer, BinaryControlObjects.BinaryStreamState.DestroyedEntities, options);
            MessagePackSerializer.Serialize(writer, entityCount, options);
        }

        public void WriteDestroyed(in TEntityKey entityKey)
        {
            var entityData = new EntityKeyData(entityKey.Age, entityKey.Key);
            MessagePackSerializer.Serialize(writer, entityData, options);
        }

        public void WriteEndDestroyed()
        {
        }

        public void WriteEndOfFrame()
        {
            MessagePackSerializer.Serialize(writer, BinaryControlObjects.BinaryStreamState.EndOfFrame, options);
        }

        public void FlushFrame()
        {
            writer.Flush();
        }
    }
}