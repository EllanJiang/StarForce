/*
  作者：LTH
  文件描述：
  文件名：XmlEntityRegistrationHandler
  创建时间：2023/07/16 17:07:SS
*/

using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using Entt.Annotations;
using Entt.Annotations.Impl;
using Entt.Serialization.Xml.Impl;
using Serilog;

namespace Entt.Serialization.Xml.AutoRegistration
{
    /// <summary>
    /// 为指定类型的组件注册xml读写handler
    /// </summary>
    public class XmlEntityRegistrationHandler:EntityRegistrationHandlerBase
    {
        static readonly ILogger logger = Log.ForContext<XmlEntityRegistrationHandler>();
        readonly ObjectSurrogateResolver? objectResolver;
        
        public XmlEntityRegistrationHandler(ObjectSurrogateResolver? objectResolver = null)
        {
            this.objectResolver = objectResolver;
        }
        
        protected override void ProcessTyped<TComponent>(EntityComponentRegistration registration)
        {
            var componentType = typeof(TComponent);
            //只能处理被打上EntityXmlSerialization属性的类或结构体
            var attr = componentType.GetCustomAttribute<EntityXmlSerializationAttribute>();
            if (attr == null)
            {
                return;
            }

            ReadHandlerDelegate<TComponent>? readHandler = null;
            WriteHandlerDelegate<TComponent>? writeHandler = null;
            FormatterResolverFactory? formatterResolver = null;

            var handlerMethods = componentType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (var m in handlerMethods)
            {
                if (IsXmlReader<TComponent>(m))
                {
                    readHandler = (ReadHandlerDelegate<TComponent>?) Delegate.CreateDelegate(typeof(ReadHandlerDelegate<TComponent>), null, m, false);
                }

                if (IsXmlWriter<TComponent>(m))
                {
                    writeHandler = (WriteHandlerDelegate<TComponent>?)Delegate.CreateDelegate(typeof(WriteHandlerDelegate<TComponent>), null, m, false);
                }

                if (IsSurrogateProvider(m))
                {
                    formatterResolver = (FormatterResolverFactory?)Delegate.CreateDelegate(typeof(FormatterResolverFactory), null, m, false);
                }
            }

            if (readHandler == null)
            {
                if (HasDataContract<TComponent>())
                {
                    readHandler = new DefaultDataContractReadHandler<TComponent>(objectResolver).Read;
                }
                else
                {
                    readHandler = new DefaultReadHandler<TComponent>().Read;
                }
            }

            if (writeHandler == null)
            {
                if (HasDataContract<TComponent>())
                {
                    writeHandler = new DefaultDataContractWriteHandler<TComponent>(objectResolver).Write;
                }
                else
                {
                    writeHandler = new DefaultWriteHandler<TComponent>().Write;
                }
            }

            registration.Store(XmlReadHandlerRegistration.Create(attr.ComponentTypeId, readHandler, attr.UsedAsTag).WithFormatterResolver(formatterResolver));
            registration.Store(XmlWriteHandlerRegistration.Create(attr.ComponentTypeId, writeHandler, attr.UsedAsTag).WithFormatterResolver(formatterResolver));

            logger.Debug("Registered Xml Handling for {ComponentType}", componentType);

        }
        
        /// <summary>
        /// 该类/enum/结构体是否被打上DataContract属性
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        bool HasDataContract<TComponent>()
        {
            var componentType = typeof(TComponent);
            return componentType.GetCustomAttribute<DataContractAttribute>() != null;
        }

        /// <summary>
        /// 该方法是否是xml reader方法（只有被打上EntityXmlReader属性的方法才是xml reader方法）
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        bool IsXmlReader<TComponent>(MethodInfo methodInfo)
        {
            var componentType = typeof(TComponent);
            return methodInfo.GetCustomAttribute<EntityXmlReaderAttribute>() != null
                   && methodInfo.IsSameFunction(componentType, typeof(XmlReader));
        }

        /// <summary>
        /// 该方法是否是xml writer方法（只有被打上EntityXmlWriter属性的方法才是xml writer方法）
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        bool IsXmlWriter<TComponent>(MethodInfo methodInfo)
        {
            var componentType = typeof(TComponent);
            return methodInfo.GetCustomAttribute<EntityXmlWriterAttribute>() != null
                   && methodInfo.IsSameAction(typeof(XmlWriter), componentType);
        }

        /// <summary>
        /// 是否是代理解析器（是否被打上EntityXmlSurrogateProvider属性）
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        bool IsSurrogateProvider(MethodInfo methodInfo)
        {
            var paramType = typeof(IEntityKeyMapper);
            var returnType = typeof(ISerializationSurrogateProvider);
            return methodInfo.GetCustomAttribute<EntityXmlSurrogateProviderAttribute>() != null
                   && methodInfo.IsSameFunction(returnType, paramType);
        }
    }       
}