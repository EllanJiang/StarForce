/*
* 文件名：SparseDictTest
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/03 20:13:42
* 修改记录：
*/

using System;
using Entt.Entities;
using Entt.Test.Fixtures;
using FluentAssertions;
using UnityEngine;

namespace Entt.Test.Helpers
{
    public class SparseDictTest:MonoBehaviour
    {
        private void Start()
        {
            TestViewProcessing();
        }

        public void TestViewProcessing()
        {
            var reg = new EntityRegistry<EntityKey>(EntityKey.MaxAge, EntityKey.Create);
            reg.Register<TestStructFixture>();

            var entity = reg.Create();
            reg.AssignComponent(entity, new TestStructFixture());
            reg.View<TestStructFixture>().Apply(Change);
            reg.GetComponent<TestStructFixture>(entity, out var c);
            c.x.Should().Be(10);
        }
        
        void Change(IEntityViewControl<EntityKey> view, EntityKey e, in TestStructFixture t)
        {
            view.WriteBack(e, new TestStructFixture(10, t.y));
        }
    }
}