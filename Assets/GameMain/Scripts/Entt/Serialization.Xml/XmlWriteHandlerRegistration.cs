/*
  作者：LTH
  文件描述：
  文件名：XmlWriteHandlerRegistration
  创建时间：2023/07/16 16:07:SS
*/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Entt.Serialization.Xml
{
    /// <summary>
    /// xml写入委托
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    public delegate void WriteHandlerDelegate<TComponent>(XmlWriter writer, TComponent component);
    
    /// <summary>
    /// 保存xml写入handler
    /// </summary>
    public readonly struct XmlWriteHandlerRegistration
    {
        private readonly object? handler;
        
        public Type TargetType { get; }     //目标类型
        public string TypeId { get; }       //类型id，一般是Type.FullName
        public bool Tag { get; }            //是否是标签
        readonly object? surrogateResolver; //解析代理

        XmlWriteHandlerRegistration(Type targetType, string typeId, object? handler, bool tag, object? surrogateResolver)
        {
            if (string.IsNullOrWhiteSpace(typeId))
            {
                throw new ArgumentException("Type id should never be an empty string.");
            }
            
            this.TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            this.TypeId = typeId;
            this.handler = handler;
            this.Tag = tag;
            this.surrogateResolver = surrogateResolver;
        }
        
        /// <summary>
        /// 获取handler，一般是一个方法，该方法参数是xmlWriter和TData，没有返回值
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public WriteHandlerDelegate<TData>? GetHandler<TData>()
        {
            return (WriteHandlerDelegate<TData>?)handler;
        }
        
        /// <summary>
        /// 获取解析工厂
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public bool TryGetResolverFactory([MaybeNullWhen(false)] out FormatterResolverFactory fn)
        {
            if (surrogateResolver is FormatterResolverFactory fnx)
            {
                fn = fnx;
                return true;
            }

            fn = default;
            return false;
        }
        
        /// <summary>
        /// 使用FormatterResolverFactory创建一个XmlWriteHandlerRegistration
        /// </summary>
        /// <param name="resolverFactory"></param>
        /// <returns></returns>
        public XmlWriteHandlerRegistration WithFormatterResolver(FormatterResolverFactory? resolverFactory)
        {
            return new XmlWriteHandlerRegistration(TargetType, TypeId, handler, Tag, resolverFactory);
        }

        /// <summary>
        /// 使用handler和tag创建XmlWriteHandlerRegistration
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="tag"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static XmlWriteHandlerRegistration Create<TData>(WriteHandlerDelegate<TData> handler, bool tag)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return new XmlWriteHandlerRegistration(typeof(TData), typeof(TData).FullName, handler, tag, null);
        }
        
        /// <summary>
        /// 使用typeId、handler和tag创建XmlWriteHandlerRegistration
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="handler"></param>
        /// <param name="tag"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static XmlWriteHandlerRegistration Create<TData>(string? typeId, WriteHandlerDelegate<TData> handler, bool tag)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return new XmlWriteHandlerRegistration(typeof(TData), typeId ?? typeof(TData).FullName, handler, tag, null);
        }
    }
}