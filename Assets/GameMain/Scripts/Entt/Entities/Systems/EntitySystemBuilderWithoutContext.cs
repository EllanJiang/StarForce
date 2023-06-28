/*
* 文件名：EntitySystemBuilderWithoutContext
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/28 17:58:52
* 修改记录：
*/

namespace Entt.Entities.Systems
{
    /// <summary>
    /// 不使用上下文创建EntitySystem
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    public readonly partial struct EntitySystemBuilderWithoutContext<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly IEntityViewFactory<TEntityKey> reg;
        readonly bool allowParallel;

        public EntitySystemBuilderWithoutContext(IEntityViewFactory<TEntityKey> reg, bool allowParallel)
        {
            this.reg = reg;
            this.allowParallel = allowParallel;
        }
    }
}