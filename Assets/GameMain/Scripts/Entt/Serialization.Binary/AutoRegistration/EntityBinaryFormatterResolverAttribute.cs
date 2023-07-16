/*
  作者：LTH
  文件描述：
  文件名：EntityBinaryFormatterResolverAttribute
  创建时间：2023/07/16 21:07:SS
*/

using System;
namespace Entt.Serialization.Binary.AutoRegistration
{
    /// <summary>
    /// entity二进制解析方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class EntityBinaryFormatterResolverAttribute : Attribute
    {

    }
}