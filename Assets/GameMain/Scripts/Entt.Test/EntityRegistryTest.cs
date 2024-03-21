﻿/*
* 文件名：EntityRegistryTest
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/30 19:33:00
* 修改记录：
*/

using System;
using System.Collections.Generic;
using Entt.Entities;
using FluentAssertions;
using UnityEngine;

namespace Entt.Test
{
    public class EntityRegistryTest:MonoBehaviour
    {
        private EntityRegistry<EntityKey>Registry;
        private IReadOnlyList<EntityKey> Keys;
        private IPersistentEntityView<EntityKey, int> PersistentEntityView;  //持续持有Entity列表，跟Adhoc相对
        //private IPersistentEntityView<EntityKey, string> PersistentEntityView2;  //持续持有Entity列表，跟Adhoc相对
        private void Start()
        {
            Registry = new EntityRegistry<EntityKey>(EntityKey.MaxAge, (age, key) => new EntityKey(age, key));
            Registry.RegisterNonConstructable<int>();
            Registry.RegisterNonConstructable<string>();

            var k1 = Registry.CreateAsActor().AssignComponent(1).Entity;  //该Entity身上挂载有1组件
            var k2 = Registry.CreateAsActor().AssignComponent(2).Entity;  //该Entity身上挂载有2组件
            var k3 = Registry.CreateAsActor().AssignComponent(3).Entity;  //该Entity身上挂载有3组件
            var k4 = Registry.CreateAsActor().Entity;//该Entity没有挂载组件

            //var k5 = Registry.CreateAsActor().AssignComponent("Test").Entity;

            Keys = new[] { k1, k2, k3, k4 };
            PersistentEntityView = Registry.PersistentView<int>();
            //PersistentEntityView2 = Registry.PersistentView<string>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                TestClear();
                //TestRemoveComponentFirst();
                //TestRemoveEntityFirst();
                //TestRemoveComponentLast();
                //TestRemoveEntityLast();
            }
        }

        /// <summary>
        /// 清除获取的所有Entity列表
        /// </summary>
        public void TestClear()
        {
            LogicShared.Logger.Debug($"清理之前，PersistentEntityView的数量应该是3，实际上是：" + PersistentEntityView.Count);
            LogicShared.Logger.Debug($"清理之前，Registry的数量应该是4，实际上是：" + Registry.Count);
            
            Registry.Clear();
            
            LogicShared.Logger.Debug($"清理之后，PersistentEntityView的数量应该是0，实际上是：" + PersistentEntityView.Count);
            LogicShared.Logger.Debug($"清理之后，Registry的数量应该是0，实际上是：" + Registry.Count);

            var pool = Registry.GetPool<int>();
            LogicShared.Logger.Debug(pool.Contains(Keys[0]).ToString()); 
            LogicShared.Logger.Debug(pool.Contains(Keys[1]).ToString()); 
            LogicShared.Logger.Debug(pool.Contains(Keys[2]).ToString()); 
            LogicShared.Logger.Debug(pool.Contains(Keys[3]).ToString()); 
        }

        /// <summary>
        /// 移除第一个组件
        /// </summary>
        public void TestRemoveComponentFirst()
        {
            LogicShared.Logger.Debug("TestRemoveComponentFirst 开始");
            //移除第一个Entity身上挂载的int组件，此时PersistentEntityView的数量应该是2
            Registry.RemoveComponent<int>(Keys[0]);
            PersistentEntityView.Count.Should().Be(2);
            //且剩下的EntityKey应该等于Keys[1]和Keys[2]
            PersistentEntityView.Should().BeEquivalentTo(new List<EntityKey>(){Keys[1], Keys[2]});
            //但是Registry拥有的entities不会变化
            Registry.Should().BeEquivalentTo(Keys);
            //EntityKey和value的键值对应该是(Keys[1], 2)和(Keys[2], 3)
            PersistentEntityView.CollectContents().Should().BeEquivalentTo(new List<(EntityKey, int)>()
            {
                (Keys[1], 2),
                (Keys[2], 4) //这里会报错：AssertionException: Expected field PersistentEntityView.CollectContents()[1].Item2 to be 4, but found 3.
            });
            
            LogicShared.Logger.Debug("TestRemoveComponentFirst 成功");
        }

