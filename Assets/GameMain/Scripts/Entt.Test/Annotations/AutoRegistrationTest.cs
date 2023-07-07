/*
  作者：LTH
  文件描述：
  文件名：AutoRegistrationTest
  创建时间：2023/07/07 22:07:SS
*/

using System;
using Entt.Annotations;
using Entt.Annotations.Impl;
using Entt.Entities;
using FluentAssertions;
using UnityEngine;

namespace Entt.Test.Annotations
{
    /// <summary>
    /// 通过反射所有属性为EntityAttribute的结构体或类，如TestStructFixture，
    /// 然后自动将TestStructFixture注册到registry中
    /// </summary>
    public class AutoRegistrationTest:MonoBehaviour
    {
        private void Start()
        {
            Test();
        }

        private void Test()
        {
            //遍历所有程序集，找到所有属性为EntityAttribute的结构体或类
            var components = new EntityRegistrationScanner()
                .With(new ComponentRegistrationHandler<EntityKey>())
                .RegisterEntitiesFromAllAssemblies();

            //打印所有组件的类型
            foreach (var component in components)
            {
                Debug.Log(component+ " ; ");
            }

            //目前只有TestStructFixture和Velocity结构体的属性类型为EntityAttribute
            components.Count.Should().Be(2);

            //最后将组件注册到registry中
            var registry = new EntityRegistry<EntityKey>(EntityKey.MaxAge, EntityKey.Create);
            EntityComponentActivator.Create<EntityKey>()
                .With(new ComponentRegistrationActivator<EntityKey>())
                .ActivateAll(registry,components);
        }
    }
}