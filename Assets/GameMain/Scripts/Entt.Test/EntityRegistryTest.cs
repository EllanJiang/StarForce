/*
* 文件名：EntityRegistryTest
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/30 19:33:00
* 修改记录：
*/

using System;
using System.Collections.Generic;
using Entt.Entities;
using UnityEngine;

namespace Entt.Test
{
    public class EntityRegistryTest:MonoBehaviour
    {
        private EntityRegistry<EntityKey> Registry;
        private IReadOnlyList<EntityKey> Keys;
        private IPersistentEntityView<EntityKey, int> PersistentEntityView;  //持续持有Entity列表，跟Adhoc相对
        private IPersistentEntityView<EntityKey, string> PersistentEntityView2;  //持续持有Entity列表，跟Adhoc相对
        private void Start()
        {
            Registry = new EntityRegistry<EntityKey>(EntityKey.MaxAge, (age, key) => new EntityKey(age, key));
            Registry.RegisterNonConstructable<int>();
            Registry.RegisterNonConstructable<string>();

            var k1 = Registry.CreateAsActor().AssignComponent(1).Entity;  //该Entity身上挂载有1组件
            var k2 = Registry.CreateAsActor().AssignComponent(2).Entity;  //该Entity身上挂载有2组件
            var k3 = Registry.CreateAsActor().AssignComponent(3).Entity;  //该Entity身上挂载有3组件
            var k4 = Registry.CreateAsActor().Entity;//该Entity没有挂载组件

            var k5 = Registry.CreateAsActor().AssignComponent("Test").Entity;

            Keys = new[] { k1, k2, k3, k4 };
            PersistentEntityView = Registry.PersistentView<int>();
            PersistentEntityView2 = Registry.PersistentView<string>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                TestClear();
            }
        }

        /// <summary>
        /// 清除获取的所有Entity列表
        /// </summary>
        public void TestClear()
        {
            LogUtil.Log($"清理之前，PersistentEntityView的数量应该是3，实际上是：" + PersistentEntityView.Count);
            LogUtil.Log($"清理之前，Registry的数量应该是4，实际上是：" + Registry.Count);
            
            Registry.Clear();
            
            LogUtil.Log($"清理之后，PersistentEntityView的数量应该是0，实际上是：" + PersistentEntityView.Count);
            LogUtil.Log($"清理之后，Registry的数量应该是0，实际上是：" + Registry.Count);

            var pool = Registry.GetPool<int>();
            LogUtil.Log(pool.Contains(Keys[0])); 
            LogUtil.Log(pool.Contains(Keys[1])); 
            LogUtil.Log(pool.Contains(Keys[2])); 
            LogUtil.Log(pool.Contains(Keys[3])); 
        }
        
        
    }
    
    
    
    
    public static class ComponentPoolHelpers
    {
        public static List<(EntityKey, T)> CollectContents<T>(this IEntityView<EntityKey, T> view)
        {
            void CollectData(IEntityViewControl<EntityKey> v, List<(EntityKey, T)> context, EntityKey k, in T data)
            {
                context.Add((k, data));
            }

            var collector = new List<(EntityKey, T)>();
            view.ApplyWithContext(collector, CollectData);
            return collector;
        }

        public static List<(EntityKey, T1, T2)> CollectContents<T1, T2>(this IEntityView<EntityKey, T1, T2> view)
        {
            void CollectData(IEntityViewControl<EntityKey> v, List<(EntityKey, T1, T2)> context, EntityKey k, in T1 data, in T2 data2)
            {
                context.Add((k, data, data2));
            }

            var collector = new List<(EntityKey, T1, T2)>();
            view.ApplyWithContext(collector, CollectData);
            return collector;
        }
    }
}