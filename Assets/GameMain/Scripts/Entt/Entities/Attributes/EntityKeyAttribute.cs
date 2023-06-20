/*
* 文件名：EntityKeyAttribute
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/20 15:09:09
* 修改记录：
*/

using System;

namespace Entt.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
    public class EntityKeyAttribute:Attribute
    {
        
    }
}