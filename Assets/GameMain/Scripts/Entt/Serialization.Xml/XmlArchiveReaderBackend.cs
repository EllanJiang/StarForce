/*
  作者：LTH
  文件描述：
  文件名：XmlArchiveReaderBackend
  创建时间：2023/07/16 17:07:SS
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Xml;
using Entt.Entities;
using Entt.Serialization.Xml.Impl;
using Serilog;

namespace Entt.Serialization.Xml
{
    /// <summary>
    /// 在后台处理xml文档读取
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public class XmlArchiveReaderBackend<TEntityKey> where TEntityKey : IEntityKey
    {
        static readonly ILogger logger = LogHelper.ForContext<XmlArchiveReaderBackend<TEntityKey>>();
        
        readonly MethodInfo tagParserMethod;
        readonly MethodInfo missingTagParserMethod;
        readonly MethodInfo componentParserMethod;
        readonly Dictionary<Type, object> cachedComponentDelegates;
        readonly Dictionary<Type, object> cachedTagDelegates;
        readonly Dictionary<Type, object> cachedMissingTagDelegates;
        
        public readonly XmlReadHandlerRegistry Registry;
        
        public XmlArchiveReaderBackend(XmlReadHandlerRegistry registry)
        {
            this.Registry = registry;
            this.cachedTagDelegates = new Dictionary<Type, object>();
            this.cachedComponentDelegates = new Dictionary<Type, object>();
            this.cachedMissingTagDelegates = new Dictionary<Type, object>();

            var paramTypes = new[]
            {
                typeof(XmlReader), typeof(TEntityKey), typeof(ISnapshotLoader<TEntityKey>), typeof(XmlReadHandlerRegistration)
            };

            //tag解析方法
            tagParserMethod = typeof(XmlArchiveReaderBackend<TEntityKey>).GetMethod(nameof(ParseTagInternal),
                                                                                    BindingFlags.Instance | BindingFlags.NonPublic, null, 
                                                                                    paramTypes, null)
                              ?? throw new InvalidOperationException("Unable to find tag parsing wrapper method");

            //组件解析方法
            componentParserMethod = typeof(XmlArchiveReaderBackend<TEntityKey>).GetMethod(nameof(ParseComponentInternal),
                                                                                          BindingFlags.Instance | BindingFlags.NonPublic, null, 
                                                                                          paramTypes, null)
                                    ?? throw new InvalidOperationException("Unable to find component parsing wrapper method");
            
            var missingTagParamTypes = new[]
            {
                typeof(ISnapshotLoader<TEntityKey>)
            };
            //丢失的tag解析方法
            missingTagParserMethod = typeof(XmlArchiveReaderBackend<TEntityKey>).GetMethod(nameof(ParseMissingTagInternal),
                                                                                           BindingFlags.Instance | BindingFlags.NonPublic, null,
                                                                                           missingTagParamTypes, null)
                                     ?? throw new InvalidOperationException("Unable to find component parsing wrapper method");
        }
        
        /// <summary>
        ///  从reader中读取entity
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entityMapper"></param>
        /// <returns></returns>
        public TEntityKey ReadEntity(XmlReader reader,
                                     IEntityKeyMapper entityMapper)
        {
            return entityMapper.EntityKeyMapper<TEntityKey>(ReadEntityData(reader, XmlTagNames.Entity));
        }

        /// <summary>
        ///  从reader中读取已经被销毁的entity
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entityMapper"></param>
        /// <returns></returns>
        public TEntityKey ReadDestroyedEntity(XmlReader reader,
                                              IEntityKeyMapper entityMapper)
        {
            return entityMapper.EntityKeyMapper<TEntityKey>(ReadEntityData(reader, XmlTagNames.DestroyedEntity));
        }

        /// <summary>
        /// 从reader中读取EntityKeyData
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        /// <exception cref="XmlReaderException"></exception>
        EntityKeyData ReadEntityData(XmlReader reader, string tag)
        {
            var age = byte.Parse(reader.GetAttribute("entity-age") ?? throw reader.FromMissingAttribute(tag, "entity-age"));
            var key = int.Parse(reader.GetAttribute("entity-key") ?? throw reader.FromMissingAttribute(tag, "entity-key"));
            logger.Verbose("Reading Entity Data: Age: {Age}, Key: {Key}", age, key);
            return new EntityKeyData(age, key);
        }


        /// <summary>
        ///  从reader中读取指定类型的tag
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entityMapper"></param>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        public void ReadTagTyped<TComponent>(XmlReader reader,
                                             IEntityKeyMapper entityMapper,
                                             out TEntityKey entity,
                                             out TComponent component)
        {
            if (!Registry.TryGetValue(typeof(TComponent), out var handler))
            {
                throw new InvalidOperationException("No handler with type '" + typeof(TComponent) + "' defined.");
            }

            if (!handler.TryGetParser<TComponent>(out var parser))
            {
                throw new InvalidOperationException("Unable to resolve handler for registered type " + typeof(TComponent));
            }

            entity = entityMapper.EntityKeyMapper<TEntityKey>(ReadEntityData(reader, XmlTagNames.Tag));
            var _ = reader.Read();
            logger.Verbose("Reading Component for {Entity}", entity);
            component = parser(reader);
        }

        /// <summary>
        ///  从reader中读取tag
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="loader"></param>
        /// <exception cref="XmlReaderException"></exception>
        /// <exception cref="SnapshotIOException"></exception>
        public void ReadTag(XmlReader reader, ISnapshotLoader<TEntityKey> loader)
        {
            var type = reader.GetAttribute("type") ?? throw reader.FromMissingAttribute(XmlTagNames.Tag, "type");
            if (!Registry.TryGetValue(type, out var handler))
            {
                throw new SnapshotIOException("No handler with type '" + type + "' defined.");
            }

            var missing = reader.GetAttribute("missing");
            if (string.Equals(missing, "true", StringComparison.InvariantCultureIgnoreCase))
            {
                ParseMissingTag(loader, handler);
                reader.Skip();
                return;
            }

            var entity = loader.Map(ReadEntityData(reader, XmlTagNames.Tag));

            var _ = reader.Read();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    ParseTag(reader, entity, loader, handler);
                }
            }
        }

        /// <summary>
        ///  从reader中读取指定类型的组件
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entityMapper"></param>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <exception cref="SnapshotIOException"></exception>
        public void ReadComponentTyped<TComponent>(XmlReader reader,
                                                   IEntityKeyMapper entityMapper,
                                                   out TEntityKey entity,
                                                   out TComponent component)
        {
            if (!Registry.TryGetValue(typeof(TComponent), out var handler))
            {
                throw new SnapshotIOException("No handler with type '" + typeof(TComponent) + "' defined.");
            }

            if (!handler.TryGetParser<TComponent>(out var parser))
            {
                throw new SnapshotIOException("Unable to resolve handler for registered type " + typeof(TComponent));
            }

            entity = entityMapper.EntityKeyMapper<TEntityKey>(ReadEntityData(reader, XmlTagNames.Component));
            var _ = reader.Read();
            component = parser(reader);
        }

        /// <summary>
        /// 从reader中读取组件
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="loader"></param>
        /// <param name="type"></param>
        /// <exception cref="SnapshotIOException"></exception>
        public void ReadComponent(XmlReader reader, ISnapshotLoader<TEntityKey> loader, string type)
        {
            var entity = loader.Map(ReadEntityData(reader, XmlTagNames.Component));

            if (!Registry.TryGetValue(type, out var handler))
            {
                throw new SnapshotIOException($"No parse handler defined for component-type '{type}'");
            }

            var _ = reader.Read();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        ParseComponent(reader, entity, loader, handler);
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 解析委托
        /// </summary>
        delegate void ParseAction(XmlReader reader, 
                                  TEntityKey entityRaw, 
                                  ISnapshotLoader<TEntityKey> loader, 
                                  XmlReadHandlerRegistration handler);

        /// <summary>
        /// 由外部传入的委托来解析tag，一般是各个tag自定义的reader方法
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entityRaw"></param>
        /// <param name="loader"></param>
        /// <param name="handler"></param>
        void ParseTag(XmlReader reader, TEntityKey entityRaw, ISnapshotLoader<TEntityKey> loader, XmlReadHandlerRegistration handler)
        {
            if (cachedTagDelegates.TryGetValue(handler.TargetType, out var actionRaw))
            {
                var action = (ParseAction)actionRaw;
                action(reader, entityRaw, loader, handler);
                return;
            }

            var method = tagParserMethod.MakeGenericMethod(handler.TargetType);
            var actionDelegate = (ParseAction)Delegate.CreateDelegate(typeof(ParseAction), this, method);
            cachedTagDelegates[handler.TargetType] = actionDelegate;
            actionDelegate(reader, entityRaw, loader, handler);
        }

        /// <summary>
        /// 内部解析tag
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entityRaw"></param>
        /// <param name="loader"></param>
        /// <param name="handler"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <exception cref="SnapshotIOException"></exception>
        void ParseTagInternal<TComponent>(XmlReader reader, TEntityKey entityRaw, 
                                          ISnapshotLoader<TEntityKey> loader, XmlReadHandlerRegistration handler)
        {
            if (!handler.TryGetParser<TComponent>(out var parser))
            {
                throw new SnapshotIOException("Unable to resolve handler for registered type " + typeof(TComponent));
            }

            var result = parser(reader);
            loader.OnTag(entityRaw, in result);
        }

        /// <summary>
        /// 由外部传入的委托来解析组件，一般是各个component自定义的reader方法
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entityRaw"></param>
        /// <param name="loader"></param>
        /// <param name="handler"></param>
        void ParseComponent(XmlReader reader, TEntityKey entityRaw, ISnapshotLoader<TEntityKey> loader, 
                            XmlReadHandlerRegistration handler)
        {
            if (cachedComponentDelegates.TryGetValue(handler.TargetType, out var actionRaw))
            {
                var action = (ParseAction)actionRaw;
                action(reader, entityRaw, loader, handler);
                return;
            }

            var method = componentParserMethod.MakeGenericMethod(handler.TargetType);
            var actionDelegate = (ParseAction)Delegate.CreateDelegate(typeof(ParseAction), this, method);
            cachedComponentDelegates[handler.TargetType] = actionDelegate;
            actionDelegate(reader, entityRaw, loader, handler);
        }

        /// <summary>
        /// 内部解析组件
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entityRaw"></param>
        /// <param name="loader"></param>
        /// <param name="handler"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <exception cref="SnapshotIOException"></exception>
        void ParseComponentInternal<TComponent>(XmlReader reader, TEntityKey entityRaw, 
                                                ISnapshotLoader<TEntityKey> loader, XmlReadHandlerRegistration handler)
        {
            if (!handler.TryGetParser<TComponent>(out var parser))
            {
                throw new SnapshotIOException("Unable to resolve handler for registered type " + typeof(TComponent));
            }

            var result = parser(reader);
            loader.OnComponent(entityRaw, in result);
        }

        /// <summary>
        /// 由外部传入的委托来解析丢失的tag，一般是各个tag自定义的reader方法
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="handler"></param>
        void ParseMissingTag(ISnapshotLoader<TEntityKey> loader, XmlReadHandlerRegistration handler)
        {
            if (cachedMissingTagDelegates.TryGetValue(handler.TargetType, out var actionRaw))
            {
                var action = (Action<ISnapshotLoader<TEntityKey>>)actionRaw;
                action(loader);
                return;
            }

            var method = missingTagParserMethod.MakeGenericMethod(handler.TargetType);
            
            var actionDelegate = (Action<ISnapshotLoader<TEntityKey>>)Delegate.CreateDelegate(typeof(Action<ISnapshotLoader<TEntityKey>>), this, method);
            cachedMissingTagDelegates[handler.TargetType] = actionDelegate;
            actionDelegate(loader);
        }

        /// <summary>
        /// 内部解析丢失的tag
        /// </summary>
        /// <param name="loader"></param>
        /// <typeparam name="TComponent"></typeparam>
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        void ParseMissingTagInternal<TComponent>(ISnapshotLoader<TEntityKey> loader)
        {
            loader.OnTagRemoved<TComponent>();
        }

    }
}