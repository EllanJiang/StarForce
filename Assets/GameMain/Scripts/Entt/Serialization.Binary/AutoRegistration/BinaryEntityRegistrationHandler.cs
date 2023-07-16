/*
  作者：LTH
  文件描述：
  文件名：BinaryEntityRegistrationHandler
  创建时间：2023/07/16 21:07:SS
*/

using System;
using System.Reflection;
using System.Runtime.Serialization;
using Entt.Annotations;
using Entt.Annotations.Impl;
using MessagePack;
using MessagePack.Formatters;
using Serilog;

namespace Entt.Serialization.Binary.AutoRegistration
{
    /// <summary>
    /// 二进制handler
    /// </summary>
    public class BinaryEntityRegistrationHandler: EntityRegistrationHandlerBase
    {
        static readonly ILogger Logger = Log.ForContext<BinaryEntityRegistrationHandler>();
        
        protected override void ProcessTyped<TComponent>(EntityComponentRegistration registration)
        {
            var componentType = typeof(TComponent);
            var msgPackAttr = componentType.GetCustomAttribute<MessagePackObjectAttribute>();
            var dataContractAttr = componentType.GetCustomAttribute<DataContractAttribute>();
            var binarySerializationAttribute = componentType.GetCustomAttribute<EntityBinarySerializationAttribute>();

            if (msgPackAttr == null && dataContractAttr == null && binarySerializationAttribute == null)
            {
                return;
            }

            var componentTypeId = binarySerializationAttribute?.ComponentTypeId ?? 
                                  componentType.FullName ?? 
                                  throw new InvalidOperationException();
            
            var usedAsTag = binarySerializationAttribute?.UsedAsTag ?? false;
            
            BinaryPreProcessor<TComponent>? preProcessor = null;
            BinaryPostProcessor<TComponent>? postProcessor = null;
            FormatterResolverFactory? resolverFactory = null;
            MessagePackFormatterFactory? messageFormatterFactory = null;
            
            var handlerMethods = componentType.GetMethods(BindingFlags.Static | BindingFlags.Public);

            foreach (var methodInfo in handlerMethods)
            {
                if (IsResolverFactory(methodInfo))
                {
                    resolverFactory = (FormatterResolverFactory?)Delegate.CreateDelegate(typeof(FormatterResolverFactory), null, methodInfo, false);
                }

                if (IsMessageFormatterFactory(methodInfo))
                {
                    messageFormatterFactory = (MessagePackFormatterFactory?)Delegate.CreateDelegate(typeof(MessagePackFormatterFactory), null, methodInfo, false);
                }

                if (IsPostProcessor<TComponent>(methodInfo))
                {
                    postProcessor = (BinaryPostProcessor<TComponent>?)Delegate.CreateDelegate(typeof(BinaryPostProcessor<TComponent>), null, methodInfo, false);
                }

                if (IsPreProcessor<TComponent>(methodInfo))
                {
                    preProcessor = (BinaryPreProcessor<TComponent>?)Delegate.CreateDelegate(typeof(BinaryPreProcessor<TComponent>), null, methodInfo, false);
                }
            }

            //在EntityComponentRegistration中保存二进制数据读取handler
            registration.Store(BinaryReadHandlerRegistration.Create(componentTypeId, usedAsTag, postProcessor)
                                                 .WithFormatterResolver(resolverFactory)
                                                 .WithMessagePackFormatter(messageFormatterFactory)
            );
            //在EntityComponentRegistration中保存二进制数据写入handler
            registration.Store(BinaryWriteHandlerRegistration.Create(componentTypeId, usedAsTag, preProcessor)
                                                  .WithFormatterResolver(resolverFactory)
                                                  .WithMessagePackFormatter(messageFormatterFactory)
            );

            if (msgPackAttr == null)
            {
                Logger.Debug("Registered Binary DataContract Handling for {ComponentType}", componentType);
            }
            else
            {
                Logger.Debug("Registered Binary MessagePack Handling for {ComponentType}", componentType);
            }
        }
        
         bool IsMessageFormatterFactory(MethodInfo methodInfo)
         {
             var paramType = typeof(IEntityKeyMapper);
             var returnType = typeof(IMessagePackFormatter);
             return methodInfo.GetCustomAttribute<EntityBinaryFormatterAttribute>() != null
                    && methodInfo.IsSameFunction(returnType, paramType);
         }

         bool IsResolverFactory(MethodInfo methodInfo)
         {
             var paramType = typeof(IEntityKeyMapper);
             var returnType = typeof(IFormatterResolver);
             return methodInfo.GetCustomAttribute<EntityBinaryFormatterResolverAttribute>() != null
                    && methodInfo.IsSameFunction(returnType, paramType);
         }

         bool IsPreProcessor<TComponent>(MethodInfo methodInfo)
         {
             var componentType = typeof(TComponent);
             return methodInfo.GetCustomAttribute<EntityBinaryPreProcessorAttribute>() != null
                    && methodInfo.IsSameFunction(componentType, componentType);
         }

         bool IsPostProcessor<TComponent>(MethodInfo methodInfo)
         {
             var componentType = typeof(TComponent);
             return methodInfo.GetCustomAttribute<EntityBinaryPostProcessorAttribute>() != null
                    && methodInfo.IsSameFunction(componentType, componentType);
         }

    }
}