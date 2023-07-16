/*
  作者：LTH
  文件描述：
  文件名：BinaryWriteHandlerRegistration
  创建时间：2023/07/16 21:07:SS
*/

using System;
using System.Diagnostics.CodeAnalysis;
using MessagePack;
using MessagePack.Formatters;

namespace Entt.Serialization.Binary
{
    public delegate IFormatterResolver? FormatterResolverFactory(IEntityKeyMapper entityMapper);
    public delegate IMessagePackFormatter? MessagePackFormatterFactory(IEntityKeyMapper entityMapper);
    
    /// <summary>
    /// 保存二进制数据写入handler
    /// </summary>
    public readonly struct BinaryWriteHandlerRegistration
    {
        public readonly string TypeId;
        public readonly Type TargetType;
        public readonly bool Tag;
        readonly object? preProcessor;
        readonly object? formatterResolverFactory;
        readonly object? messagePackFormatterFactory;
        
        public BinaryWriteHandlerRegistration(string typeId,
            Type targetType,
            bool tag,
            object? preProcessor,
            object? resolver,
            object? messageFormatter)
        {
            TypeId = typeId;
            TargetType = targetType;
            Tag = tag;
            this.preProcessor = preProcessor;
            formatterResolverFactory = resolver;
            messagePackFormatterFactory = messageFormatter;
        }
        
        /// <summary>
        /// 获取预处理器
        /// </summary>
        /// <param name="fn"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public bool TryGetPreProcessor<TComponent>([MaybeNullWhen(false)] out BinaryPreProcessor<TComponent> fn)
        {
            if (preProcessor is BinaryPreProcessor<TComponent> fnx)
            {
                fn = fnx;
                return true;
            }

            fn = default;
            return false;
        }

        /// <summary>
        /// 获取解析器
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public bool TryGetResolverFactory([MaybeNullWhen(false)] out FormatterResolverFactory fn)
        {
            if (formatterResolverFactory is FormatterResolverFactory fnx)
            {
                fn = fnx;
                return true;
            }

            fn = default;
            return false;
        }

        /// <summary>
        /// 获取MassagePack解析器
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public bool TryGetMessagePackFormatterFactory([MaybeNullWhen(false)] out MessagePackFormatterFactory fn)
        {
            if (formatterResolverFactory is MessagePackFormatterFactory fnx)
            {
                fn = fnx;
                return true;
            }

            fn = default;
            return false;
        }

        /// <summary>
        /// 使用FormatterResolverFactory创建BinaryWriteHandlerRegistration
        /// </summary>
        /// <param name="resolverFactory"></param>
        /// <returns></returns>
        public BinaryWriteHandlerRegistration WithFormatterResolver(FormatterResolverFactory? resolverFactory)
        {
            return new BinaryWriteHandlerRegistration(TypeId, TargetType, Tag, preProcessor, resolverFactory, messagePackFormatterFactory);
        }

        /// <summary>
        /// 使用MessagePackFormatterFactory创建BinaryWriteHandlerRegistration
        /// </summary>
        /// <param name="messagePackFormatter"></param>
        /// <returns></returns>
        public BinaryWriteHandlerRegistration WithMessagePackFormatter(MessagePackFormatterFactory? messagePackFormatter)
        {
            return new BinaryWriteHandlerRegistration(TypeId, TargetType, Tag, preProcessor, formatterResolverFactory, messagePackFormatter);
        }

        /// <summary>
        /// 创建BinaryWriteHandlerRegistration
        /// </summary>
        /// <param name="isTag"></param>
        /// <param name="preProcessor"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static BinaryWriteHandlerRegistration Create<TComponent>(bool isTag,
                                                                        BinaryPreProcessor<TComponent>? preProcessor = null)
        {
            return new BinaryWriteHandlerRegistration(typeof(TComponent).FullName, typeof(TComponent), isTag, preProcessor, null, null);
        }

        /// <summary>
        /// 创建BinaryWriteHandlerRegistration
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="isTag"></param>
        /// <param name="preProcessor"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static BinaryWriteHandlerRegistration Create<TComponent>(string typeId,
                                                                        bool isTag,
                                                                        BinaryPreProcessor<TComponent>? preProcessor = null)
        {
            if (string.IsNullOrEmpty(typeId))
            {
                typeId = typeof(TComponent).FullName;
            }

            return new BinaryWriteHandlerRegistration(typeId, typeof(TComponent), isTag, preProcessor, null, null);
        }
    }
}