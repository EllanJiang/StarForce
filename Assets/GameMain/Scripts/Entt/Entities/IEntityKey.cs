/*
* 文件名：IEntityKey
* 文件描述：EntityID
* 作者：aronliang
* 创建时间：2023/06/20 15:59:54
* 修改记录：
*/

using System;

namespace Entt.Entities
{
    public interface IEntityKey:IEquatable<IEntityKey>
    {
        /// <summary>
        /// 可以认为是Entity的生存时长
        /// 最小值是-128，最大值是2^7-1，也就是[-128,127]
        /// </summary>
        byte Age { get; }
        
        /// <summary>
        /// Entity的唯一标识
        /// 最小值是-2^21，最大值是2^21-1
        /// </summary>
        int Key { get; }
        
        /// <summary>
        /// 获取该Entity的哈希值
        /// </summary>
        /// <returns></returns>
        int GetHashCode();
        
        /// <summary>
        /// 该Entity是否为空
        /// </summary>
        /// <returns></returns>
        bool IsEmpty();
    }
}