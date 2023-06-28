/*
* 文件名：DocumentationExample
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/28 19:42:45
* 修改记录：
*/

using System;
using Entt.Entities;
using UnityEngine;

namespace Entt.Test
{
    public class DocumentationExample:MonoBehaviour
    {
        private EntityRegistry<EntityKey> m_Registry;
        private float m_MinSpeed = -0.3f;
        private float m_MaxSpeed = 0.4f;
        private void Start()
        {
            
            //创建Entity管理器
            var registry = new EntityRegistry<EntityKey>(EntityKey.MaxAge, CreateEntityKey);

            //注册组件
            registry.Register<Velocity>();
            registry.Register<Position>();

            //创建Entity并挂载Component
            for (int i = 0; i < 10; i++)
            {
                //创建一个新的Entity
                var entity = registry.Create();
                //第一种添加组件方法：把Position组件挂载到新创建的Entity身上
                registry.AssignComponent<Position>(entity);
                if (i % 2 == 0)
                {
                    //第二种添加组件方法：把Velocity组件挂载到新创建的Entity身上
                    registry.AssignComponent(entity,new Velocity(UnityEngine.Random.Range(m_MinSpeed, m_MaxSpeed),UnityEngine.Random.Range(m_MinSpeed, m_MaxSpeed)));
                }
            }

            m_Registry = registry;
        }

        private EntityKey CreateEntityKey(byte age, int key)
        {
            return new EntityKey(age, key);
        }
        
        private void Update()
        {
            var deltaTime = Time.deltaTime;
            UpdatePosition(deltaTime);
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                ClearVelocity();
            }
        }

        //更新玩家位置
        private void UpdatePosition(float deltaTime)
        {
            //view contains all the entities that have both a Position and a Velocity component ...
            var view = m_Registry.View<Velocity, Position>();
            foreach (var entity in view)
            {
                if (view.GetComponent<Position>(entity, out Position position) &&
                    view.GetComponent<Velocity>(entity, out Velocity velocity))
                {
                    var newPos = new Position(position.X + velocity.DeltaX * deltaTime,
                        position.Y + velocity.DeltaY * deltaTime);
                    
                    m_Registry.ReplaceComponent(entity,in newPos);
                    transform.localPosition = new Vector3(newPos.X, newPos.Y, 0);
                }
            }
        }

        //重置速度
        private void ClearVelocity()
        {
            var view = m_Registry.View<Velocity>();
            foreach (var entity in view)
            {
                m_Registry.AssignComponent<Velocity>(entity);
            }
        }

       
    }
}