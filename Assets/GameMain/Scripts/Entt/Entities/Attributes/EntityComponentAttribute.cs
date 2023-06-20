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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
    public class EntityComponentAttribute : Attribute
    {
        public EntityComponentAttribute(EntityConstructor constructor = EntityConstructor.Auto)
        {
            Constructor = constructor;
        }
        
        public EntityConstructor Constructor { get; }
    }
}