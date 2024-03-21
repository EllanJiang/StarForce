/*
  作者：LTH
  文件描述：
  文件名：DefaultReadHandler
  创建时间：2023/07/16 16:07:SS
*/

using System;
using System.Xml;
using System.Xml.Serialization;

namespace Entt.Serialization.Xml.Impl
{
    /// <summary>
    /// 默认xml读取handler
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    public class DefaultReadHandler<TComponent>
    {
        private readonly XmlSerializer serializer;
        
        public DefaultReadHandler() : this(new XmlSerializer(typeof(TComponent)))
        {
        }

        public DefaultReadHandler(XmlSerializer serializer)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        /// <summary>
        /// 把reader反序列化为TComponent
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public TComponent Read(XmlReader reader)
        {
            return (TComponent)serializer.Deserialize(reader);
        }
    }
}