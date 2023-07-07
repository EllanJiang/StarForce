/*
* 文件名：EntityConstructor
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/20 15:11:05
* 修改记录：
*/

namespace Entt.Entities.Attributes
{
    /// <summary>
    /// Entity或Component构造函数类型
    /// </summary>
    public enum EntityConstructor
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        Auto = 0,
        /// <summary>
        /// 没有构造函数
        /// </summary>
        NonConstructable = 1,
        /// <summary>
        /// 标志组件
        /// </summary>
        Flag = 2
    }
}