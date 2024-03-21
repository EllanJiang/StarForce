/*
  作者：LTH
  文件描述：
  文件名：BinaryEntityArchiveReader
  创建时间：2023/07/16 21:07:SS
*/

using System.IO;
using Entt.Entities;
using Entt.Serialization.Binary.Impl;
using MessagePack;

namespace Entt.Serialization.Binary
{
    /// <summary>
    /// 二进制entity数据reader
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public class BinaryEntityArchiveReader<TEntityKey>: IEntityArchiveReader<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly MessagePackSerializerOptions? options;
        readonly BinaryReaderBackend<TEntityKey> registry;
        readonly Stream reader;
        BinaryReadHandlerRegistration readHandlerRegistration;

        public BinaryEntityArchiveReader(BinaryReaderBackend<TEntityKey> registry, 
            Stream reader,
            MessagePackSerializerOptions? optionsRaw = null)
        {
            this.registry = registry;
            this.reader = reader;
            this.options = optionsRaw;
        }

        public int ReadEntityCount()
        {
            var recordType = MessagePackSerializer.Deserialize<BinaryControlObjects.BinaryStreamState>(reader, options);
            if (recordType != BinaryControlObjects.BinaryStreamState.Entities)
            {
                throw new BinaryReaderException("Invalid stream state: Expected Entity-Start record");
            }

            return MessagePackSerializer.Deserialize<int>(reader, options);
        }
        
        public TEntityKey ReadEntity(IEntityKeyMapper entityMapper)
        {
            var entityKeyData = MessagePackSerializer.Deserialize<EntityKeyData>(reader, options);
            return entityMapper.EntityKeyMapper<TEntityKey>(entityKeyData);
        }
        
        public int ReadComponentCount<TComponent>()
        {
            var recordType = MessagePackSerializer.Deserialize<BinaryControlObjects.BinaryStreamState>(reader, options);
            if (recordType != BinaryControlObjects.BinaryStreamState.Component)
            {
                throw new BinaryReaderException("Invalid stream state: Expected Component-Start record");
            }

            var startComponentRecord = MessagePackSerializer.Deserialize<BinaryControlObjects.StartComponentRecord>(reader, options);
            if (!registry.Registry.TryGetValue(startComponentRecord.ComponentId, out readHandlerRegistration))
            {
                throw new BinaryReaderException($"Invalid stream state: No handler for component type {startComponentRecord.ComponentId}");
            }

            return startComponentRecord.ComponentCount;
        }
        
        public bool TryReadComponent<TComponent>(IEntityKeyMapper entityMapper, 
            out TEntityKey key, out TComponent component)
        {

            var entityKey = MessagePackSerializer.Deserialize<EntityKeyData>(reader, options);
            component = MessagePackSerializer.Deserialize<TComponent>(reader, options);
            if (readHandlerRegistration.TryGetPostProcessor<TComponent>(out var pp))
            {
                component = pp(in component);
            }

            key = entityMapper.EntityKeyMapper<TEntityKey>(entityKey);
            return true;
        }
        
        public bool ReadTagFlag<TComponent>()
        {
            var recordType = MessagePackSerializer.Deserialize<BinaryControlObjects.BinaryStreamState>(reader, options);
            if (recordType != BinaryControlObjects.BinaryStreamState.Tag)
            {
                throw new BinaryReaderException("Invalid stream state: Expected Tag-Start record");
            }

            var startTagRecord = MessagePackSerializer.Deserialize<BinaryControlObjects.StartTagRecord>(reader, options);
            if (!registry.Registry.TryGetValue(startTagRecord.ComponentId, out readHandlerRegistration))
            {
                throw new BinaryReaderException($"Invalid stream state: No handler for component type {startTagRecord.ComponentId}");
            }

            return startTagRecord.ComponentExists;
        }

        public bool TryReadTag<TComponent>(IEntityKeyMapper entityMapper, out TEntityKey key, out TComponent component)
        {
            var entityKey = MessagePackSerializer.Deserialize<EntityKeyData>(reader, options);
            component = MessagePackSerializer.Deserialize<TComponent>(reader, options);
            if (readHandlerRegistration.TryGetPostProcessor<TComponent>(out var pp))
            {
                component = pp(in component);
            }

            key = entityMapper.EntityKeyMapper<TEntityKey>(entityKey);
            return true;
        }

        public int ReadDestroyedCount()
        {
            var recordType = MessagePackSerializer.Deserialize<BinaryControlObjects.BinaryStreamState>(reader, options);
            if (recordType != BinaryControlObjects.BinaryStreamState.DestroyedEntities)
            {
                throw new BinaryReaderException("Invalid stream state: Expected DestroyedEntities-Start record");
            }

            return MessagePackSerializer.Deserialize<int>(reader, options);
        }

        public TEntityKey ReadDestroyed(IEntityKeyMapper entityMapper)
        {
            var key = MessagePackSerializer.Deserialize<EntityKeyData>(reader, options);
            return entityMapper.EntityKeyMapper<TEntityKey>(key);
        }
    }
}