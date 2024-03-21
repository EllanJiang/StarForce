/*
  作者：LTH
  文件描述：
  文件名：DefaultDataContractWriteHandler
  创建时间：2023/07/16 17:07:SS
*/

using System.Runtime.Serialization;
using System.Xml;

namespace Entt.Serialization.Xml.Impl
{
    /// <summary>
    /// 默认DataContract写入handler
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class DefaultDataContractWriteHandler<TData>
    {
        readonly DataContractSerializer serializer;

        public DefaultDataContractWriteHandler(ObjectSurrogateResolver? surrogateResolver = null)
        {
            var ds = new DataContractSerializerSettings();
            ds.SerializeReadOnlyTypes = true;

            serializer = new DataContractSerializer(typeof(TData), ds);
            if (surrogateResolver != null)
            {
                serializer.SetSerializationSurrogateProvider(surrogateResolver);
            }
        }

        /// <summary>
        /// 把component序列化到writer中
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="component"></param>
        public void Write(XmlWriter writer, TData component)
        {
            serializer.WriteObject(writer, component!);
        }
    }
}