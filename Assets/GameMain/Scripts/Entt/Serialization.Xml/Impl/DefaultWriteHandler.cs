/*
  作者：LTH
  文件描述：
  文件名：DefaultWriteHandler
  创建时间：2023/07/16 16:07:SS
*/

using System.Xml;
using System.Xml.Serialization;

namespace Entt.Serialization.Xml.Impl
{
    /// <summary>
    /// 默认xml写入handler
    /// </summary>
    public class DefaultWriteHandler<T>
    {
        private readonly XmlSerializer serializer;
        
        public DefaultWriteHandler()
        {
            serializer = new XmlSerializer(typeof(T));
        }
        
        /// <summary>
        /// 把component序列化到writer中
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="component"></param>
        public void Write(XmlWriter writer, T component)
        {
            object? o = component;
            if (o != null)
            {
                serializer.Serialize(writer, o);
            }
        }
    }
}