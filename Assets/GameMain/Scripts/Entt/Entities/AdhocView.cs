/*
  作者：LTH
  文件描述：用于entity临时显示，跟PersistentView相对
  文件名：AdhocView
  创建时间：2023/06/23 23:06:SS
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Entt.Entities.Helpers;
using Entt.Entities.Pools;

namespace Entt.Entities
{
    /// <summary>
    /// 用于entity临时显示，跟PersistentView正好相反
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    public sealed class AdhocView<TEntityKey, TComponent> : IEntityView<TEntityKey, TComponent> 
        where TEntityKey : IEntityKey
    {
        /// <summary>
        /// Entity管理器
        /// </summary>
        readonly IEntityPoolAccess<TEntityKey> registry;
        /// <summary>
        /// Entity视图数据
        /// </summary>
        readonly IPool<TEntityKey, TComponent> viewData;
        readonly EventHandler<TEntityKey> onCreated;
        readonly EventHandler<TEntityKey> onDestroyed;
        /// <summary>
        /// 是否已经释放该临时数据
        /// </summary>
        bool disposed;
        public bool AllowParallelExecution { get; set; }
        
        public AdhocView(IEntityPoolAccess<TEntityKey> registry)
        {
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            if (!registry.TryGetWritablePool<TComponent>(out var viewDataRaw))
            {
                throw new ArgumentException("No such pool: " + typeof(TComponent));
            }

            viewData = viewDataRaw;
            onCreated = OnCreated;
            onDestroyed = OnDestroyed;
            this.viewData.Destroyed += onDestroyed;
            this.viewData.Created += onCreated;
        }

        ~AdhocView()
        {
            Dispose(false);
        }
        
        public bool IsValid(TEntityKey entity)
        {
            return registry.IsValid(entity);
        }

        public bool IsOrphan(TEntityKey entity)
        {
            return registry.IsOrphan(entity);
        }

        public void Reset(TEntityKey entity)
        {
            registry.Reset(entity);
        }

        public bool HasTag<TTag>()
        {
            return registry.HasTag<TTag>();
        }

        public bool TryGetTag<TTag>([MaybeNullWhen(false)] out TEntityKey k, [MaybeNullWhen(false)] out TTag tag)
        {
            return registry.TryGetTag(out k, out tag);
        }

        public void AttachTag<TTag>(TEntityKey entity)
        {
            registry.AttachTag<TTag>(entity);
        }

        public void AttachTag<TTag>(TEntityKey entity, in TTag tag)
        {
            registry.AttachTag(entity, in tag);
        }

        public void RemoveTag<TTag>()
        {
            registry.RemoveTag<TTag>();
        }

        public bool HasComponent<TOtherComponent>(TEntityKey entity)
        {
            return registry.HasComponent<TOtherComponent>(entity);
        }

        public TOtherComponent AssignComponent<TOtherComponent>(TEntityKey entity)
        {
            return registry.AssignComponent<TOtherComponent>(entity);
        }

        public void AssignComponent<TOtherComponent>(TEntityKey entity, in TOtherComponent c)
        {
            registry.AssignComponent(entity, in c);
        }

        public bool ReplaceComponent<TOtherComponent>(TEntityKey entity, in TOtherComponent c)
        {
            return registry.ReplaceComponent(entity, in c);
        }

        TOtherComponent IEntityViewControl<TEntityKey>.AssignOrReplace<TOtherComponent>(TEntityKey entity)
        {
            return registry.AssignOrReplace<TOtherComponent>(entity);
        }

        void OnCreated(object sender, TEntityKey e)
        {
            Created?.Invoke(sender, e);
        }

        void OnDestroyed(object sender, TEntityKey e)
        {
            Destroyed?.Invoke(sender, e);
        }

        public bool Contains(TEntityKey e)
        {
            return viewData.Contains(e);
        }

        public void Reserve(int capacity)
        {
            registry.GetPool<TComponent>().Reserve(capacity);
        }

        public void Respect<TOtherComponent>()
        {
            if (registry.TryGetWritablePool<TComponent>(out var wpool) &&
                registry.TryGetPool<TOtherComponent>(out var other))
            {
                wpool.Respect(other);
            }
        }

        public bool GetComponent<TOtherComponent>(TEntityKey entity, [MaybeNullWhen(false)] out TOtherComponent data)
        {
            return registry.GetComponent(entity, out data);
        }

        public void WriteBack<TOtherComponent>(TEntityKey entity, in TOtherComponent data)
        {
            registry.WriteBack(entity, in data);
        }

        public void RemoveComponent<TOtherComponent>(TEntityKey entity)
        {
            registry.RemoveComponent<TOtherComponent>(entity);
        }

        public event EventHandler<TEntityKey>? Destroyed;
        public event EventHandler<TEntityKey>? Created;

        public void Apply(ViewDelegates.Apply<TEntityKey> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this);
            try
            {
                foreach (var ek in p)
                {
                    bulk(this, ek);
                }
            }
            finally
            {
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this);
            try
            {
                foreach (var ek in p)
                {
                    bulk(this, context, ek);
                }
            }
            finally
            {
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        public void Apply(ViewDelegates.Apply<TEntityKey, TComponent> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this);
            try
            {
                foreach (var ek in p)
                {
                    TComponent? d = default;
                    ref readonly var c = ref viewData.TryGetRef(ek, ref d, out var s);
                    if (s)
                    {
                        bulk(this, ek, in c!);
                    }
                }
            }
            finally
            {
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        public void Apply(ViewDelegates.ApplyIn0Out1<TEntityKey, TComponent> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this);
            try
            {
                foreach (var ek in p)
                {
                    TComponent? d = default;
                    ref var c = ref viewData.TryGetModifiableRef(ek, ref d, out var s);
                    if (s)
                    {
                        bulk(this, ek, ref c!);
                    }
                }
            }
            finally
            {
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext context,
                                               ViewDelegates.ApplyWithContext<TEntityKey, TContext, TComponent> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this);
            try
            {
                foreach (var ek in p)
                {
                    TComponent? d = default;
                    ref readonly var c = ref viewData.TryGetRef(ek, ref d, out var s);
                    if (s)
                    {
                        bulk(this, context, ek, in c!);
                    }
                }
            }
            finally
            {
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext context,
                                               ViewDelegates.ApplyWithContextIn0Out1<TEntityKey, TContext, TComponent> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this);
            try
            {
                foreach (var ek in p)
                {
                    TComponent? d = default;
                    ref var c = ref viewData.TryGetModifiableRef(ek, ref d, out var s);
                    if (s)
                    {
                        bulk(this, context, ek, ref c!);
                    }
                }
            }
            finally
            {
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        public void AssignOrReplace<TOtherComponent>(TEntityKey entity)
        {
            registry.AssignOrReplace<TOtherComponent>(entity);
        }

        public void AssignOrReplace<TOtherComponent>(TEntityKey entity, in TOtherComponent c)
        {
            registry.AssignOrReplace(entity, in c);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TEntityKey> GetEnumerator()
        {
            return viewData.GetEnumerator();
        }

        public void Dispose()
        {
            Dispose(true);
            //抑制析构函数（析构函数在CLR层面成为Finalize方法）
            //该行代码的作用是：请求CLR不要调用该对象的析构函数。就是说我已经手动调用Dispose方法释放非托管资源了，
            //CLR就不要再触发我的析构函数了，否则再次执行析构函数就相当于又做了一次清理非托管资源的操作，有可能造成未知风险。
            //参考：https://www.cnblogs.com/huangxincheng/p/12811291.html
            GC.SuppressFinalize(this);
        }

        //SuppressMessage的作用：忽略代码检查时违反检查规则发出的警告，为什么要忽略这个警告？因为并不是所有的警告都是有意义的，对于确定不需要理会的警告，可以直接忽略掉，这就是SuppressMessage的作用
        //参考：https://blog.csdn.net/xingan_xie/article/details/52175416
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            this.viewData.Destroyed -= onDestroyed;
            this.viewData.Created -= onCreated;
        }

        public int EstimatedSize => viewData.Count;

        public void CopyTo(RawList<TEntityKey> k)
        {
            viewData.CopyTo(k);
        }
    }
}