/*
  作者：LTH
  文件描述：
  文件名：NestedKeyXmlTest
  创建时间：2023/07/17 23:07:SS
*/

using System;
using System.IO;
using System.Text;
using System.Xml;
using Entt.Annotations;
using Entt.Annotations.Impl;
using Entt.Entities;
using Entt.Serialization;
using Entt.Serialization.Xml;
using Entt.Serialization.Xml.AutoRegistration;
using Entt.Serialization.Xml.Impl;
using Entt.Test.Serialisation.Surrogates;
using FluentAssertions;
using FluentAssertions.Execution;
using UnityEngine;
using UnityEngine.Assertions;

namespace Entt.Test.Serialisation.NestedKeys
{
    public class NestedKeyXmlTest:MonoBehaviour
    {
        private void Start()
        {
            TestXmlSerialization();
            TestDataContractResolving();
            TestDataContractResolving_With_Mapping();
        }
        
        //测试xml序列化
        public void TestXmlSerialization()
        {
            var registry = CreteEntityRegistry();
            var parent = registry.Create();
            var child = registry.CreateAsActor().AssignComponent(new NestedKeyComponent(parent));

            var xmlString = Serialize(registry);

            Console.WriteLine("----------------------");
            Console.WriteLine(xmlString);
            Console.WriteLine("----------------------");

            registry.Clear();

            Deserialize(registry, xmlString);

            registry.Count.Should().Be(2);
            registry.GetComponent(parent, out NestedKeyComponent _).Should().BeFalse();
            registry.GetComponent(child, out NestedKeyComponent childComponent).Should().BeTrue();
            childComponent.ParentReference.Should().Be(parent);


            var freshRegistry = CreteEntityRegistry();
            freshRegistry.Create();
            Deserialize(freshRegistry, xmlString);

            freshRegistry.Count.Should().Be(3);
            freshRegistry.GetComponent(new EntityKey(1, 1), out NestedKeyComponent _).Should().BeFalse();
            freshRegistry.GetComponent(new EntityKey(1, 2), out NestedKeyComponent freshComponent).Should().BeTrue();
            freshComponent.ParentReference.Should().Be(new EntityKey(1, 1));
        }
        
        //测试DataContract序列化
        public void TestDataContractResolving()
        {
            var model = new NestedKeyComponent(new EntityKey(100, 200));
            var surrogate = new ObjectSurrogateResolver();
            surrogate.Register(new DummyEnumObjectSurrogateProvider());
            surrogate.Register(new EntityKeySurrogateProvider());

            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb,
                new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "  ",
                    NamespaceHandling = NamespaceHandling.OmitDuplicates
                });


            var dc = new DefaultDataContractWriteHandler<NestedKeyComponent>(surrogate);
            dc.Write(xmlWriter, model);
            xmlWriter.Flush();

            Debug.Log("TestDataContractResolving ---------------------- start");
            Debug.Log(sb.ToString());
            Debug.Log("TestDataContractResolving ---------------------- end");

            var xmlReader = XmlReader.Create(new StringReader(sb.ToString()));
            var dr = new DefaultDataContractReadHandler<NestedKeyComponent>(surrogate);
            var loaded = dr.Read(xmlReader);
            loaded.ParentReference.Should().Be(new EntityKey(100, 200));
        }
        
        //测试DataContract序列化
        public void TestDataContractResolving_With_Mapping()
        {
            EntityKey Map(EntityKeyData s) => new EntityKey((byte)(s.Age + 1), s.Key + 1);

            var model = new NestedKeyComponent(new EntityKey(100, 200));
            var surrogate = new ObjectSurrogateResolver();
            surrogate.Register(new DummyEnumObjectSurrogateProvider());
            surrogate.Register(new EntityKeySurrogateProvider(new DefaultEntityKeyMapper().Register(Map)));

            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb,
                new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "  ",
                    NamespaceHandling = NamespaceHandling.OmitDuplicates
                });


            var dc = new DefaultDataContractWriteHandler<NestedKeyComponent>(surrogate);
            dc.Write(xmlWriter, model);
            xmlWriter.Flush();

            Debug.Log("TestDataContractResolving_With_Mapping ---------------------- start");
            Debug.Log(sb.ToString());
            Debug.Log("TestDataContractResolving_With_Mapping ---------------------- end");

            var xmlReader = XmlReader.Create(new StringReader(sb.ToString()));
            var dr = new DefaultDataContractReadHandler<NestedKeyComponent>(surrogate);
            var loaded = dr.Read(xmlReader);
            loaded.Should().Be(new EntityKey(100, 200));
            loaded.ParentReference.Should().Be(new EntityKey(101, 201));
        }
        
        EntityRegistry<EntityKey> CreteEntityRegistry()
        {
            var scanner = new EntityRegistrationScanner(new ComponentRegistrationHandler<EntityKey>());
            if (!scanner.TryRegisterComponent<NestedKeyComponent>(out var reg))
            {
                throw new AssertionFailedException("Unexpected error");
            }

            var registry = new EntityRegistry<EntityKey>(EntityKey.MaxAge, EntityKey.Create);
            registry.Register(reg);
            return registry;
        }
        
        //序列化EntityRegistry
        string Serialize(EntityRegistry<EntityKey> registry)
        {
            var surrogateResolver = new ObjectSurrogateResolver();
            surrogateResolver.Register(new EntityKeySurrogateProvider());
            surrogateResolver.Register(new DummyEnumObjectSurrogateProvider());

            var xmlScanner = new EntityRegistrationScanner(new XmlDataContractRegistrationHandler(surrogateResolver),
                new XmlEntityRegistrationHandler(surrogateResolver));
            if (!xmlScanner.TryRegisterComponent<NestedKeyComponent>(out var xmlRegistration))
            {
                Debug.LogError($"Can Not Register Component {typeof(NestedKeyComponent)} for xmlScanner");
            }

            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb,
                new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "  ",
                    NamespaceHandling = NamespaceHandling.OmitDuplicates
                });
            var writerRegistry = new XmlWriteHandlerRegistry();
            writerRegistry.Register(xmlRegistration);

            var writer = new XmlArchiveWriter<EntityKey>(writerRegistry, xmlWriter);

            registry.CreateSnapshot().WriteAll(writer);
            xmlWriter.Flush();

            return sb.ToString();
        }
        
        /// <summary>
        /// 反序列化EntityRegistry
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="xmlString"></param>
        void Deserialize(EntityRegistry<EntityKey> registry, string xmlString)
        {
            using (var loader = registry.CreateLoader())
            {
                var surrogateResolver = new ObjectSurrogateResolver();
                surrogateResolver.Register(new EntityKeySurrogateProvider(new DefaultEntityKeyMapper().Register(loader.Map)));
                surrogateResolver.Register(new DummyEnumObjectSurrogateProvider());

                var xmlScanner = new EntityRegistrationScanner(new XmlDataContractRegistrationHandler(surrogateResolver),
                    new XmlEntityRegistrationHandler(surrogateResolver));
                if (!xmlScanner.TryRegisterComponent<NestedKeyComponent>(out var xmlRegistration))
                {
                    Debug.LogError($"Can Not Register Component {typeof(NestedKeyComponent)} for xmlScanner");
                }

                var readerRegistry = new XmlReadHandlerRegistry();
                readerRegistry.Register(xmlRegistration);

                var reader = new XmlBulkArchiveReader<EntityKey>(readerRegistry);
                var xmlReader = XmlReader.Create(new StringReader(xmlString));
                reader.ReadAll(xmlReader, loader, new DefaultEntityKeyMapper().Register(loader.Map));
            }
        }
    }
}