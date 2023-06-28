/*
* 文件名：EntitySystemBuilderWithContext
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/28 17:55:59
* 修改记录：
*/

using System.Diagnostics.CodeAnalysis;

namespace Entt.Entities.Systems
{
    /// <summary>
    /// 使用上下文场景EntitySystem
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public readonly partial struct EntitySystemBuilderWithContext<TEntityKey, TContext> 
        where TEntityKey : IEntityKey
    {
        readonly IEntityViewFactory<TEntityKey> reg;
        readonly bool allowParallel;
        
        public EntitySystemBuilderWithContext(IEntityViewFactory<TEntityKey> registry, bool allowParallelExecution)
        {
            this.allowParallel = allowParallelExecution;
            this.reg = registry;
        }
    }
}