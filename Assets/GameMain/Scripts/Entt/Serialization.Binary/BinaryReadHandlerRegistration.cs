/*
  作者：LTH
  文件描述：
  文件名：BinaryReadHandlerRegistration
  创建时间：2023/07/16 21:07:SS
*/


using System;
using System.Diagnostics.CodeAnalysis;

namespace Entt.Serialization.Binary
{
    /// <summary>
    /// 后处理委托
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    public delegate TComponent BinaryPostProcessor<TComponent>(in TComponent data);  
    /// <summary>
    /// 预处理委托
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    public delegate TComponent BinaryPreProcessor<TComponent>(in TComponent data);
    
    /// <summary>
    /// 保存二进制数据读取handler
    /// </summary>
    public readonly struct BinaryReadHandlerRegistration
    {
        public readonly string TypeId;
        public readonly Type TargetType;
        public readonly bool Tag;
        readonly object? postProcessor;
        readonly object? formatterResolverFactory;
        readonly object? messagePackFormatterFactory;

        BinaryReadHandlerRegistration(string typeId, 
            Type targetType, 
            bool tag, 
            object? postProcessor, 
            object? resolverFactory,
            object? messageFormatter)
        {
            TypeId = typeId;
            TargetType = targetType;
            Tag = tag;
            this.postProcessor = postProcessor;
            formatterResolverFactory = resolverFactory;
            this.messagePackFormatterFactory = messageFormatter;
        }
        
        /// <summary>
        /// 获取后处理委托
        /// </summary>
        /// <param name="fn"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public bool TryGetPostProcessor<TComponent>([MaybeNullWhen(false)] out BinaryPostProcessor<TComponent> fn)
        {
            if (postProcessor is BinaryPostProcessor<TComponent> fnx)
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
        /// 获取MessagePack解析器
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
        /// 使用FormatterResolverFactory创建BinaryReadHandlerRegistration
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public BinaryReadHandlerRegistration WithFormatterResolver(FormatterResolverFactory? resolver)
        {
            return new BinaryReadHandlerRegistration(TypeId, TargetType, Tag, postProcessor, resolver, messagePackFormatterFactory);
        }

        /// <summary>
        /// 使用MessagePackFormatterFactory创建BinaryReadHandlerRegistration
        /// </summary>
        /// <param name="messagePackFormatter"></param>
        /// <returns></returns>
        public BinaryReadHandlerRegistration WithMessagePackFormatter(MessagePackFormatterFactory? messagePackFormatter)
        {
            return new BinaryReadHandlerRegistration(TypeId, TargetType, Tag, postProcessor, formatterResolverFactory, messagePackFormatter);
        }

        /// <summary>
        /// 创建BinaryReadHandlerRegistration
        /// </summary>
        /// <param name="isTag"></param>
        /// <param name="postProcessor"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static BinaryReadHandlerRegistration Create<TComponent>(bool isTag, 
                                                                       BinaryPostProcessor<TComponent>? postProcessor = null)
        {
            return new BinaryReadHandlerRegistration(typeof(TComponent).FullName, typeof(TComponent), isTag, postProcessor, null, null);
        }

        /// <summary>
        /// 创建BinaryReadHandlerRegistration
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="isTag"></param>
        /// <param name="postProcessor"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static BinaryReadHandlerRegistration Create<TComponent>(string typeId, 
                                                                       bool isTag, 
                                                                       BinaryPostProcessor<TComponent>? postProcessor = null)
        {
            if (string.IsNullOrEmpty(typeId))
            {
                typeId = typeof(TComponent).FullName;
            }
            return new BinaryReadHandlerRegistration(typeId, typeof(TComponent), isTag, postProcessor, null, null);
        }

    }
}