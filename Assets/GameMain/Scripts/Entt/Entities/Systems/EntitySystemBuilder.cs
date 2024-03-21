/*
* 文件名：EntitySystemBuilder
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/28 17:52:07
* 修改记录：
*/

namespace Entt.Entities.Systems
{
    /// <summary>
    /// Entity系统构造器
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public readonly struct EntitySystemBuilder<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly IEntityViewFactory<TEntityKey> reg;
        readonly bool allowParallel;
        
        public EntitySystemBuilder(IEntityViewFactory<TEntityKey> reg, bool allowParallel)
        {
            this.reg = reg;
            this.allowParallel = allowParallel;
        }

        public EntitySystemBuilder<TEntityKey> AllowParallelExecution()
        {
            return new EntitySystemBuilder<TEntityKey>(reg, true);
        }
        
        public EntitySystemBuilderWithContext<TEntityKey, TGameContext> WithContext<TGameContext>()
        {
            return new EntitySystemBuilderWithContext<TEntityKey, TGameContext>(reg, EntitySystemExtensions.GlobalAllowParallel && allowParallel);
        }

        public EntitySystemBuilderWithoutContext<TEntityKey> WithoutContext()
        {
            return new EntitySystemBuilderWithoutContext<TEntityKey>(reg, EntitySystemExtensions.GlobalAllowParallel && allowParallel);
        }
    }
}