/*
* 文件名：PacketType
* 文件描述：
* 作者：aronliang
* 创建时间：2023/08/04 19:03:47
* 修改记录：
*/

using System;

namespace LogicShared.LiteNetLib.Utils
{
    /// <summary>
    /// Packet类型
    /// </summary>
    public enum PacketType : byte
    {
        // Movement,           //移动
        // Spawn,              //生成
        // ServerState,        //服务器状态
        Serialized,         //自动序列化的包
        //Shoot               //射击
    }
    
    [Flags]
    public enum MovementKeys : byte
    {
        Left = 1 << 1,
        Right = 1 << 2,
        Up = 1 << 3,
        Down = 1 << 4,
        Fire = 1 << 5
    }
}