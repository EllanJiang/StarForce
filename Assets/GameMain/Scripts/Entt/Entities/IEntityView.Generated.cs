﻿


//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
 
namespace Entt.Entities
{

    /// <summary>
    /// 临时显示Entities
    /// </summary>
    public interface IEntityView<TEntityKey, T1>  : IEntityView<TEntityKey>
        where TEntityKey: IEntityKey
    {
        void Apply(ViewDelegates.ApplyIn0Out1<TEntityKey, T1> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn0Out1<TEntityKey, TContext, T1> bulk);
        void Apply(ViewDelegates.Apply<TEntityKey, T1> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1> bulk);

    }
    
    /// <summary>
    /// 永久显示Entities
    /// </summary>
    public interface IPersistentEntityView<TEntityKey, T1>  : IEntityView<TEntityKey, T1>
        where TEntityKey: IEntityKey
    {
        int Count { get; }
    }

    /// <summary>
    /// 临时显示Entities
    /// </summary>
    public interface IEntityView<TEntityKey, T1, T2>  : IEntityView<TEntityKey>
        where TEntityKey: IEntityKey
    {
        void Apply(ViewDelegates.ApplyIn0Out2<TEntityKey, T1, T2> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn0Out2<TEntityKey, TContext, T1, T2> bulk);
        void Apply(ViewDelegates.ApplyIn1Out1<TEntityKey, T1, T2> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn1Out1<TEntityKey, TContext, T1, T2> bulk);
        void Apply(ViewDelegates.Apply<TEntityKey, T1, T2> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2> bulk);

    }
    
    /// <summary>
    /// 永久显示Entities
    /// </summary>
    public interface IPersistentEntityView<TEntityKey, T1, T2>  : IEntityView<TEntityKey, T1, T2>
        where TEntityKey: IEntityKey
    {
        int Count { get; }
    }

    /// <summary>
    /// 临时显示Entities
    /// </summary>
    public interface IEntityView<TEntityKey, T1, T2, T3>  : IEntityView<TEntityKey>
        where TEntityKey: IEntityKey
    {
        void Apply(ViewDelegates.ApplyIn0Out3<TEntityKey, T1, T2, T3> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn0Out3<TEntityKey, TContext, T1, T2, T3> bulk);
        void Apply(ViewDelegates.ApplyIn1Out2<TEntityKey, T1, T2, T3> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn1Out2<TEntityKey, TContext, T1, T2, T3> bulk);
        void Apply(ViewDelegates.ApplyIn2Out1<TEntityKey, T1, T2, T3> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn2Out1<TEntityKey, TContext, T1, T2, T3> bulk);
        void Apply(ViewDelegates.Apply<TEntityKey, T1, T2, T3> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3> bulk);

    }
    
    /// <summary>
    /// 永久显示Entities
    /// </summary>
    public interface IPersistentEntityView<TEntityKey, T1, T2, T3>  : IEntityView<TEntityKey, T1, T2, T3>
        where TEntityKey: IEntityKey
    {
        int Count { get; }
    }

    /// <summary>
    /// 临时显示Entities
    /// </summary>
    public interface IEntityView<TEntityKey, T1, T2, T3, T4>  : IEntityView<TEntityKey>
        where TEntityKey: IEntityKey
    {
        void Apply(ViewDelegates.ApplyIn0Out4<TEntityKey, T1, T2, T3, T4> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn0Out4<TEntityKey, TContext, T1, T2, T3, T4> bulk);
        void Apply(ViewDelegates.ApplyIn1Out3<TEntityKey, T1, T2, T3, T4> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn1Out3<TEntityKey, TContext, T1, T2, T3, T4> bulk);
        void Apply(ViewDelegates.ApplyIn2Out2<TEntityKey, T1, T2, T3, T4> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn2Out2<TEntityKey, TContext, T1, T2, T3, T4> bulk);
        void Apply(ViewDelegates.ApplyIn3Out1<TEntityKey, T1, T2, T3, T4> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn3Out1<TEntityKey, TContext, T1, T2, T3, T4> bulk);
        void Apply(ViewDelegates.Apply<TEntityKey, T1, T2, T3, T4> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4> bulk);

    }
    
    /// <summary>
    /// 永久显示Entities
    /// </summary>
    public interface IPersistentEntityView<TEntityKey, T1, T2, T3, T4>  : IEntityView<TEntityKey, T1, T2, T3, T4>
        where TEntityKey: IEntityKey
    {
        int Count { get; }
    }

    /// <summary>
    /// 临时显示Entities
    /// </summary>
    public interface IEntityView<TEntityKey, T1, T2, T3, T4, T5>  : IEntityView<TEntityKey>
        where TEntityKey: IEntityKey
    {
        void Apply(ViewDelegates.ApplyIn0Out5<TEntityKey, T1, T2, T3, T4, T5> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn0Out5<TEntityKey, TContext, T1, T2, T3, T4, T5> bulk);
        void Apply(ViewDelegates.ApplyIn1Out4<TEntityKey, T1, T2, T3, T4, T5> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn1Out4<TEntityKey, TContext, T1, T2, T3, T4, T5> bulk);
        void Apply(ViewDelegates.ApplyIn2Out3<TEntityKey, T1, T2, T3, T4, T5> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn2Out3<TEntityKey, TContext, T1, T2, T3, T4, T5> bulk);
        void Apply(ViewDelegates.ApplyIn3Out2<TEntityKey, T1, T2, T3, T4, T5> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn3Out2<TEntityKey, TContext, T1, T2, T3, T4, T5> bulk);
        void Apply(ViewDelegates.ApplyIn4Out1<TEntityKey, T1, T2, T3, T4, T5> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn4Out1<TEntityKey, TContext, T1, T2, T3, T4, T5> bulk);
        void Apply(ViewDelegates.Apply<TEntityKey, T1, T2, T3, T4, T5> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4, T5> bulk);

    }
    
