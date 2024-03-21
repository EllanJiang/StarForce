/*
* 文件名：PoolFactory
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/21 18:02:58
* 修改记录：
*/

namespace Entt.Entities.Pools
{
    /// <summary>
    /// 对象池工厂
    /// </summary>
    public static class PoolFactory
    {
        /// <summary>
        /// 创建Entity对象池
        /// </summary>
        /// <param name="reg">组件注册器，可用于监听组件的移除</param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IPool<TEntityKey, T> Create<TEntityKey, T>(IComponentRegistration<TEntityKey, T> reg) where TEntityKey : IEntityKey
        {
            if (reg.HasDestructor())
            {
                return new DestructorPool<TEntityKey, T>(reg);
            }

            return new Pool<TEntityKey, T>();
        }

        /// <summary>
        /// 创建Entity或组件标志对象池
        /// </summary>
        /// <param name="sharedData"></param>
        /// <param name="reg"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IPool<TEntityKey, T> CreateFlagPool<TEntityKey, T>(T sharedData, IComponentRegistration<TEntityKey, T> reg)
            where TEntityKey : IEntityKey
        {
            if (reg.HasDestructor())
            {
                return new DestructorFlagPool<TEntityKey, T>(sharedData, reg);
            }

            return new FlagPool<TEntityKey, T>(sharedData);
        }
    }
}