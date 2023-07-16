/*
  作者：LTH
  文件描述：
  文件名：XmlWriteHandlerRegistry
  创建时间：2023/07/16 16:07:SS
*/

using System;
using System.Collections.Generic;
using Entt.Annotations;
using Entt.Serialization.Xml.Impl;

namespace Entt.Serialization.Xml
{
    /// <summary>
    /// xml写入handler的管理器
    /// </summary>
    public class XmlWriteHandlerRegistry
    {
        /// <summary>
        /// 根据类型保存所有handler
        /// </summary>
        readonly Dictionary<Type, XmlWriteHandlerRegistration> handlers;
        
        public XmlWriteHandlerRegistry()
        {
            this.handlers = new Dictionary<Type, XmlWriteHandlerRegistration>();
        }

        public IEnumerable<XmlWriteHandlerRegistration> Handlers => handlers.Values;
        
        public XmlWriteHandlerRegistry RegisterRange(IEnumerable<EntityComponentRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }
            return this;
        }
        
        public XmlWriteHandlerRegistry RegisterRange(IEnumerable<XmlWriteHandlerRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }
            return this;
        }
        
        /// <summary>
        /// 注册一个组件，用于写入xml
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public XmlWriteHandlerRegistry Register(EntityComponentRegistration reg)
        {
            if (reg.TryGet(out XmlWriteHandlerRegistration r))
            {
                Register(r);
            }
            return this;
        }

        /// <summary>
        /// 注册一个XmlWriteHandler
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public XmlWriteHandlerRegistry Register(in XmlWriteHandlerRegistration reg)
        {
            handlers.Add(reg.TargetType, reg);
            return this;
        }
        
        /// <summary>
        /// 使用typeId，useAsTag来创建一个XmlWriteHandlerRegistration，并注册到registry中
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="useAsTag"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public XmlWriteHandlerRegistry Register<TComponent>(string typeId, bool useAsTag = false)
        {
            Register(XmlWriteHandlerRegistration.Create<TComponent>(typeId, new DefaultWriteHandler<TComponent>().Write, useAsTag));
            return this;
        }

        /// <summary>
        /// 查询并返回指定类型的handler
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public XmlWriteHandlerRegistration QueryHandler<TComponent>()
        {
            if (handlers.TryGetValue(typeof(TComponent), out var handler))
            {
                return handler;
            }
            throw new ArgumentException("Unable to find write-handler for type " + typeof(TComponent));
        }
    }
}