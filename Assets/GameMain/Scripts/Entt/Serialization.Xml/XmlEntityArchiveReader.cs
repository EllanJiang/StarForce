/*
  作者：LTH
  文件描述：
  文件名：XmlEntityArchiveReader
  创建时间：2023/07/16 16:07:SS
*/

using System;
using System.Xml;
using Entt.Entities;
using Entt.Serialization.Xml.Impl;

namespace Entt.Serialization.Xml
{
    public class XmlEntityArchiveReader<TEntityKey> : IEntityArchiveReader<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly XmlArchiveReaderBackend<TEntityKey> readerConfiguration;
        readonly XmlReader reader;

        public XmlEntityArchiveReader(XmlReadHandlerRegistry readerConfiguration, XmlReader reader) :
            this(new XmlArchiveReaderBackend<TEntityKey>(readerConfiguration), reader)
        {
            
        }
        
        public XmlEntityArchiveReader(XmlArchiveReaderBackend<TEntityKey> readerConfiguration, XmlReader reader)
        {
            this.readerConfiguration = readerConfiguration ?? throw new ArgumentNullException(nameof(readerConfiguration));
            this.reader = reader;
        }
        
        public XmlReadHandlerRegistry Registry
        {
            get { return readerConfiguration.Registry; }
        }
        
        /// <summary>
        /// 读取entity数量
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SnapshotIOException"></exception>
        public int ReadEntityCount()
        {
            reader.AdvanceToElement(XmlTagNames.Entities);  //跳转到entities节点
            if (reader.TryGetAttributeInt(XmlTagNames.CountAttribute, out var value))  //获取entity数量
            {
                return value;
            }
            throw new SnapshotIOException("Unable to parse 'count' attribute on element 'Entities'");
        }
        
        /// <summary>
        /// 读取单个entity
        /// </summary>
        /// <param name="entityMapper"></param>
        /// <returns></returns>
        public TEntityKey ReadEntity(IEntityKeyMapper entityMapper)
        {
            reader.AdvanceToElement(XmlTagNames.Entity);
            return readerConfiguration.ReadEntity(reader, entityMapper);
        }

        /// <summary>
        /// 读取指定类型的component的数量
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        /// <exception cref="SnapshotIOException"></exception>
        public int ReadComponentCount<TComponent>()
        {
            reader.AdvanceToElement(XmlTagNames.Components);
            if (reader.TryGetAttributeInt(XmlTagNames.CountAttribute, out var value))
            {
                return value;
            }
            throw new SnapshotIOException("Unable to parse 'count' attribute on element 'Components'");
        }

        /// <summary>
        /// 读取指定类型的component
        /// </summary>
        /// <param name="entityMapper"></param>
        /// <param name="key"></param>
        /// <param name="component"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public bool TryReadComponent<TComponent>(IEntityKeyMapper entityMapper, 
                                                 out TEntityKey key, out TComponent component)
        {
            reader.AdvanceToElement(XmlTagNames.Component);
            readerConfiguration.ReadComponentTyped(reader, entityMapper, out key, out component);
            return true;
        }

        /// <summary>
        /// 是否有指定类型的tag
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public bool ReadTagFlag<TComponent>()
        {
            reader.AdvanceToElement(XmlTagNames.Tag);
            if (reader.TryGetAttributeBool("missing", out var value))
            {
                // missing=false indicates that the tag is declared.
                return value == false;
            }

            return false;
        }

        /// <summary>
        /// 读取指定类型的tag
        /// </summary>
        /// <param name="entityMapper"></param>
        /// <param name="entityKey"></param>
        /// <param name="component"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public bool TryReadTag<TComponent>(IEntityKeyMapper entityMapper,
                                           out TEntityKey entityKey, out TComponent component)
        {
            readerConfiguration.ReadTagTyped(reader, entityMapper, out entityKey, out component);
            return true;
        }

        /// <summary>
        /// 读取已经被销毁的entity的数量
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SnapshotIOException"></exception>
        public int ReadDestroyedCount()
        {
            reader.AdvanceToElement(XmlTagNames.DestroyedEntities);
            if (reader.TryGetAttributeInt(XmlTagNames.CountAttribute, out var value))
            {
                return value;
            }
            throw new SnapshotIOException("Unable to parse 'count' attribute on element 'DestroyedEntities'");
        }

        /// <summary>
        /// 读取已经被销毁的entity
        /// </summary>
        /// <param name="entityMapper"></param>
        /// <returns></returns>
        public TEntityKey ReadDestroyed(IEntityKeyMapper entityMapper)
        {
            return readerConfiguration.ReadDestroyedEntity(reader, entityMapper);
        }
    }
}