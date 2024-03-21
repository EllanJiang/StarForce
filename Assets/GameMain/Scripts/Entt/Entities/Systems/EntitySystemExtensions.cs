/*
* 文件名：EntitySystemExtensions
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/28 17:54:44
* 修改记录：
*/

namespace Entt.Entities.Systems
{
    public static class EntitySystemExtensions
    {
        public static bool GlobalAllowParallel = true;

        /// <summary>
        /// 构建一个Entity系统
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="allowParallel"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public static EntitySystemBuilder<TEntityKey> BuildSystem<TEntityKey>(this IEntityViewFactory<TEntityKey> registry, bool allowParallel = false)
            where TEntityKey : IEntityKey
        {
            return new EntitySystemBuilder<TEntityKey>(registry, GlobalAllowParallel && allowParallel);
        }
    }
}