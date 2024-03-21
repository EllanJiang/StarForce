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
    /// 不使用内容构建EntitySystem
    /// 这里的Context应该理解成EntityKey对应的内容
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