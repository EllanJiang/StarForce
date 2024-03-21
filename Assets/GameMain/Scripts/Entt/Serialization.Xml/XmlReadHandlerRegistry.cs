/*
  作者：LTH
  文件描述：
  文件名：XmlReadHandlerRegistry
  创建时间：2023/07/16 16:07:SS
*/

using System;
using System.Collections.Generic;
using Entt.Annotations;

namespace Entt.Serialization.Xml
{
    /// <summary>
    /// xml读取handler管理器
    /// </summary>
    public class XmlReadHandlerRegistry
    {
        readonly Dictionary<string, XmlReadHandlerRegistration> handlers;       //key是TypeId
        readonly Dictionary<Type, XmlReadHandlerRegistration> handlersByType;   //key是Type

        public IEnumerable<XmlReadHandlerRegistration> Handlers => handlersByType.Values;
        
        public XmlReadHandlerRegistry()
        {
            handlers = new Dictionary<string, XmlReadHandlerRegistration>();
            handlersByType = new Dictionary<Type, XmlReadHandlerRegistration>();
        }

        public void RegisterRange(IEnumerable<EntityComponentRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }
        }

        public XmlReadHandlerRegistry Register(EntityComponentRegistration reg)
        {
            if (reg.TryGet(out XmlReadHandlerRegistration r))
            {
                Register(r);
            }

            return this;
        }

        public XmlReadHandlerRegistry RegisterRange(IEnumerable<XmlReadHandlerRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }

            return this;
        }

        public XmlReadHandlerRegistry Register(XmlReadHandlerRegistration r)
        {
            handlers.Add(r.TypeId, r);
            handlersByType.Add(r.TargetType, r);

            return this;
        }

        public bool TryGetValue(Type type, out XmlReadHandlerRegistration o)
        {
            return handlersByType.TryGetValue(type, out o);
        }

        public bool TryGetValue(string typeId, out XmlReadHandlerRegistration o)
        {
            return handlers.TryGetValue(typeId, out o);
        }
    }
}