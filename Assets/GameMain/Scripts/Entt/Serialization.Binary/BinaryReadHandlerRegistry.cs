/*
  作者：LTH
  文件描述：
  文件名：BinaryReadHandlerRegistry
  创建时间：2023/07/16 21:07:SS
*/

using System;
using System.Collections.Generic;
using Entt.Annotations;

namespace Entt.Serialization.Binary
{
    /// <summary>
    /// 二进制读取handler管理器
    /// </summary>
    public class BinaryReadHandlerRegistry
    {
        //key：TypeId，一般是Type.FullName
        readonly Dictionary<string, BinaryReadHandlerRegistration> handlers;
        readonly Dictionary<Type, BinaryReadHandlerRegistration> handlersByType;
        
        public BinaryReadHandlerRegistry()
        {
            handlers = new Dictionary<string, BinaryReadHandlerRegistration>();
            handlersByType = new Dictionary<Type, BinaryReadHandlerRegistration>();
        }

        public BinaryReadHandlerRegistry RegisterRange(IEnumerable<EntityComponentRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }

            return this;
        }

        public IEnumerable<BinaryReadHandlerRegistration> Handlers => handlersByType.Values;

        public BinaryReadHandlerRegistry Register(EntityComponentRegistration reg)
        {
            if (reg.TryGet(out BinaryReadHandlerRegistration r))
            {
                Register(r);
            }

            return this;
        }

        public BinaryReadHandlerRegistry RegisterRange(IEnumerable<BinaryReadHandlerRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }

            return this;
        }

        public BinaryReadHandlerRegistry Register(BinaryReadHandlerRegistration r)
        {
            handlers.Add(r.TypeId, r);
            handlersByType.Add(r.TargetType, r);
            return this;
        }

        public bool TryGetValue(Type typeId, out BinaryReadHandlerRegistration o)
        {
            return handlersByType.TryGetValue(typeId, out o);
        }

        public bool TryGetValue(string typeId, out BinaryReadHandlerRegistration o)
        {
            return handlers.TryGetValue(typeId, out o);
        }
    }
}