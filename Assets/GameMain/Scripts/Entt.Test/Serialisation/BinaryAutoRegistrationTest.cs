/*
  作者：LTH
  文件描述：
  文件名：BinaryAutoRegistrationTest
  创建时间：2023/07/17 22:07:SS
*/

using System;
using Entt.Annotations;
using Entt.Serialization.Binary;
using Entt.Serialization.Binary.AutoRegistration;
using FluentAssertions;
using UnityEngine;

namespace Entt.Test.Serialisation
{
    public class BinaryAutoRegistrationTest:MonoBehaviour
    {
        private void Start()
        {
            TestRegisterHandlers();
        }

        public void TestRegisterHandlers()
        {
            var components = new EntityRegistrationScanner()
                .With(new BinaryEntityRegistrationHandler())
                .RegisterEntitiesFromAllAssemblies();
            components.Count.Should().Be(1);

            var xmlReadRegistry = new BinaryReadHandlerRegistry();
            xmlReadRegistry.RegisterRange(components);

            var xmlWriteRegistry = new BinaryWriteHandlerRegistry();
            xmlWriteRegistry.RegisterRange(components);
        }
    }
}