/*
  作者：LTH
  文件描述：
  文件名：DefaultDataContractReadHandler
  创建时间：2023/07/16 16:07:SS
*/

using System.Runtime.Serialization;
using System.Xml;

namespace Entt.Serialization.Xml.Impl
{
    /// <summary>
    /// 默认DataContract读取handler
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    public class DefaultDataContractReadHandler<TComponent>
    {
        readonly DataContractSerializer serializer;

        public DefaultDataContractReadHandler(ObjectSurrogateResolver? objectResolver = null)
        {
            var serializerSettings = new DataContractSerializerSettings()
            {
                SerializeReadOnlyTypes = true   //只序列化只读属性或字段
            };

            serializer = new DataContractSerializer(typeof(TComponent), serializerSettings);
            if (objectResolver != null)
            {
                serializer.SetSerializationSurrogateProvider(objectResolver);
            }
        }

        /// <summary>
        /// 把reader反序列化为TComponent
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public TComponent Read(XmlReader reader)
        {
            return (TComponent)serializer.ReadObject(reader);
        }
    }
}