/*
  作者：LTH
  文件描述：
  文件名：NetPacketPool
  创建时间：2023/07/22 19:07:SS
*/

using System;
using System.Threading;

namespace LogicShared.LiteNetLib
{
    /// <summary>
    /// 网络包对象池
    /// </summary>
    internal  sealed class NetPacketPool
    {
        private NetPacket _head;    //对象池中的第一个可用对象
        private int _count;         //对象池中可用对象数量
        
        /// <summary>
        /// 从字节数组中创建网络包
        /// </summary>
        /// <param name="property"></param>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public NetPacket GetWithData(PacketProperty property, byte[] data, int start, int length)
        {
            int headerSize = NetPacket.GetHeaderSize(property);
            NetPacket packet = GetPacket(length + headerSize);
            packet.Property = property;
            Buffer.BlockCopy(data, start, packet.RawData, headerSize, length);
            return packet;
        }
        
        /// <summary>
        /// Get packet with property and size
        /// 返回的packet的RawData中每个元素都是0
        /// </summary>
        /// <param name="property"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public NetPacket GetWithProperty(PacketProperty property, int size)
        {
            NetPacket packet = GetPacket(size + NetPacket.GetHeaderSize(property));
            packet.Property = property;
            return packet;
        }

        /// <summary>
        /// Get packet with property
        /// packet的大小等于包头大小（NetPacket.GetHeaderSize(property)）
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public NetPacket GetWithProperty(PacketProperty property)
        {
            NetPacket packet = GetPacket(NetPacket.GetHeaderSize(property));
            packet.Property = property;
            return packet;
        }

        /// <summary>
        /// 获取一个可用的网络包
        /// </summary>
        /// <param name="size">网络包字节数组大小</param>
        /// <returns></returns>
        public NetPacket GetPacket(int size)
        {
            if (size > NetConstants.MaxPacketSize) 
                return new NetPacket(size);

            NetPacket packet;
            do
            {
                packet = _head;                 //每次都先获取第一个可用对象
                if (packet == null)
                    return new NetPacket(size); //如果第一个可用对象为空，那么直接new一个新的
                //如果_head==packet，则_head=packet.Next
                //Interlocked.CompareExchange返回值是_head修改之前的值
            } while (packet != Interlocked.CompareExchange(ref _head, packet.Next, packet));

            _count--;
            packet.Size = size;
            if (packet.RawData.Length < size)
                packet.RawData = new byte[size];
            return packet;
        }

        /// <summary>
        /// 网络包放回对象池中
        /// </summary>
        /// <param name="packet"></param>
        public void Recycle(NetPacket packet)
        {
            if (packet.RawData.Length > NetConstants.MaxPacketSize || _count >= NetConstants.PacketPoolSize)
            {
                //Don't pool big packets. Save memory
                return;
            }

            _count++;

            //Clean fragmented flag
            packet.RawData[0] = 0;

            do
            {
                packet.Next = _head;
            } while (packet.Next != Interlocked.CompareExchange(ref _head, packet, packet.Next));
        }
    }
}