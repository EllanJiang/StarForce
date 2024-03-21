/*
  作者：LTH
  文件描述：
  文件名：XmlAutoRegistrationTest
  创建时间：2023/07/17 22:07:SS
*/

using Entt.Annotations;
using Entt.Serialization.Xml;
using Entt.Serialization.Xml.AutoRegistration;
using FluentAssertions;
using UnityEngine;

namespace Entt.Test.Serialisation
{
    /// <summary>
    /// Xml序列化测试
    /// </summary>
    public class XmlAutoRegistrationTest:MonoBehaviour
    {
        private void Start()
        {
            TestRegisterHandlers();
        }
        
        public void TestRegisterHandlers()
        {
            var components = new EntityRegistrationScanner()
                .With(new XmlEntityRegistrationHandler())
                .With(new XmlDataContractRegistrationHandler())
                .RegisterEntitiesFromAllAssemblies();
            components.Count.Should().Be(1);

            var xmlReadRegistry = new XmlReadHandlerRegistry();
            xmlReadRegistry.RegisterRange(components);

            var xmlWriteRegistry = new XmlWriteHandlerRegistry();
            xmlWriteRegistry.RegisterRange(components);
            
            
        }
    }
}