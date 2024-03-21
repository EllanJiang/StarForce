/*
  作者：LTH
  文件描述：
  文件名：BinaryWriteHandlerRegistry
  创建时间：2023/07/16 21:07:SS
*/

using System;
using System.Collections.Generic;
using Entt.Annotations;

namespace Entt.Serialization.Binary
{
    /// <summary>
    /// 二进制写入handler管理器
    /// </summary>
    public class BinaryWriteHandlerRegistry
    {
        /// <summary>
        /// 所有的handler
        /// </summary>
        private readonly Dictionary<Type, BinaryWriteHandlerRegistration> handlers;
        
        public IEnumerable<BinaryWriteHandlerRegistration> Handlers => handlers.Values;
        
        public BinaryWriteHandlerRegistry()
        {
            this.handlers = new Dictionary<Type, BinaryWriteHandlerRegistration>();
        }

        public BinaryWriteHandlerRegistry RegisterRange(IEnumerable<EntityComponentRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }

            return this;
        }

        public BinaryWriteHandlerRegistry Register(EntityComponentRegistration reg)
        {
            if (reg.TryGet(out BinaryWriteHandlerRegistration r))
            {
                Register(r);
            }

            return this;
        }

        public BinaryWriteHandlerRegistry Register(in BinaryWriteHandlerRegistration reg)
        {
            handlers.Add(reg.TargetType, reg);
            return this;
        }

        public BinaryWriteHandlerRegistry RegisterRange(IEnumerable<BinaryWriteHandlerRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }
            return this;
        }

        public BinaryWriteHandlerRegistry Register<TComponent>(string typeId, bool useAsTag = false)
        {
            Register(BinaryWriteHandlerRegistration.Create<TComponent>(typeId, useAsTag));
            return this;
        }

        public BinaryWriteHandlerRegistration QueryHandler<TComponent>()
        {
            if (handlers.TryGetValue(typeof(TComponent), out var handler))
            {
                return handler;
            }
            throw new ArgumentException("Unable to find write-handler for type " + typeof(TComponent));
        }
    }
}