    /// <summary>
    /// 永久显示Entities
    /// </summary>
    public interface IPersistentEntityView<TEntityKey, T1, T2, T3, T4, T5>  : IEntityView<TEntityKey, T1, T2, T3, T4, T5>
        where TEntityKey: IEntityKey
    {
        int Count { get; }
    }

    /// <summary>
    /// 临时显示Entities
    /// </summary>
    public interface IEntityView<TEntityKey, T1, T2, T3, T4, T5, T6>  : IEntityView<TEntityKey>
        where TEntityKey: IEntityKey
    {
        void Apply(ViewDelegates.ApplyIn0Out6<TEntityKey, T1, T2, T3, T4, T5, T6> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn0Out6<TEntityKey, TContext, T1, T2, T3, T4, T5, T6> bulk);
        void Apply(ViewDelegates.ApplyIn1Out5<TEntityKey, T1, T2, T3, T4, T5, T6> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn1Out5<TEntityKey, TContext, T1, T2, T3, T4, T5, T6> bulk);
        void Apply(ViewDelegates.ApplyIn2Out4<TEntityKey, T1, T2, T3, T4, T5, T6> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn2Out4<TEntityKey, TContext, T1, T2, T3, T4, T5, T6> bulk);
        void Apply(ViewDelegates.ApplyIn3Out3<TEntityKey, T1, T2, T3, T4, T5, T6> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn3Out3<TEntityKey, TContext, T1, T2, T3, T4, T5, T6> bulk);
        void Apply(ViewDelegates.ApplyIn4Out2<TEntityKey, T1, T2, T3, T4, T5, T6> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn4Out2<TEntityKey, TContext, T1, T2, T3, T4, T5, T6> bulk);
        void Apply(ViewDelegates.ApplyIn5Out1<TEntityKey, T1, T2, T3, T4, T5, T6> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn5Out1<TEntityKey, TContext, T1, T2, T3, T4, T5, T6> bulk);
        void Apply(ViewDelegates.Apply<TEntityKey, T1, T2, T3, T4, T5, T6> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4, T5, T6> bulk);

    }
    
    /// <summary>
    /// 永久显示Entities
    /// </summary>
    public interface IPersistentEntityView<TEntityKey, T1, T2, T3, T4, T5, T6>  : IEntityView<TEntityKey, T1, T2, T3, T4, T5, T6>
        where TEntityKey: IEntityKey
    {
        int Count { get; }
    }

    /// <summary>
    /// 临时显示Entities
    /// </summary>
    public interface IEntityView<TEntityKey, T1, T2, T3, T4, T5, T6, T7>  : IEntityView<TEntityKey>
        where TEntityKey: IEntityKey
    {
        void Apply(ViewDelegates.ApplyIn0Out7<TEntityKey, T1, T2, T3, T4, T5, T6, T7> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn0Out7<TEntityKey, TContext, T1, T2, T3, T4, T5, T6, T7> bulk);
        void Apply(ViewDelegates.ApplyIn1Out6<TEntityKey, T1, T2, T3, T4, T5, T6, T7> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn1Out6<TEntityKey, TContext, T1, T2, T3, T4, T5, T6, T7> bulk);
        void Apply(ViewDelegates.ApplyIn2Out5<TEntityKey, T1, T2, T3, T4, T5, T6, T7> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn2Out5<TEntityKey, TContext, T1, T2, T3, T4, T5, T6, T7> bulk);
        void Apply(ViewDelegates.ApplyIn3Out4<TEntityKey, T1, T2, T3, T4, T5, T6, T7> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn3Out4<TEntityKey, TContext, T1, T2, T3, T4, T5, T6, T7> bulk);
        void Apply(ViewDelegates.ApplyIn4Out3<TEntityKey, T1, T2, T3, T4, T5, T6, T7> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn4Out3<TEntityKey, TContext, T1, T2, T3, T4, T5, T6, T7> bulk);
        void Apply(ViewDelegates.ApplyIn5Out2<TEntityKey, T1, T2, T3, T4, T5, T6, T7> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn5Out2<TEntityKey, TContext, T1, T2, T3, T4, T5, T6, T7> bulk);
        void Apply(ViewDelegates.ApplyIn6Out1<TEntityKey, T1, T2, T3, T4, T5, T6, T7> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContextIn6Out1<TEntityKey, TContext, T1, T2, T3, T4, T5, T6, T7> bulk);
        void Apply(ViewDelegates.Apply<TEntityKey, T1, T2, T3, T4, T5, T6, T7> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4, T5, T6, T7> bulk);

    }
    
    /// <summary>
    /// 永久显示Entities
    /// </summary>
    public interface IPersistentEntityView<TEntityKey, T1, T2, T3, T4, T5, T6, T7>  : IEntityView<TEntityKey, T1, T2, T3, T4, T5, T6, T7>
        where TEntityKey: IEntityKey
    {
        int Count { get; }
    }

  

}
