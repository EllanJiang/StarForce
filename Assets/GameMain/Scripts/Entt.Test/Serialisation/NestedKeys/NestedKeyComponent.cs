/*
  作者：LTH
  文件描述：
  文件名：NestedKeyComponent
  创建时间：2023/07/17 22:07:SS
*/

using System.Runtime.Serialization;
using Entt.Entities;
using Entt.Entities.Attributes;

namespace Entt.Test.Serialisation.NestedKeys
{
    /// <summary>
    /// 嵌套组件
    /// 
    /// </summary>
    [DataContract]
    [EntityComponent(EntityConstructor.NonConstructable)]
    public class NestedKeyComponent
    {
        [DataMember(Name = "ParentReference", Order = 0)]
        public EntityKey ParentReference { get; set; }  //父Entity

        public NestedKeyComponent(EntityKey parentReference)
        {
            ParentReference = parentReference;
        }
    }
}