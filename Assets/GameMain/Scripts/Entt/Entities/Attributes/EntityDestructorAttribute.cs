/*
* 文件名：EntityDestructorAttribute
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/20 15:10:34
* 修改记录：
*/

using System;

namespace Entt.Entities.Attributes
{
    /// <summary>
    /// Entity析构函数属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class EntityDestructorAttribute:Attribute
    {
        
    }
}