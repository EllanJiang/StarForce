/*
* 文件名：EntityRegistry
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/21 20:09:47
* 修改记录：
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Entt.Entities.Helpers;
using Entt.Entities.Pools;
using Serilog;

namespace Entt.Entities
{
    /// <summary>
    /// Entity管理器，存储并管理所有Entity和Component
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public partial class EntityRegistry<TEntityKey> : IEntityViewFactory<TEntityKey>,
                                                      IEntityPoolAccess<TEntityKey>,
                                                      IEntityComponentRegistry<TEntityKey>
        where TEntityKey : IEntityKey
    {
        readonly ILogger logger = LogHelper.ForContext<EntityRegistry<TEntityKey>>();
        readonly EqualityComparer<TEntityKey> equalityComparer;
        /// <summary>
        /// 所有Entity
        /// </summary>
        readonly List<TEntityKey> entities;
        /// <summary>
        /// 所有对象池
        /// </summary>
        readonly List<PoolEntry> pools;
        /// <summary>
        /// 所有组件id
        /// </summary>
        readonly Dictionary<Type, IComponentRegistration> componentIndex;
        /// <summary>
        /// 所有标签id
        /// </summary>
        readonly Dictionary<Type, int> tagIndex;
        /// <summary>
        /// 所有附加的标签列表
        /// </summary>
        readonly List<Attachment> tags;
        /// <summary>
        /// 保存所有entity view
        /// Key是组件类型,value是拥有该组件的所有Entity列表
        /// </summary>
        readonly Dictionary<Type, IEntityView<TEntityKey>> views;
        /// <summary>
        /// entityKey工厂，用来创建EntityKey
        /// </summary>
        readonly Func<byte, int, TEntityKey> entityKeyFactory;
        /// <summary>
        /// 下一个可以使用的EntityKey
        /// </summary>
        int next;
        /// <summary>
        ///  在entities列表中，在[0,available-1]区间内的索引是无效索引，在[available,entities.Count-1]区间内的索引是有效索引
        /// </summary>
        int available;
        
        public int MaxAge { get; }
        
        /// <summary>
        /// Entity销毁之前会触发该事件
        /// </summary>
        public event EventHandler<TEntityKey>? BeforeEntityDestroyed;
        
        public EntityRegistry(int maxAge, Func<byte, int, TEntityKey> entityKeyFactory)
        {
            if (maxAge < 2)
            {
                throw new ArgumentException();
            }

            MaxAge = maxAge;
            this.entityKeyFactory = entityKeyFactory ?? throw new ArgumentNullException(nameof(entityKeyFactory));
            equalityComparer = EqualityComparer<TEntityKey>.Default;
            componentIndex = new Dictionary<Type, IComponentRegistration>();
            entities = new List<TEntityKey>();
            pools = new List<PoolEntry>();
            tagIndex = new Dictionary<Type, int>();
            tags = new List<Attachment>();
            views = new Dictionary<Type, IEntityView<TEntityKey>>();
        }
        
        /// <summary>
        /// entities列表中有效entity的数量
        /// </summary>
        public int Count
        {
            get { return entities.Count - available; }
        }
        
        /// <summary>
        /// entities列表是否为空
        /// </summary>
        public bool IsEmpty
        {
            get { return Count == 0; }
        }
        
        /// <summary>
        /// 该组件是否正在被管理，也就是是否正在被使用
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public bool IsManaged<TComponent>()
        {
            return componentIndex.ContainsKey(typeof(TComponent));
        }

        /// <summary>
        /// 根据组件类型，获取该组件在组件列表中的索引
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        int ManagedIndex<TComponent>()
        {
            if (componentIndex.TryGetValue(typeof(TComponent), out var reg))
            {
                return reg.Index;
            }

            return -1;
        }

        #region 组件注册相关接口

 
        /// <summary>
        /// 获取组件注册接口
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        IComponentRegistration<TEntityKey, TComponent> GetRegistration<TComponent>()
        {
            if (componentIndex.TryGetValue(typeof(TComponent), out var reg))
            {
                return (IComponentRegistration<TEntityKey, TComponent>)reg;
            }

            ThrowInvalidRegistrationError(typeof(TComponent));
            return default;
        }
        
        [DoesNotReturn]
        void ThrowInvalidRegistrationError(Type t)
        {
            throw new ArgumentException($"Unknown registration at EntityRegistry<{typeof(TEntityKey)}> for component type {t}");
        }
        
        /// <summary>
        /// 获取该组件在world中总数
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        internal int CountComponents<TComponent>()
        {
            var idx = ManagedIndex<TComponent>();
            if (idx == -1)
            {
                ThrowInvalidRegistrationError(typeof(TComponent));
                return -1;
            }

            return pools[idx].ReadonlyPool.Count;
        }
        
        /// <summary>
        /// 获取该组件对应的只读对象池
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public IReadOnlyPool<TEntityKey, TComponent> GetPool<TComponent>()
        {
            if (!TryGetPool<TComponent>(out var pool))
            {
                ThrowInvalidRegistrationError(typeof(TComponent));
                return default;
            }

            return pool;
        }
        
        /// <summary>
        /// 获取该组件对应的只读对象池
        /// </summary>
        /// <param name="pool"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public bool TryGetPool<TComponent>([MaybeNullWhen(false)] out IReadOnlyPool<TEntityKey, TComponent> pool)
        {
            var idx = ManagedIndex<TComponent>();
            if (idx == -1)
            {
                pool = default;
                return false;
            }

            if (pools[idx].TryGetPool(out var maybePool) && maybePool is IReadOnlyPool<TEntityKey, TComponent> readOnlyPool)
            {
                pool = readOnlyPool;
                return true;
            }
            
            pool = default;
            return false;
        }
        
        /// <summary>
        /// 获取该组件对应的可读可写对象池
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public IPool<TEntityKey, TComponent> GetWritablePool<TComponent>()
        {
            if (!TryGetWritablePool<TComponent>(out var p))
            {
                ThrowInvalidRegistrationError(typeof(TComponent));
                return default;
            }

            return p;
        }
        
        /// <summary>
        /// 获取该组件对应的可读可写对象池
        /// </summary>
        /// <param name="pool"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public bool TryGetWritablePool<TComponent>([MaybeNullWhen(false)] out IPool<TEntityKey, TComponent> pool)
        {
            var idx = ManagedIndex<TComponent>();
            if (idx == -1)
            {
                pool = default;
                return false;
            }

            if (pools[idx].TryGetPool(out var maybePool) && maybePool is IPool<TEntityKey, TComponent> writablePool)
            {
                pool = writablePool;
                return true;
            }

            pool = default;
            return false;
        }
        
        /// <summary>
        /// 注册组件到Entity身上（这时并没有真正创建组件）,并返回组件对象池
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IPool<TEntityKey, TComponent> Register<TComponent>() where TComponent : new()
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException($"重复注册该组件:{typeof(TComponent)}");
            }

            var registration = ComponentRegistration.Create(componentIndex.Count, this, () => new TComponent());
            var pool = PoolFactory.Create(registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, new PoolEntry(pool));
            return pool;
        }
        
        /// <summary>
        /// 注册组件标志，并返回组件标志对象池
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IPool<TEntityKey, TComponent> RegisterFlag<TComponent>() where TComponent : new()
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException($"重复注册该组件标志:{typeof(TComponent)}");
            }

            var registration = ComponentRegistration.Create(componentIndex.Count, this, () => new TComponent());
            var pool = PoolFactory.CreateFlagPool(new TComponent(), registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, new PoolEntry(pool));
            return pool;
        }
        
        /// <summary>
        /// 注册组件标志，并返回组件标志对象池
        /// </summary>
        /// <param name="sharedData"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IPool<TEntityKey, TComponent> RegisterFlag<TComponent>(TComponent sharedData)
        {
            if (IsManaged<TComponent>())
            {
                 throw new ArgumentException($"重复注册该组件标志:{typeof(TComponent)}");
            }

            var registration = ComponentRegistration.Create<TEntityKey, TComponent>(componentIndex.Count, this);
            var pool = PoolFactory.CreateFlagPool(sharedData, registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, new PoolEntry(pool));
            return pool;
        }

        public IReadOnlyPool<TEntityKey, Not<TComponent>> RegisterNonExistingFlag<TComponent>()
        {
            if (IsManaged<Not<TComponent>>())
            {
                throw new ArgumentException($"重复注册该组件标志:{typeof(TComponent)}");
            }

            if (!IsManaged<TComponent>())
            {
                throw new ArgumentException($"Require component registration of base type {typeof(TComponent)}");
            }

            var registration = ComponentRegistration.Create<TEntityKey, Not<TComponent>>(componentIndex.Count, this);
            var basePool = GetPool<TComponent>();
            var pool = new NotPool<TEntityKey, TComponent>(this, basePool);
            componentIndex[typeof(Not<TComponent>)] = registration;
            pools.StoreAt(registration.Index, new PoolEntry(pool));
            return pool;
        }
        
        /// <summary>
        /// 注册Entity组件标注
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        void IEntityComponentRegistry<TEntityKey>.RegisterFlag<TComponent>()
        {
            RegisterFlag(default(TComponent));
        }
        
        /// <summary>
        /// 注册Entity组件，需要提供组件构造函数和析构函数
        /// </summary>
        /// <param name="constructorFn"></param>
        /// <param name="destructorFn"></param>
        /// <typeparam name="TComponent"></typeparam>
        void IEntityComponentRegistry<TEntityKey>.Register<TComponent>(Func<TComponent> constructorFn,
            Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent>? destructorFn)
        {
            Register(constructorFn, destructorFn);
        }
        
        public IPool<TEntityKey, TComponent> Register<TComponent>(Func<TComponent> constructor,
                Action<TEntityKey, EntityRegistry<TEntityKey>, TComponent>? destructor = null)
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException($"重复注册该组件:{typeof(TComponent)}");
            }

            var registration = ComponentRegistration.Create(componentIndex.Count, this, constructor, destructor);
            var pool = PoolFactory.Create(registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, new PoolEntry(pool));
            return pool;
        }
        
        void IEntityComponentRegistry<TEntityKey>.RegisterNonConstructable<TComponent>(Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent>? destructorFn)
        {
            RegisterNonConstructable(destructorFn);
        }

        /// <summary>
        /// 注册Entity组件，不需要提供组件的构造函数
        /// </summary>
        /// <param name="destructor"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IPool<TEntityKey, TComponent> RegisterNonConstructable<TComponent>(
            Action<TEntityKey, EntityRegistry<TEntityKey>, TComponent>? destructor = null)
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException($"重复注册该组件:{typeof(TComponent)}");
            }

            var registration = ComponentRegistration.Create(componentIndex.Count, this, destructor);
            var pool = PoolFactory.Create(registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, new PoolEntry(pool));
            return pool;
        }
        
               

        #endregion

        /// <summary>
        /// 重置该组件对象池容量
        /// </summary>
        /// <param name="capacity"></param>
        /// <typeparam name="TComponent"></typeparam>
        public void Reserve<TComponent>(int capacity)
        {
            GetPool<TComponent>().Reserve(capacity);
        }
        
        public bool IsPoolEmpty<TComponent>()
        {
            return GetPool<TComponent>().Count == 0;
        }
        
        /// <summary>
        /// 判断该Entity是否正在使用
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsValid(TEntityKey key)
        {
            if (key.Key < 0 || key.Key >= entities.Count)
            {
                return false;
            }

            return equalityComparer.Equals(entities[key.Key], key);
        }
        
        /// <summary>
        /// 获取该Entity被复用的次数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal int StoredVersion(EntityKey key)
        {
            return entities[key.Key].Age;
        }
        
        /// <summary>
        /// 创建一个Entity
        /// </summary>
        /// <returns></returns>
        public TEntityKey Create()
        {
            if (available > 0)
            {
                // for empty slots, entities contains the pointer to the next empty element
                // this is filled in during destroy ...
                var entityKey = next;
                var nextEmpty = entities[next];
                var entity = entityKeyFactory(nextEmpty.Age, entityKey);
                entities[next] = entity;
                next = nextEmpty.Key;
                available -= 1;
                return entity;
            }
            else
            {
                var entity = entityKeyFactory(1, entities.Count);
                entities.Add(entity);
                return entity;
            }
        }
        
        /// <summary>
        /// 销毁一个Entity和它身上挂载的所有Component
        /// </summary>
        /// <param name="entity"></param>
        public void Destroy(TEntityKey entity)
        {
            AssertValid(entity);

            BeforeEntityDestroyed?.Invoke(this, entity);
            logger.Verbose("Destroying {Entity}", entity);

            var entityKey = entity.Key;
            var node = entityKeyFactory(RollingAgeIncrement(entity.Age), available > 0 ? next : entityKey + 1);

            foreach (var pool in pools)
            {
                if (pool.TryGetPool(out var p))
                {
                    logger.Verbose("- Removing component from pool [{Pool}] entry {Entity}", p, entity);
                    p.Remove(entity);
                }
            }
            
            entities[entityKey] = node;
            next = entityKey;
            available += 1;

        }
        
        void AssertValid(TEntityKey entity)
        {
            if (!IsValid(entity))
            {
                throw new ArgumentException($"Key {entity} is not valid in this registry.");
            }
        }
        
        /// <summary>
        /// 循环增加Entity的Age
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        byte RollingAgeIncrement(byte value)
        {
            value += 1;
            if (value == MaxAge)
            {
                return 1;
            }

            return value;
        }

        #region 标签相关
        /// <summary>
        /// 标签列表中是否有该标签
        /// </summary>
        /// <typeparam name="TTag"></typeparam>
        /// <returns></returns>
        public bool HasTag<TTag>()
        {
            if (!tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                return false;
            }

            return tags[idx].Tag != null;
        }

        /// <summary>
        /// 附加一个标签到entity上
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TTag"></typeparam>
        public void AttachTag<TTag>(TEntityKey entity)
        {
            var tag = GetRegistration<TTag>().Create();
            AttachTag(entity, tag);
        }

        public void AttachTag<TTag>(TEntityKey entity, in TTag? tag)
        {
            AssertValid(entity);

            if (HasTag<TTag>())
            {
                throw new ArgumentException($"重复附加该标签:{typeof(TTag)}");
            }

            if (!tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                idx = tagIndex.Count;
                tagIndex[typeof(TTag)] = idx;
                tags.Add(new Attachment(entity, tag));
            }
            else
            {
                tags[idx] = new Attachment(entity, tag);
            }
        }
        
        /// <summary>
        /// 移除指定标签
        /// </summary>
        /// <typeparam name="TTag"></typeparam>
        public void RemoveTag<TTag>()
        {
            if (tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                var e = tags[idx];
                if (e.Tag != null)
                {
                    GetRegistration<TTag>().Destruct(e.Entity, (TTag)e.Tag);
                }

                tags[idx] = default;
            }
        }

        /// <summary>
        /// 获取指定类型的标签
        /// </summary>
        /// <typeparam name="TTag"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public TTag GetTag<TTag>()
        {
            if (tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                var att = tags[idx];
                var tagRaw = att.Tag;
                if (tagRaw == null)
                {
                    throw new ArgumentException();
                }

                return (TTag)tagRaw;
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// 尝试指定类型的标签
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tag"></param>
        /// <typeparam name="TTag"></typeparam>
        /// <returns></returns>
        public bool TryGetTag<TTag>([MaybeNullWhen(false)] out TEntityKey entity, [MaybeNullWhen(false)] out TTag tag)
        {
            if (tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                var att = tags[idx];
                if (att.Tag is TTag typedTag)
                {
                    tag = typedTag;
                    entity = att.Entity;
                    return true;
                }
            }

            entity = default;
            tag = default;
            return false;
        }

        /// <summary>
        /// 获取指定标签对应的Entity
        /// </summary>
        /// <typeparam name="TTag"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public TEntityKey GetTaggedEntity<TTag>()
        {
            if (tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                var att = tags[idx];
                var tagRaw = att.Tag;
                if (tagRaw == null)
                {
                    throw new ArgumentException($"No entity is tagged with type {typeof(TTag)}");
                }

                return att.Entity;
            }

            throw new ArgumentException($"No entity is tagged with type {typeof(TTag)}");
        }
        #endregion
        
        /// <summary>
        ///  Synchronizes the state of the registry with data received from the
        ///  snapshot loader. This will insert keys by using internal knowledge
        ///  of the data structured used.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="destroyed"></param>
        public void AssureEntityState(TEntityKey entity, bool destroyed)
        {
            var entt = entity.Key;
            while (entities.Count <= entt)
            {
                entities.Add(entityKeyFactory(0, entities.Count));
            }

            entities[entt] = entity;
            if (destroyed)
            {
                Destroy(entity);
                entities[entt] = entityKeyFactory(entity.Age, entities[entt].Key);
            }
        }

       

        #region 组件相关
        void AssertManaged<TComponent>()
        {
            if (!IsManaged<TComponent>())
            {
                throw new ArgumentException($"Unknown registration for type {typeof(TComponent)}");
            }
        }
        
        /// <summary>
        /// 添加默认组件到entity身上
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public TComponent AssignComponent<TComponent>(TEntityKey entity)
        {
            AssertManaged<TComponent>();

            var component = GetRegistration<TComponent>().Create();
            AssignComponent(entity, in component);
            return component;
        }

        /// <summary>
        /// 添加指定组件到entity身上
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="c"></param>
        /// <typeparam name="TComponent"></typeparam>
        public void AssignComponent<TComponent>(TEntityKey entity, in TComponent c)
        {
            AssertValid(entity);

            if (TryGetWritablePool<TComponent>(out var p))
            {
                p.Add(entity, in c);
            }
            else
            {
                AssertManaged<TComponent>();
            }
        }

        /// <summary>
        /// 移除entity身上指定的组件
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TComponent"></typeparam>
        public void RemoveComponent<TComponent>(TEntityKey entity)
        {
            AssertValid(entity);

            if (TryGetWritablePool<TComponent>(out var p))
            {
                p.Remove(entity);
            }
            else
            {
                AssertManaged<TComponent>();
            }
        }

        /// <summary>
        /// 判断该entity身上是否有指定组件
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public bool HasComponent<TComponent>(TEntityKey entity)
        {
            AssertManaged<TComponent>();
            AssertValid(entity);
            if (TryGetPool<TComponent>(out var pool))
            {
                return pool.Contains(entity);
            }

            AssertManaged<TComponent>();
            return false;
        }

        /// <summary>
        /// 获取该entity身上指定的组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="c"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public bool GetComponent<TComponent>(TEntityKey entity, [MaybeNullWhen(false)] out TComponent c)
        {
            AssertManaged<TComponent>();
            AssertValid(entity);
            if (TryGetPool<TComponent>(out var pool))
            {
                return pool.TryGet(entity, out c);
            }

            AssertManaged<TComponent>();
            c = default;
            return false;
        }

        /// <summary>
        /// 添加或替换该entity身上指定的组件
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public TComponent AssignOrReplace<TComponent>(TEntityKey entity)
        {
            var component = GetRegistration<TComponent>().Create();
            AssignOrReplace(entity, in component);
            return component;
        }

        /// <summary>
        /// 添加或替换该entity身上指定的组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="c"></param>
        /// <typeparam name="TComponent"></typeparam>
        public void AssignOrReplace<TComponent>(TEntityKey entity, in TComponent c)
        {
            AssertValid(entity);

            if (TryGetWritablePool<TComponent>(out var p))
            {
                if (!p.WriteBack(entity, in c))
                {
                    p.Add(entity, in c);
                }
            }
            else
            {
                AssertManaged<TComponent>();
            }
        }
        
        /// <summary>
        /// 该world中是否有该entity
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool Contains(TEntityKey e)
        {
            return IsValid(e);
        }
        
        
        /// <summary>
        /// 更新组件内容
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="data"></param>
        /// <typeparam name="TComponent"></typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBack<TComponent>(TEntityKey entity, in TComponent data)
        {
            AssertValid(entity);

            if (TryGetWritablePool<TComponent>(out var p))
            {
                p.WriteBack(entity, in data);
            }
            else
            {
                AssertManaged<TComponent>();
            }
        }
        
        /// <summary>
        /// 替换该entity身上指定的组件为默认组件
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public bool ReplaceComponent<TComponent>(TEntityKey entity)
        {
            return ReplaceComponent(entity, GetRegistration<TComponent>().Create());
        }

        /// <summary>
        /// 替换该entity身上指定的组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="c"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public bool ReplaceComponent<TComponent>(TEntityKey entity, in TComponent c)
        {
            AssertValid(entity);

            if (TryGetWritablePool<TComponent>(out var p))
            {
                return p.WriteBack(entity, in c);
            }
            else
            {
                AssertManaged<TComponent>();
            }

            return false;
        }

        /// <summary>
        /// 对组件进行排序（使用堆排序算法）
        /// </summary>
        /// <param name="comparator"></param>
        /// <typeparam name="TComponent"></typeparam>
        public void Sort<TComponent>(IComparer<TComponent> comparator)
        {
            if (TryGetPool<TComponent>(out var pool) && pool is ISortableCollection<TComponent> sortablePool)
            {
                sortablePool.HeapSort(comparator);
            }
        }

        /// <summary>
        /// 清空指定组件
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        public void ResetComponent<TComponent>()
        {
            if (TryGetWritablePool<TComponent>(out var pool))
            {
                pool.RemoveAll();
            }
        }
        
        public void CopyTo(List<TEntityKey> k)
        {
            k.Clear();
            k.Capacity = Math.Max(k.Capacity, Count);
            foreach (var e in this)
            {
                k.Add(e);
            }
        }

        /// <summary>
        /// 清除所有entity
        /// </summary>
        public void Clear()
        {
            var l = EntityKeyListPool<TEntityKey>.Reserve(this);
            foreach (var last in l)
            {
                if (IsValid(last))
                {
                    Destroy(last);
                }
                else
                {
                    logger.Verbose("Invalid entry {Entity}", last);
                }
            }

            EntityKeyListPool<TEntityKey>.Release(l);

            if (!IsEmpty)
            {
                // someone is doing something silly, like creating new entities
                // during create. Nuke the beast from orbit ..
                foreach (var pool in pools)
                {
                    if (pool.TryGetPool(out var p))
                    {
                        p.RemoveAll();
                    }
                }
            }

            entities.Clear();
            available = 0;
            next = default;
        }

        public void Reset(TEntityKey entity)
        {
            foreach (var pool in pools)
            {
                if (pool.TryGetPool(out var p))
                {
                    p.Remove(entity);
                }
            }
        }
        
        /// <summary>
        /// 当一个Entity仍在使用中，但是已经没有挂载任何Component时，我们称这个Entity为Orphan Entity
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool IsOrphan(TEntityKey e)
        {
            AssertValid(e);
            var orphan = true;
            foreach (var pool in pools)
            {
                orphan &= !pool.ReadonlyPool.Contains(e);
            }

            foreach (var tag in tags)
            {
                orphan &= !equalityComparer.Equals(tag.Entity, e);
            }

            return orphan;
        }
        #endregion
        
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<TEntityKey> IEnumerable<TEntityKey>.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public EntityKeyEnumerator GetEnumerator()
        {
            return new EntityKeyEnumerator(entities);
        }
    }
}