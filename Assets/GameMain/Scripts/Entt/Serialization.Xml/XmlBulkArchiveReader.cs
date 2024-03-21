/*
  作者：LTH
  文件描述：
  文件名：XmlBulkArchiveReader
  创建时间：2023/07/16 18:07:SS
*/

using System.Xml;
using Entt.Entities;
using Entt.Serialization.Xml.Impl;
using Serilog;

namespace Entt.Serialization.Xml
{
    public class XmlBulkArchiveReader<TEntityKey> where TEntityKey : IEntityKey
    {
        static readonly ILogger logger = LogHelper.ForContext<XmlBulkArchiveReader<TEntityKey>>();
        readonly XmlArchiveReaderBackend<TEntityKey> readerConfiguration;
        
        public XmlBulkArchiveReader(XmlReadHandlerRegistry registry) : this(new XmlArchiveReaderBackend<TEntityKey>(registry))
        {
        }

        public XmlBulkArchiveReader(XmlArchiveReaderBackend<TEntityKey> readerConfiguration)
        {
            this.readerConfiguration = readerConfiguration;
        }
        
        /// <summary>
        /// 从reader中读取所有数据
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="loader"></param>
        /// <param name="mapper"></param>
        public void ReadAllFragment(XmlReader reader, ISnapshotLoader<TEntityKey> loader, IEntityKeyMapper mapper)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            logger.Verbose("Begin ReadAllFragment");
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (!HandleElement(reader, loader, mapper))
                        {
                            reader.Skip();
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        return;
                    }
                }
            }
            finally
            {
                logger.Verbose("End ReadAllFragment");
            }
        }

        /// <summary>
        /// 创建一个新的快照，然后从reader中读取数据到快照中
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="loader"></param>
        /// <param name="mapper"></param>
        public void ReadAll(XmlReader reader, ISnapshotLoader<TEntityKey> loader, IEntityKeyMapper mapper)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "snapshot")
                    {
                        ReadAllFragment(reader, loader, mapper);
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 根据reader.Name处理xml节点元素
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="loader"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        /// <exception cref="XmlReaderException"></exception>
        bool HandleElement(XmlReader reader, ISnapshotLoader<TEntityKey> loader, IEntityKeyMapper mapper)
        {
            switch (reader.Name)
            {
                case XmlTagNames.Entities:
                {
                    ReadEntities(reader, loader, mapper);
                    return true;
                }
                case XmlTagNames.DestroyedEntities:
                {
                    ReadDestroyed(reader, loader, mapper);
                    return true;
                }
                case XmlTagNames.Components:
                {
                    var type = reader.GetAttribute("type");
                    if (string.IsNullOrEmpty(type))
                    {
                        throw reader.FromMissingAttribute(XmlTagNames.Component, "type");
                    }

                    ReadComponent(reader, loader, type);
                    return true;
                }
                case XmlTagNames.Tag:
                {
                    readerConfiguration.ReadTag(reader, loader);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 读取所有Entity到loader中
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="loader"></param>
        /// <param name="mapper"></param>
        void ReadEntities(XmlReader reader, ISnapshotLoader<TEntityKey> loader, IEntityKeyMapper mapper)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            logger.Verbose("Begin ReadEntities");
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == XmlTagNames.Entity)
                        {
                            var entity = readerConfiguration.ReadEntity(reader, mapper);
                            loader.OnEntity(entity);
                        }
                        else
                        {
                            reader.Skip();
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        return;
                    }
                }
            }
            finally
            {
                logger.Verbose("End ReadEntities");
            }
        }

        /// <summary>
        /// 读取所有已销毁的entity到loader中
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="loader"></param>
        /// <param name="mapper"></param>
        void ReadDestroyed(XmlReader reader, ISnapshotLoader<TEntityKey> loader, IEntityKeyMapper mapper)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            logger.Verbose("Begin ReadDestroyed");
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == XmlTagNames.DestroyedEntity)
                        {
                            var entity = readerConfiguration.ReadDestroyedEntity(reader, mapper);
                            loader.OnDestroyedEntity(entity);
                        }
                        else
                        {
                            reader.Skip();
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        return;
                    }
                }
            }
            finally
            {
                logger.Verbose("End ReadDestroyed");
            }
        }

        /// <summary>
        /// 读取所有component到loader中
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="loader"></param>
        /// <param name="type"></param>
        void ReadComponent(XmlReader reader, ISnapshotLoader<TEntityKey> loader, string type)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            logger.Verbose("Begin ReadComponent");
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == XmlTagNames.Component)
                        {
                            readerConfiguration.ReadComponent(reader, loader, type);
                        }
                        else
                        {
                            reader.Skip();
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        return;
                    }
                }
            }
            finally
            {
                logger.Verbose("End ReadComponent");
            }
        }
    }
}