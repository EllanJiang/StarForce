/*
* 文件名：IComponentRegistration
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/21 18:04:12
* 修改记录：
*/

namespace Entt.Entities
{
    public interface IComponentRegistration
    {
        /// <summary>
        /// 该组件在组件列表中的索引
        /// Entity身上挂载了n个组件，该组件在这n个组件中的位置
        /// </summary>
        int Index { get; }
    }
    
    /// <summary>
    /// 组件注册接口
    /// </summary>
    /// <typeparam name="TEntityKey"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface IComponentRegistration<in TEntityKey, T> : IComponentRegistration where TEntityKey: IEntityKey
    {
        /// <summary>
        /// 创建一个组件
        /// </summary>
        /// <returns></returns>
        T Create();
        /// <summary>
        /// 触发析构器
        /// </summary>
        /// <param name="k"></param>
        /// <param name="o"></param>
        void Destruct(TEntityKey k, T o);
        /// <summary>
        /// 是否有析构器？
        /// </summary>
        /// <returns></returns>
        bool HasDestructor();
    }
}