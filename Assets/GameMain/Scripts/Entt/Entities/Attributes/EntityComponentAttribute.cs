/*
* 文件名：EntityComponentAttribute
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/20 15:09:56
* 修改记录：
*/

using System;
namespace Entt.Entities.Attributes
{
    /// <summary>
    /// Entity或component构造函数属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
    public class EntityComponentAttribute : Attribute
    {
        public EntityComponentAttribute(EntityConstructor constructor = EntityConstructor.Auto)
        {
            Constructor = constructor;
        }
        /// <summary>
        /// Entity或Component的构造函数类型
        /// </summary>
        public EntityConstructor Constructor { get; }
    }
}