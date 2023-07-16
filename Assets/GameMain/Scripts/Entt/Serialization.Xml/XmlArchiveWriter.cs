/*
  作者：LTH
  文件描述：
  文件名：XmlArchiveWriter
  创建时间：2023/07/16 17:07:SS
*/

using System;
using System.Globalization;
using System.Xml;
using Entt.Entities;
using Entt.Serialization.Xml.Impl;

namespace Entt.Serialization.Xml
{
    /// <summary>
    /// xml文档writer
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public class XmlArchiveWriter<TEntityKey> : IEntityArchiveWriter<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly XmlWriter writer;

        public XmlWriteHandlerRegistry Registry { get; }

        public XmlArchiveWriter(XmlWriteHandlerRegistry registry, XmlWriter writer)
        {
            this.Registry = registry;
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }
        
        /// <summary>
        /// 创建xml文档
        /// </summary>
        public void WriteDefaultSnapshotDocumentHeader()
        {
            writer.WriteStartDocument();                    //开始写入文档
            writer.WriteStartElement("snapshot");  //快照节点
            writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
        }

        /// <summary>
        /// 结束创建xml文档
        /// </summary>
        public void WriteDefaultSnapshotDocumentFooter()
        {
            writer.WriteEndElement();           //结束写入节点元素
            writer.WriteEndDocument();          //结束写入文档
        }
        
        public void WriteStartDestroyed(in int entityCount)
        {
            writer.WriteStartElement(XmlTagNames.DestroyedEntities);
            writer.WriteAttributeString(XmlTagNames.CountAttribute, entityCount.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteDestroyed(in TEntityKey entity)
        {
            writer.WriteStartElement(XmlTagNames.DestroyedEntity);
            writer.WriteAttributeString("entity-key", entity.Key.ToString("X"));
            writer.WriteAttributeString("entity-age", entity.Age.ToString("X"));
            writer.WriteEndElement();
        }

        public void WriteEndDestroyed()
        {
            writer.WriteEndElement();
        }

        public void WriteStartEntity(in int entityCount)
        {
            writer.WriteStartElement(XmlTagNames.Entities);
            writer.WriteAttributeString(XmlTagNames.CountAttribute, entityCount.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteEntity(in TEntityKey entity)
        {
            writer.WriteStartElement(XmlTagNames.Entity);
            writer.WriteAttributeString("entity-key", entity.Key.ToString("X"));
            writer.WriteAttributeString("entity-age", entity.Age.ToString("X"));
            writer.WriteEndElement();
        }

        public void WriteEndEntity()
        {
            writer.WriteEndElement();
        }

        public void WriteStartComponent<TComponent>(in int entityCount)
        {
            writer.WriteStartElement(XmlTagNames.Components);
            writer.WriteAttributeString(XmlTagNames.CountAttribute, entityCount.ToString(CultureInfo.InvariantCulture));

            var handler = Registry.QueryHandler<TComponent>();
            writer.WriteAttributeString("type", handler.TypeId);  //写入组件类型
        }

        /// <summary>
        /// 写入某个entity的某个component
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        /// <typeparam name="TComponent"></typeparam>
        public void WriteComponent<TComponent>(in TEntityKey entity, in TComponent component)
        {
            var handler = Registry.QueryHandler<TComponent>();
            writer.WriteStartElement(XmlTagNames.Component);
            writer.WriteAttributeString("entity-key", entity.Key.ToString("X"));  //先写入EntityKey
            writer.WriteAttributeString("entity-age", entity.Age.ToString("X"));
            handler.GetHandler<TComponent>()?.Invoke(writer, component);                                //再写入component，每个组件都需要自定义writer
            writer.WriteEndElement();
        }

        public void WriteEndComponent<TComponent>()
        {
            writer.WriteEndElement();
        }

        public void WriteMissingTag<TComponent>()
        {
            var handler = Registry.QueryHandler<TComponent>();
            writer.WriteStartElement(XmlTagNames.Tag);
            writer.WriteAttributeString("type", handler.TypeId);
            writer.WriteAttributeString("missing", "true");
            writer.WriteEndElement();
        }

        public void WriteTag<TComponent>(in TEntityKey entity, in TComponent c)
        {
            var handler = Registry.QueryHandler<TComponent>();
            writer.WriteStartElement(XmlTagNames.Tag);
            writer.WriteAttributeString("type", handler.TypeId);
            writer.WriteAttributeString("missing", "false");
            writer.WriteAttributeString("entity-key", entity.Key.ToString("X"));    //先写入EntityKey
            writer.WriteAttributeString("entity-age", entity.Age.ToString("X"));
            handler.GetHandler<TComponent>()?.Invoke(writer, c);                                          //再写入Tag，每个tag都需要自定义writer
            writer.WriteEndElement();
        }

        public void WriteEndOfFrame()
        {
            
        }

        /// <summary>
        /// 调用XmlWriter的Flush方法
        /// </summary>
        public void FlushFrame()
        {
            this.writer.Flush();
        }
    }
}