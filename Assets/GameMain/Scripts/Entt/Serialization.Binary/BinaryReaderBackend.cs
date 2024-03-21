/*
  作者：LTH
  文件描述：
  文件名：BinaryReaderBackend
  创建时间：2023/07/16 21:07:SS
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Entt.Entities;
using Entt.Serialization.Binary.Impl;
using MessagePack;

namespace Entt.Serialization.Binary
{
    /// <summary>
    /// 在后台处理二进制读取相关操作
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public class BinaryReaderBackend<TEntityKey> where TEntityKey : IEntityKey
    {
        /// <summary>
        /// 解析方法委托
        /// </summary>
        delegate void ParseFunctionDelegate(Stream stream, TEntityKey entityKey, 
            ISnapshotLoader<TEntityKey> loader, 
            BinaryReadHandlerRegistration registration, 
            MessagePackSerializerOptions? options);

        public readonly BinaryReadHandlerRegistry Registry;
        
        readonly MethodInfo missingTagParserMethod;
        readonly MethodInfo tagParserMethod;
        readonly MethodInfo componentParserMethod;
        readonly Dictionary<Type, object> cachedComponentDelegates;
        readonly Dictionary<Type, object> cachedTagDelegates;
        readonly Dictionary<Type, object> cachedMissingTagDelegates;

         public BinaryReaderBackend(BinaryReadHandlerRegistry registry)
        {
            this.Registry = registry;
            this.cachedTagDelegates = new Dictionary<Type, object>();
            this.cachedComponentDelegates = new Dictionary<Type, object>();
            this.cachedMissingTagDelegates = new Dictionary<Type, object>();

            //方法参数类型
            var paramTypes = new[]
            {
                typeof(Stream), 
                typeof(TEntityKey), 
                typeof(ISnapshotLoader<TEntityKey>), 
                typeof(BinaryReadHandlerRegistration),
                typeof(MessagePackSerializerOptions)
            };

            tagParserMethod = typeof(BinaryReaderBackend<TEntityKey>).GetMethod(nameof(ParseTagInternal),
                                                                                BindingFlags.Instance | BindingFlags.NonPublic, null, 
                                                                                paramTypes, null)
                              ?? throw new InvalidOperationException("Unable to find tag parsing wrapper method");

            componentParserMethod = typeof(BinaryReaderBackend<TEntityKey>).GetMethod(nameof(ParseComponentInternal),
                                                                                      BindingFlags.Instance | BindingFlags.NonPublic, null, paramTypes, null)
                                    ?? throw new InvalidOperationException("Unable to find component parsing wrapper method");

            var missingTagParamTypes = new[]
            {
                typeof(ISnapshotLoader<TEntityKey>)
            };

            missingTagParserMethod = typeof(BinaryReaderBackend<TEntityKey>).GetMethod(nameof(ParseMissingTagInternal),
                                                                                       BindingFlags.Instance | BindingFlags.NonPublic, null,
                                                                                       missingTagParamTypes, null)
                                     ?? throw new InvalidOperationException("Unable to find tag parsing wrapper method");
        }

        
        /// <summary>
        /// 读取tag
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="loader"></param>
        /// <param name="options"></param>
        /// <exception cref="BinaryReaderException"></exception>
        public void ReadTag(Stream stream, ISnapshotLoader<TEntityKey> loader, 
                            MessagePackSerializerOptions? options)
        {
            var startTagRecord = MessagePackSerializer.Deserialize<BinaryControlObjects.StartTagRecord>(stream, options);
            if (!Registry.TryGetValue(startTagRecord.ComponentId, out var handler))
            {
                throw new BinaryReaderException($"Corrupted stream state: No handler for component type {startTagRecord.ComponentId}");
            }
            
            if (startTagRecord.ComponentExists)
            {
                var entityKey = MessagePackSerializer.Deserialize<EntityKeyData>(stream, options);
                ParseTag(stream, loader.Map(entityKey), loader, handler, options);
            }
            else
            {
                ParseMissingTag(loader, handler);
            }
        }

        /// <summary>
        /// 解析丢失的tag
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="handler"></param>
        void ParseMissingTag(ISnapshotLoader<TEntityKey> loader, BinaryReadHandlerRegistration handler)
        {
            if (cachedMissingTagDelegates.TryGetValue(handler.TargetType, out var actionRaw))
            {
                var parseAction = (Action<ISnapshotLoader<TEntityKey>, BinaryReadHandlerRegistration>)actionRaw;
                parseAction(loader, handler);
                return;
            }

            var method = missingTagParserMethod.MakeGenericMethod(handler.TargetType);
            var newParseAction = (Action<ISnapshotLoader<TEntityKey>, BinaryReadHandlerRegistration>)
                Delegate.CreateDelegate(typeof(Action<ISnapshotLoader<TEntityKey>, BinaryReadHandlerRegistration>), this, method);
            cachedMissingTagDelegates[handler.TargetType] = newParseAction;
            newParseAction(loader, handler);

        }

        /// <summary>
        /// 在内部解析丢失的tag
        /// </summary>
        /// <param name="loader"></param>
        /// <typeparam name="TComponent"></typeparam>
        void ParseMissingTagInternal<TComponent>(ISnapshotLoader<TEntityKey> loader)
        {
            loader.OnTagRemoved<TComponent>();
        }

        /// <summary>
        /// 在外部（一般是tag组件内部）解析tag
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entityRaw"></param>
        /// <param name="loader"></param>
        /// <param name="handler"></param>
        /// <param name="options"></param>
        void ParseTag(Stream reader, 
                      TEntityKey entityRaw, 
                      ISnapshotLoader<TEntityKey> loader, 
                      BinaryReadHandlerRegistration handler,
                      MessagePackSerializerOptions? options)
        {
            if (cachedTagDelegates.TryGetValue(handler.TargetType, out var actionRaw))
            {
                var parseAction = (ParseFunctionDelegate)actionRaw;
                parseAction(reader, entityRaw, loader, handler, options);
                return;
            }

            var method = tagParserMethod.MakeGenericMethod(handler.TargetType);
            var newParseAction = (ParseFunctionDelegate)Delegate.CreateDelegate(typeof(ParseFunctionDelegate), this, method);
            cachedTagDelegates[handler.TargetType] = newParseAction;
            newParseAction(reader, entityRaw, loader, handler, options);
        }

        /// <summary>
        /// 在内部解析tag
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="entity"></param>
        /// <param name="loader"></param>
        /// <param name="registration"></param>
        /// <param name="options"></param>
        /// <typeparam name="TComponent"></typeparam>
        void ParseTagInternal<TComponent>(Stream stream, 
                                          TEntityKey entity, 
                                          ISnapshotLoader<TEntityKey> loader, 
                                          BinaryReadHandlerRegistration registration,
                                          MessagePackSerializerOptions? options)
        {
            var component = MessagePackSerializer.Deserialize<TComponent>(stream, options);
            if (registration.TryGetPostProcessor<TComponent>(out var pp))
            {
                component = pp(in component);
            }

            loader.OnTag(entity, component);
        }

        /// <summary>
        /// 从二进制流中读取组件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="loader"></param>
        /// <param name="registration"></param>
        /// <param name="options"></param>
        public void ReadComponent(Stream stream, 
                                  ISnapshotLoader<TEntityKey> loader, 
                                  BinaryReadHandlerRegistration registration,
                                  MessagePackSerializerOptions? options)
        {
            var entityKeyData = MessagePackSerializer.Deserialize<EntityKeyData>(stream, options);
            var entityKey = loader.Map(entityKeyData);
            ParseComponent(stream, entityKey, loader, registration, options);
        }

        /// <summary>
        /// 在外部解析组件数据（一般是在组件的reader方法内处理）
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entityRaw"></param>
        /// <param name="loader"></param>
        /// <param name="handler"></param>
        /// <param name="options"></param>
        void ParseComponent(Stream reader, TEntityKey entityRaw,
                            ISnapshotLoader<TEntityKey> loader, BinaryReadHandlerRegistration handler,
                            MessagePackSerializerOptions? options)
        {
            if (cachedComponentDelegates.TryGetValue(handler.TargetType, out var actionRaw))
            {
                var parseAction = (ParseFunctionDelegate)actionRaw;
                parseAction(reader, entityRaw, loader, handler, options);
                return;
            }

            var method = componentParserMethod.MakeGenericMethod(handler.TargetType);
            var newParseAction = (ParseFunctionDelegate)Delegate.CreateDelegate(typeof(ParseFunctionDelegate), this, method);
            cachedComponentDelegates[handler.TargetType] = newParseAction;
            newParseAction(reader, entityRaw, loader, handler, options);
        }

        /// <summary>
        /// 在内部解析组件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="entity"></param>
        /// <param name="loader"></param>
        /// <param name="registration"></param>
        /// <param name="options"></param>
        /// <typeparam name="TComponent"></typeparam>
        void ParseComponentInternal<TComponent>(Stream stream, 
                                                TEntityKey entity, 
                                                ISnapshotLoader<TEntityKey> loader, 
                                                BinaryReadHandlerRegistration registration,
                                                MessagePackSerializerOptions? options)
        {
            var component = MessagePackSerializer.Deserialize<TComponent>(stream, options);
            if (registration.TryGetPostProcessor<TComponent>(out var pp))
            {
                component = pp(in component);
            }

            loader.OnComponent(entity, component);
        }
    }
}