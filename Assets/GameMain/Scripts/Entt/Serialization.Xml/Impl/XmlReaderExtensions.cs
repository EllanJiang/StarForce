/*
  作者：LTH
  文件描述：
  文件名：XmlReaderExtensions
  创建时间：2023/07/16 16:07:SS
*/

using System;
using System.Xml;

namespace Entt.Serialization.Xml.Impl
{
    public static class XmlReaderExtensions
    {
        /// <summary>
        /// 一直读取，直到reader.NodeType == XmlNodeType.Element
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="localName"></param>
        /// <exception cref="SnapshotIOException"></exception>
        public static void AdvanceToElement(this XmlReader reader, string? localName = null)
        {
            while (reader.Read())
            {
                if (reader.EOF)
                {
                    throw new SnapshotIOException("EOF");
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (localName != null && reader.LocalName != localName)
                    {
                        throw new SnapshotIOException($"Expected {localName}, but found {reader.LocalName} instead.");
                    }

                    return;
                }
            }
        }

        /// <summary>
        /// 读取子节点，直到reader.NodeType == XmlNodeType.Element
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="onStartElement">如果onStartElement(reader)返回false，那么skip reader</param>
        public static void ReadChildElements(this XmlReader reader, Func<XmlReader, bool> onStartElement)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        if (!onStartElement(reader))
                        {
                            reader.Skip();
                        }
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 读取子节点，直到reader.NodeType == XmlNodeType.Element
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="onStartElement">如果onStartElement()返回false，那么skip reader</param>
        public static void ReadChildElements(this XmlReader reader, Func<bool> onStartElement)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        if (!onStartElement())
                        {
                            reader.Skip();
                        }
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 获取int类型的属性
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetAttributeInt(this XmlReader reader, string name, out int value)
        {
            string? rawValue = reader.GetAttribute(name);
            if (string.IsNullOrEmpty(rawValue))
            {
                value = default;
                return false;
            }

            return int.TryParse(rawValue, out value);
        }

        /// <summary>
        /// 获取bool类型的属性
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetAttributeBool(this XmlReader reader, string name, out bool value)
        {
            string? rawValue = reader.GetAttribute(name);
            if (string.IsNullOrEmpty(rawValue))
            {
                value = default;
                return false;
            }

            return bool.TryParse(rawValue, out value);
        }
    }
}