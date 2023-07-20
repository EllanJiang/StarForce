/*
* 文件名：PacketException
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/20 20:10:18
* 修改记录：
*/

using System;

namespace LogicShared.LiteNetLib
{
    public class InvalidPacketException : ArgumentException
    {
        public InvalidPacketException(string message) : base(message)
        {
        }
    }

    public class TooBigPacketException : InvalidPacketException
    {
        public TooBigPacketException(string message) : base(message)
        {
        }
    }
}