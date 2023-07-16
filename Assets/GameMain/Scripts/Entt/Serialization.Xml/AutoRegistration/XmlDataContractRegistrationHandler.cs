/*
  作者：LTH
  文件描述：
  文件名：XmlDataContractRegistrationHandler
  创建时间：2023/07/16 17:07:SS
*/

using System;
using System.Reflection;
using System.Runtime.Serialization;
using Entt.Annotations;
using Entt.Annotations.Impl;
using Entt.Serialization.Xml.Impl;
using Serilog;

namespace Entt.Serialization.Xml.AutoRegistration
{
    /// <summary>
    /// 为DataContract标注的组件注册xml读写handler
    /// </summary>
    public class XmlDataContractRegistrationHandler: EntityRegistrationHandlerBase
    {
        readonly ObjectSurrogateResolver? objectResolver;
        static readonly ILogger logger = Log.ForContext<XmlDataContractRegistrationHandler>();
        
        public XmlDataContractRegistrationHandler(ObjectSurrogateResolver? objectResolver = null)
        {
            this.objectResolver = objectResolver;
        }
        
        protected override void ProcessTyped<TComponent>(EntityComponentRegistration registration)
        {
            var componentType = typeof(TComponent);
            //对于EntityXmlSerializationAttribute属性标注的类或结构体，不使用该handler进行处理
            if (componentType.GetCustomAttribute<EntityXmlSerializationAttribute>() != null)
            {
                // Handled in the data processor for proper components.
                return;
            }

            var attr = componentType.GetCustomAttribute<DataContractAttribute>();
            if (attr == null)
            {
                return;
            }

            var handlerMethods = componentType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            FormatterResolverFactory? formatterResolver = null;
            foreach (var m in handlerMethods)
            {
                if (IsSurrogateProvider(m))
                {
                    formatterResolver = (FormatterResolverFactory?)Delegate.CreateDelegate(typeof(FormatterResolverFactory), null, m, false);
                }
            }


            ReadHandlerDelegate<TComponent> readHandler = new DefaultDataContractReadHandler<TComponent>(objectResolver).Read;
            WriteHandlerDelegate<TComponent> writeHandler = new DefaultDataContractWriteHandler<TComponent>(objectResolver).Write;
            
            registration.Store(XmlReadHandlerRegistration.Create(null, readHandler, false).WithFormatterResolver(formatterResolver));
            registration.Store(XmlWriteHandlerRegistration.Create(null, writeHandler, false).WithFormatterResolver(formatterResolver));

            logger.Debug("Registered Xml DataContract Handling for {ComponentType}", componentType);
        }

        bool IsSurrogateProvider(MethodInfo methodInfo)
        {
            var paramType = typeof(IEntityKeyMapper);
            var returnType = typeof(ISerializationSurrogateProvider);
            return methodInfo.GetCustomAttribute<EntityXmlSurrogateProviderAttribute>() != null
                   && methodInfo.IsSameFunction(returnType, paramType);
        }
    }
}