/*
  作者：LTH
  文件描述：
  文件名：EntityXmlSerializationAttribute
  创建时间：2023/07/16 17:07:SS
*/

using System;

namespace Entt.Serialization.Xml.AutoRegistration
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class EntityXmlSerializationAttribute:Attribute
    {
        /// <summary>
        /// 组件类型id，一般是Type.FullName
        /// </summary>
        public string? ComponentTypeId { get; set; }
        /// <summary>
        /// 是否是标签（tag）
        /// </summary>
        public bool UsedAsTag { get; set; }
    }
}