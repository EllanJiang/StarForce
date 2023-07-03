/*
* 文件名：EntityFlagPoolTest
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/03 10:32:59
* 修改记录：
*/

using System;
using System.Collections.Generic;
using Entt.Entities;
using FluentAssertions;
using UnityEngine;

namespace Entt.Test
{
    /// <summary>
    /// 测试标志Pool
    /// </summary>
    public class EntityFlagPoolTest:MonoBehaviour
    {
        private EntityRegistry<EntityKey>registry;
        private EntityKey[] keys;
        private IPersistentEntityView<EntityKey, SomeMarker> view;

        private void Start()
        {
            registry = new EntityRegistry<EntityKey>(EntityKey.MaxAge, (age, key) => new EntityKey(age, key));
            registry.RegisterNonConstructable<int>();
            registry.RegisterFlag<SomeMarker>(); //注册标志

            var k1 = registry.CreateAsActor().AssignComponent(1).Entity;
            var k2 = registry.CreateAsActor().AssignComponent(2).AssignComponent<SomeMarker>().Entity; //EntityKey为（key=2,age=1）的Entity身上挂载了两个组件：组件2和组件SomeMarker
            var k3 = registry.CreateAsActor().AssignComponent(3).AssignComponent<SomeMarker>().Entity;
            var k4 = registry.CreateAsActor().AssignComponent<SomeMarker>().Entity;
            var k5 = registry.CreateAsActor().Entity;

            keys = new[] { k1, k2, k3, k4, k5 };
            view = registry.PersistentView<SomeMarker>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                TestClear();
            }
        }

        public void TestClear()
        {
            LogUtil.Log("TestClear 开始");
            view.Count.Should().Be(3);
            registry.Count.Should().Be(5);
            var pool = registry.GetPool<SomeMarker>();

            pool.Contains(keys[0]).Should().BeFalse();
            pool.Contains(keys[1]).Should().BeTrue();
            pool.Contains(keys[2]).Should().BeTrue();
            pool.Contains(keys[3]).Should().BeTrue();
            pool.Contains(keys[4]).Should().BeFalse();
            
            registry.Clear();

            view.Count.Should().Be(0);
            registry.Count.Should().Be(0);
            
            pool.Contains(keys[0]).Should().BeFalse();
            pool.Contains(keys[1]).Should().BeFalse();
            pool.Contains(keys[2]).Should().BeFalse();
            pool.Contains(keys[3]).Should().BeFalse();
            pool.Contains(keys[4]).Should().BeFalse();
            
            LogUtil.Log("TestClear 结束");
        }
        
        public void TestRemoveComponentFirst()
        {
            registry.RemoveComponent<SomeMarker>(keys[1]);

            view.Should().BeEquivalentTo(new List<EntityKey>(){ keys[2], keys[3]});
            registry.Should().BeEquivalentTo(keys);
        }
        
        public void TestRemoveEntityFirst()
        {
            registry.Destroy(keys[1]);

            view.Should().BeEquivalentTo(new List<EntityKey>(){keys[2], keys[3]});
            registry.Should().BeEquivalentTo(new List<EntityKey>(){keys[0], keys[2], keys[3], keys[4]});
            
            var pool = registry.GetPool<SomeMarker>();
            pool.Contains(keys[1]).Should().BeFalse();
        }
        
        public void TestRemoveComponentLast()
        {
            registry.RemoveComponent<SomeMarker>(keys[3]);

            view.Should().BeEquivalentTo(new List<EntityKey>(){keys[1], keys[2]});
            registry.Should().BeEquivalentTo(keys);
        }
        public void TestRemoveEntityLast()
        {
            registry.Destroy(keys[3]);

            view.Should().BeEquivalentTo(new List<EntityKey>(){keys[1], keys[2]});
            registry.Should().BeEquivalentTo(new List<EntityKey>(){keys[0], keys[1], keys[2], keys[4]});

            var pool = registry.GetPool<SomeMarker>();
            pool.Contains(keys[3]).Should().BeFalse();
        }

        
        
        readonly struct SomeMarker
        {
        }
    }
}