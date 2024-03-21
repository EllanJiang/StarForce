/*
  作者：LTH
  文件描述：
  文件名：XmlReadHandlerRegistration
  创建时间：2023/07/16 16:07:SS
*/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Xml;

namespace Entt.Serialization.Xml
{
    public delegate ISerializationSurrogateProvider? FormatterResolverFactory(IEntityKeyMapper entityMapper);
    public delegate TComponent ReadHandlerDelegate<TComponent>(XmlReader reader);
    
    /// <summary>
    /// 保存xml读取handler
    /// </summary>
    public readonly struct XmlReadHandlerRegistration
    {
        public readonly string TypeId;
        public readonly Type TargetType;
        public readonly bool Tag;
        readonly object? parserFunction;
        readonly object? surrogateResolver;

        public XmlReadHandlerRegistration(string typeId, Type targetType, object? parserFunction, bool tag, object? surrogateResolver)
        {
            TypeId = typeId;
            TargetType = targetType;
            this.parserFunction = parserFunction;
            this.Tag = tag;
            this.surrogateResolver = surrogateResolver;
        }

        public bool TryGetParser<TComponent>([MaybeNullWhen(false)] out ReadHandlerDelegate<TComponent> fn)
        {
            if (parserFunction is ReadHandlerDelegate<TComponent> fnx)
            {
                fn = fnx;
                return true;
            }

            fn = default;
            return false;
        }
        
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

        public XmlReadHandlerRegistration WithFormatterResolver(FormatterResolverFactory? resolverFactory)
        {
            return new XmlReadHandlerRegistration(TypeId, TargetType, parserFunction, Tag, resolverFactory);
        }

        public static XmlReadHandlerRegistration Create<TComponent>(ReadHandlerDelegate<TComponent> handler, bool tag)
        {
            return new XmlReadHandlerRegistration(typeof(TComponent).FullName, typeof(TComponent), handler, tag, null);
        }

        public static XmlReadHandlerRegistration Create<TComponent>(string? typeId, ReadHandlerDelegate<TComponent> handler, bool tag)
        {
            if (string.IsNullOrEmpty(typeId))
            {
                typeId = typeof(TComponent).FullName ?? typeof(TComponent).Name;
            }

            if (typeId == null) throw new InvalidOperationException();
            
            return new XmlReadHandlerRegistration(typeId, typeof(TComponent), handler, tag, null);
        }

    }
}