/*
  作者：LTH
  文件描述：
  文件名：NestedKeyBinaryTest
  创建时间：2023/07/17 23:07:SS
*/

using System;
using System.IO;
using Entt.Annotations;
using Entt.Annotations.Impl;
using Entt.Entities;
using Entt.Serialization;
using Entt.Serialization.Binary;
using Entt.Serialization.Binary.AutoRegistration;
using Entt.Serialization.Binary.Impl;
using FluentAssertions;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;
using UnityEngine.Assertions;


namespace Entt.Test.Serialisation.NestedKeys
{
    /// <summary>
    /// 二进制序列化
    /// </summary>
    public class NestedKeyBinaryTest:MonoBehaviour
    {
        private void Start()
        {
            TestBinarySerialization();
        }

        EntityRegistry<EntityKey> CreteEntityRegistry()
        {
            var scanner = new EntityRegistrationScanner(new ComponentRegistrationHandler<EntityKey>());
            if (!scanner.TryRegisterComponent<NestedKeyComponent>(out var reg))
            {
                Debug.LogError($"Can Not Register Component {typeof(NestedKeyComponent)} for scanner");
            }

            var registry = new EntityRegistry<EntityKey>(EntityKey.MaxAge, EntityKey.Create);
            registry.Register(reg);
            return registry;
        }
        
        public void TestBinarySerialization()
        {
            var registry = CreteEntityRegistry();
            var parent = registry.Create();
            var child = registry.CreateAsActor().AssignComponent(new NestedKeyComponent(parent));

            var xmlString = Serialize(registry);
            xmlString.Length.Should().Be(72);

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

        /// <summary>
        /// 序列化EntityRegistry为二进制数组
        /// </summary>
        /// <param name="registry"></param>
        /// <returns></returns>
        byte[] Serialize(EntityRegistry<EntityKey> registry)
        {
            var scanner = new EntityRegistrationScanner().With(new BinaryEntityRegistrationHandler());
            if (!scanner.TryRegisterComponent<NestedKeyComponent>(out var registration))
            {
                Debug.LogError($"Can Not Register Component {typeof(NestedKeyComponent)} for scanner");
            }

            var handlerRegistry = new BinaryWriteHandlerRegistry();
            handlerRegistry.Register(registration);

            var stream = new MemoryStream();

            using (var snapshot = registry.CreateSnapshot())
            {
                var resolver = CompositeResolver.Create(
                    new EntityKeyDataResolver(),
                    new EntityKeyResolver(),
                    OptionalResolver.Instance,
                    StandardResolver.Instance
                );

                var msgPackOptions = MessagePackSerializerOptions.Standard
                                                                 .WithResolver(resolver)
                                                                 .WithCompression(MessagePackCompression.None);
                var writer = new BinaryArchiveWriter<EntityKey>(handlerRegistry, stream, msgPackOptions);

                snapshot.WriteAll(writer);
            }

            stream.Flush();

            return stream.ToArray();
        }

        /// <summary>
        /// 反序列化二进制数组到EntityRegistry中
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="data"></param>
        void Deserialize(EntityRegistry<EntityKey> registry, byte[] data)
        {
            var scanner = new EntityRegistrationScanner().With(new BinaryEntityRegistrationHandler());
            if (!scanner.TryRegisterComponent<NestedKeyComponent>(out var registration))
            {
                Debug.LogError($"Can Not Register Component {typeof(NestedKeyComponent)} for scanner");
            }

            var handlerRegistry = new BinaryReadHandlerRegistry();
            handlerRegistry.Register(registration);

            var readerBackend = new BinaryReaderBackend<EntityKey>(handlerRegistry);

            using (var loader = registry.CreateLoader())
            {
                var resolver = CompositeResolver.Create(
                    new EntityKeyDataResolver(),
                    new EntityKeyResolver(new DefaultEntityKeyMapper().Register(loader.Map)),
                    OptionalResolver.Instance,
                    StandardResolver.Instance
                );

                var msgPackOptions = MessagePackSerializerOptions.Standard.WithResolver(resolver);
                var reader = new BinaryBulkArchiveReader<EntityKey>(readerBackend, msgPackOptions);

                var stream = new MemoryStream(data);

                reader.ReadAll(stream, loader);
            }
        }
    }
}