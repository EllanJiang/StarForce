/*
  作者：LTH
  文件描述：
  文件名：EntityBinaryPreProcessorAttribute
  创建时间：2023/07/16 21:07:SS
*/

using System;
namespace Entt.Serialization.Binary.AutoRegistration
{
    /// <summary>
    /// Entity二进制预处理方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class EntityBinaryPreProcessorAttribute : Attribute
    {

    }
}
