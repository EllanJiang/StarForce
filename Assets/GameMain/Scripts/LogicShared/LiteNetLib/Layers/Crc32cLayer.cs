/*
* 文件名：Crc32cLayer
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 10:28:35
* 修改记录：
*/

using System;
using System.Net;
using LogicShared.LiteNetLib.Utils;

namespace LogicShared.LiteNetLib.Layers
{
    /// <summary>
    /// crc：冗余校验码
    /// CRC32 会产生一个32bit的校验值。 由于CRC32产生校验值时源数据块的每一个bit（位）都参与了计算，所以数据块中即使只有一位发生了变化，也会得到不同的CRC32值
    /// </summary>
    public sealed class Crc32cLayer : PacketLayerBase
    {
        public Crc32cLayer() : base(CRC32C.ChecksumSize)
        {

        }

        public override void ProcessInboundPacket(IPEndPoint endPoint, ref byte[] data, ref int offset, ref int length)
        {
            if (length < NetConstants.HeaderSize + CRC32C.ChecksumSize)
            {
                Logger.Error("[Crc32cLayer] DataReceived size: bad!");
                return;
            }

            //校验点
            int checksumPoint = length - CRC32C.ChecksumSize;
            if (CRC32C.Compute(data, offset, checksumPoint) != BitConverter.ToUInt32(data, checksumPoint))
            {
                Logger.Error("[Crc32cLayer] DataReceived checksum: bad!");
                return;
            }
            length -= CRC32C.ChecksumSize;
        }

        public override void ProcessOutBoundPacket(IPEndPoint endPoint, ref byte[] data, ref int offset, ref int length)
        {
            FastBitConverter.GetBytes(data, length, CRC32C.Compute(data, offset, length));
            length += CRC32C.ChecksumSize;
        }
    }
}