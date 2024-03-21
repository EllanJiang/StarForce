/*
* 文件名：EntityActor
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/28 15:00:31
* 修改记录：
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Entt.Entities
{
    /// <summary>
    /// 用于在场景中表示一个Entity实体
    /// </summary>
    public static class EntityActor
    {
        /// <summary>
        /// 创建一个新的EntityActor
        /// </summary>
        /// <param name="registry"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public static EntityActor<TEntityKey> Create<TEntityKey>(EntityRegistry<TEntityKey> registry) where TEntityKey : IEntityKey
        {
            var entity = registry.Create();
            return new EntityActor<TEntityKey>(registry, entity);
        }
        
        /// <summary>
        /// 创建一个EntityActor
        /// </summary>
        /// <param name="registry"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public static EntityActor<TEntityKey> CreateAsActor<TEntityKey>(this EntityRegistry<TEntityKey> registry) 
            where TEntityKey : IEntityKey
        {
            return EntityActor.Create(registry);
        }

        /// <summary>
        /// 把一个Entity转成EntityActor
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="entityKey"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public static EntityActor<TEntityKey> AsActor<TEntityKey>(this EntityRegistry<TEntityKey> registry, TEntityKey entityKey) 
            where TEntityKey : IEntityKey
        {
            return new EntityActor<TEntityKey>(registry, entityKey);
        }
    }

    /// <summary>
    /// 代表一个Entity
    /// 这是一个结构体
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public readonly struct EntityActor<TEntityKey> where TEntityKey : IEntityKey
    {
        static readonly EqualityComparer<TEntityKey> EqualityHandler = EqualityComparer<TEntityKey>.Default;
        readonly TEntityKey entity;
        readonly IEntityViewControl<TEntityKey> registry;
        
        internal EntityActor(IEntityViewControl<TEntityKey> registry, TEntityKey entity)
        {
            this.entity = entity;
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public bool IsValid()
        {
            return registry.Contains(entity);
        }
        
        public bool HasTag<TTag>()
        {
            if (registry.TryGetTag<TTag>(out var k, out _))
            {
                return EqualityHandler.Equals(k, entity);
            }

            return false;
        }
        
        public EntityActor<TEntityKey> AttachTag<TTag>()
        {
            registry.AttachTag<TTag>(entity);
            return this;
        }
        
        public EntityActor<TEntityKey> AttachTag<TTag>(TTag tag)
        {
            registry.AttachTag(entity, tag);
            return this;
        }

        public EntityActor<TEntityKey> RemoveTag<TTag>()
        {
            registry.RemoveTag<TTag>();
            return this;
        }

        public bool TryGetTag<TTag>(out Optional<TTag> tag)
        {
            if (registry.TryGetTag(out var other, out tag))
            {
                if (EqualityHandler.Equals(other, entity))
                {
                    return true;
                }
            }

            tag = default;
            return false;
        }
        
        /// <summary>
        /// EntityActor身上的Entity
        /// </summary>
        public TEntityKey Entity
        {
            get { return entity; }
        }
        
        public EntityActor<TEntityKey> AssignComponent<TComponent>()
        {
            registry.AssignComponent<TComponent>(entity);
            return this;
        }

        public EntityActor<TEntityKey> AssignComponent<TComponent>(TComponent c)
        {
            registry.AssignComponent(entity, c);
            return this;
        }

        public EntityActor<TEntityKey> AssignComponent<TComponent>(in TComponent c)
        {
            registry.AssignComponent(entity, in c);
            return this;
        }

        public EntityActor<TEntityKey> RemoveComponent<TComponent>()
        {
            registry.RemoveComponent<TComponent>(entity);
            return this;
        }
        
        public bool HasComponent<TComponent>()
        {
            return registry.HasComponent<TComponent>(entity);
        }

        public bool GetComponent<TComponent>([MaybeNullWhen(false)] out TComponent c)
        {
            return registry.GetComponent(entity, out c);
        }

        public EntityActor<TEntityKey> AssignOrReplace<TComponent>(in TComponent c)
        {
            registry.AssignOrReplace(entity, in c);
            return this;
        }

        public bool ReplaceComponent<TComponent>(in TComponent c)
        {
            return registry.ReplaceComponent(entity, in c);
        }

        public bool IsOrphan()
        {
            return registry.IsOrphan(entity);
        }

        public void WriteBack<TComponent>(in TComponent c) where TComponent : struct
        {
            registry.WriteBack(entity, in c);
        }

        /// <summary>
        /// 把EntityActor隐式转换成Entity
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static implicit operator TEntityKey(EntityActor<TEntityKey> self) => self.Entity;
    }
}