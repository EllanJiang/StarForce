/*
  作者：LTH
  文件描述：
  文件名：EntityBinarySerializationAttribute
  创建时间：2023/07/16 21:07:SS
*/

using System;
namespace Entt.Serialization.Binary.AutoRegistration
{
    /// <summary>
    /// Entity二进制序列化类型：类、枚举和结构体
    /// </summary>
    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Enum | AttributeTargets.Struct)]
    public sealed class EntityBinarySerializationAttribute : Attribute
    {
        /// <summary>
        /// 一般是Type.FullName
        /// </summary>
        public string? ComponentTypeId { get; set; }
        /// <summary>
        /// 是否是tag组件
        /// </summary>
        public bool UsedAsTag { get; set; }
    }
}