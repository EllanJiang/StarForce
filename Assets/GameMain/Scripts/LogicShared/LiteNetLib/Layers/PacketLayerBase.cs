/*
  作者：LTH
  文件描述：
  文件名：PacketLayerBase
  创建时间：2023/07/22 23:07:SS
*/

using System.Net;

namespace LogicShared.LiteNetLib.Layers
{
    public abstract class PacketLayerBase
    {
        public readonly int ExtraPacketSizeForLayer;

        protected PacketLayerBase(int extraPacketSizeForLayer)
        {
            ExtraPacketSizeForLayer = extraPacketSizeForLayer;
        }

        public abstract void ProcessInboundPacket(IPEndPoint endPoint, ref byte[] data, ref int offset, ref int length);
        public abstract void ProcessOutBoundPacket(IPEndPoint endPoint, ref byte[] data, ref int offset, ref int length);
    }
}