        public void TestRemoveEntityFirst()
        {
            LogicShared.Logger.Debug("TestRemoveEntityFirst 开始");
            
            //销毁entities列表中第一个entity及其身上挂载的所有component
            Registry.Destroy(Keys[0]);
            
            PersistentEntityView.Count.Should().Be(2);
            //且剩下的EntityKey应该等于Keys[1]和Keys[2]
            PersistentEntityView.Should().BeEquivalentTo(new List<EntityKey>()
            {
                Keys[1], Keys[2]
            });
            //Registry拥有的entities少了第一个
            Registry.Should().BeEquivalentTo(new List<EntityKey>()
            {
                Keys[1], Keys[2],Keys[3]
            });
            //EntityKey和value的键值对应该是(Keys[1], 2)和(Keys[2], 3)
            PersistentEntityView.CollectContents().Should().BeEquivalentTo(new List<(EntityKey, int)>()
            {
                (Keys[1], 2),
                (Keys[2], 3) 
            });

            var pool = Registry.GetPool<int>();
            pool.Contains(Keys[0]).Should().BeTrue(); //这里会报错：AssertionException: Expected pool.Contains(Keys[0]) to be true, but found False.
            
            LogicShared.Logger.Debug("TestRemoveEntityFirst 成功");
        }
        
        //移除最后一个Entity身上挂载的Component组件
        public void TestRemoveComponentLast()
        {
            LogicShared.Logger.Debug("TestRemoveComponentLast开始");
            Registry.RemoveComponent<int>(Keys[2]);
            PersistentEntityView.Count.Should().Be(2);

            //且剩下的EntityKey应该等于Keys[0]和Keys[1]
            PersistentEntityView.Should().BeEquivalentTo(new List<EntityKey>(){Keys[0], Keys[1]});
            //但是Registry拥有的entities不会变化
            Registry.Should().BeEquivalentTo(Keys);
            //EntityKey和value的键值对应该是(Keys[0], 1)和(Keys[1], 2)
            PersistentEntityView.CollectContents().Should().BeEquivalentTo(new List<(EntityKey, int)>()
            {
                (Keys[0], 1),
                (Keys[1], 2) 
            });
            LogicShared.Logger.Debug("TestRemoveComponentLast成功");
        }

        //移除entities列表中最后一个entity及其身上挂载的component
        public void TestRemoveEntityLast()
        {
            LogicShared.Logger.Debug("TestRemoveEntityLast开始");
            Registry.Destroy(Keys[2]);
            
            PersistentEntityView.Count.Should().Be(2);
            //且剩下的EntityKey应该等于Keys[0]和Keys[1]
            PersistentEntityView.Should().BeEquivalentTo(new List<EntityKey>()
            {
                Keys[0], Keys[1]
            });
            //Registry拥有的entities少了最后一个
            Registry.Should().BeEquivalentTo(new List<EntityKey>()
            {
                Keys[0], Keys[1],Keys[3]  //Keys[3]对应的Entity没有Component
            });
            //EntityKey和value的键值对应该是(Keys[0], 1)和(Keys[1], 2)
            PersistentEntityView.CollectContents().Should().BeEquivalentTo(new List<(EntityKey, int)>()
            {
                (Keys[0], 1),
                (Keys[1], 2) 
            });

            var pool = Registry.GetPool<int>();
            pool.Contains(Keys[2]).Should().BeFalse(); 
            LogicShared.Logger.Debug("TestRemoveEntityLast成功");
        }
    }
    
    
    
    
    public static class ComponentPoolHelpers
    {
        public static List<(EntityKey, T)> CollectContents<T>(this IEntityView<EntityKey, T> view)
        {
            //TContext = List<(EntityKey, T)>
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