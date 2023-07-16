/*
  作者：LTH
  文件描述：
  文件名：XmlReaderException
  创建时间：2023/07/16 16:07:SS
*/

using System;
using System.Runtime.Serialization;
using System.Xml;

namespace Entt.Serialization.Xml
{
    public class XmlReaderException : SnapshotIOException
    {
        public XmlReaderException()
        {
        }

        protected XmlReaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public XmlReaderException(string message) : base(message)
        {
        }

        public XmlReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public XmlReaderException(string message, int hresult) : base(message, hresult)
        {
        }
    }
    
    public static class XmlReaderExceptionExtensions
    {
        public static XmlReaderException FromMissingAttribute(this XmlReader reader, string tag, string attribute)
        {
            if (reader is IXmlLineInfo li && li.HasLineInfo())
            {
                return new XmlReaderException($"Missing attribute '{attribute}' on element '{tag}' at {li.LineNumber}:{li.LinePosition}");
            }
            return new XmlReaderException($"Missing attribute '{attribute}' on element '{tag}'");
        }
    }
}