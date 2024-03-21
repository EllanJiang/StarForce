/*
  作者：LTH
  文件描述：
  文件名：BinaryBulkArchiveReader
  创建时间：2023/07/16 21:07:SS
*/

using System;
using System.IO;
using Entt.Entities;
using Entt.Serialization.Binary.Impl;
using MessagePack;

namespace Entt.Serialization.Binary
{
    /// <summary>
    /// 二进制数据reader
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public class BinaryBulkArchiveReader<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly MessagePackSerializerOptions? options;
        readonly BinaryReaderBackend<TEntityKey> readerBackend;

        public BinaryBulkArchiveReader(BinaryReadHandlerRegistry reg, MessagePackSerializerOptions? optionsRaw = null): 
            this(new BinaryReaderBackend<TEntityKey>(reg), optionsRaw)
        {
        }

        public BinaryBulkArchiveReader(BinaryReaderBackend<TEntityKey> readerBackend, MessagePackSerializerOptions? optionsRaw = null)
        {
            this.readerBackend = readerBackend;
            this.options = optionsRaw;
        }

        /// <summary>
        /// 从二进制流数据中读取entity和component到loader中
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="loader"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void ReadAll(Stream reader, ISnapshotLoader<TEntityKey> loader)
        {
            for (;;)
            {
                var recordType = MessagePackSerializer.Deserialize<BinaryControlObjects.BinaryStreamState>(reader, options);
                switch (recordType)
                {
                    case BinaryControlObjects.BinaryStreamState.DestroyedEntities:
                        ReadDestroyedEntities(reader, loader);
                        break;
                    case BinaryControlObjects.BinaryStreamState.Entities:
                        ReadEntities(reader, loader);
                        break;
                    case BinaryControlObjects.BinaryStreamState.Component:
                        ReadComponents(reader, loader);
                        break;
                    case BinaryControlObjects.BinaryStreamState.Tag:
                        ReadTag(reader, loader);
                        break;
                    case BinaryControlObjects.BinaryStreamState.EndOfFrame:
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        void ReadTag(Stream reader, ISnapshotLoader<TEntityKey> loader)
        {
            readerBackend.ReadTag(reader, loader, options);
        }

        void ReadComponents(Stream reader, ISnapshotLoader<TEntityKey> loader)
        {
            var startRecord = MessagePackSerializer.Deserialize<BinaryControlObjects.StartComponentRecord>(reader, options);
            if (!readerBackend.Registry.TryGetValue(startRecord.ComponentId, out var handler))
            {
                throw new BinaryReaderException($"Corrupted stream state: No handler for component type {startRecord.ComponentId}");
            }

            for (int i = 0; i < startRecord.ComponentCount; i++)
            {
                readerBackend.ReadComponent(reader, loader, handler, options);
            }
        }

        void ReadDestroyedEntities(Stream reader, ISnapshotLoader<TEntityKey> loader)
        {
            var entityCount = MessagePackSerializer.Deserialize<int>(reader, options);
            for (int i = 0; i < entityCount; i++)
            {
                var key = MessagePackSerializer.Deserialize<EntityKeyData>(reader, options);
                loader.OnDestroyedEntity(loader.Map(key));
            }
        }

        void ReadEntities(Stream reader, ISnapshotLoader<TEntityKey> loader)
        {
            var entityCount = MessagePackSerializer.Deserialize<int>(reader, options);
            for (int i = 0; i < entityCount; i++)
            {
                var key = MessagePackSerializer.Deserialize<EntityKeyData>(reader, options);
                loader.OnEntity(loader.Map(key));
            }
        }
    }
}