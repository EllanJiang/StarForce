/*
* 文件名：NetPacket
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/20 20:30:51
* 修改记录：
*/

using System;

namespace LogicShared.LiteNetLib
{
    /// <summary>
    /// 网络包属性类型
    /// </summary>
    internal enum PacketProperty : byte
    {
        Unreliable,
        Channeled,
        Ack,
        Ping,
        Pong,
        ConnectRequest,
        ConnectAccept,
        Disconnect,
        UnconnectedMessage,
        MtuCheck,
        MtuOk,
        Broadcast,
        Merged,
        ShutdownOk,
        PeerNotFound,
        InvalidProtocol,
        NatMessage,
        Empty
    }

    
    /// <summary>
    /// 一个网络包
    /// </summary>
    internal sealed class NetPacket
    {
        private static readonly int LastProperty = Enum.GetValues(typeof(PacketProperty)).Length;
        private static readonly int[] HeaderSizes;          //网络包包头大小，不同属性类型，包头大小不一样
        
        //Data
        public byte[] RawData;      //原始字节数组
        public int Size;            //原始字节数组大小

        //Delivery
        public object UserData;     //用户自定义数据

        //Pool node
        public NetPacket Next;      //下一个网络包
        
        static NetPacket()
        {
            HeaderSizes = new int[LastProperty+1];
            for (int i = 0; i < HeaderSizes.Length; i++)
            {
                switch ((PacketProperty)i)
                {
                    case PacketProperty.Channeled:
                    case PacketProperty.Ack:
                        HeaderSizes[i] = NetConstants.ChanneledHeaderSize;
                        break;
                    case PacketProperty.Ping:
                        HeaderSizes[i] = NetConstants.HeaderSize + 2;
                        break;
                    case PacketProperty.ConnectRequest:
                        HeaderSizes[i] = NetConnectRequestPacket.HeaderSize;
                        break;
                    case PacketProperty.ConnectAccept:
                        HeaderSizes[i] = NetConnectAcceptPacket.Size;
                        break;
                    case PacketProperty.Disconnect:
                        HeaderSizes[i] = NetConstants.HeaderSize + 8;
                        break;
                    case PacketProperty.Pong:
                        HeaderSizes[i] = NetConstants.HeaderSize + 10;
                        break;
                    default:
                        HeaderSizes[i] = NetConstants.HeaderSize;
                        break;
                }
            }
        }
        
        /// <summary>
        /// 属性
        /// </summary>
        public PacketProperty Property
        {
            get { return (PacketProperty)(RawData[0] & 0x1F); }
            set { RawData[0] = (byte)((RawData[0] & 0xE0) | (byte)value); }
        }

        /// <summary>
        /// 连接数量
        /// </summary>
        public byte ConnectionNumber
        {
            get { return (byte)((RawData[0] & 0x60) >> 5); }
            set { RawData[0] = (byte) ((RawData[0] & 0x9F) | (value << 5)); }
        }

        /// <summary>
        /// 序列号
        /// </summary>
        public ushort Sequence
        {
            get { return BitConverter.ToUInt16(RawData, 1); }
            set { FastBitConverter.GetBytes(RawData, 1, value); }
        }

        /// <summary>
        /// 是否是消息片段
        /// </summary>
        public bool IsFragmented
        {
            get { return (RawData[0] & 0x80) != 0; }
        }

        /// <summary>
        /// 标记为消息片段
        /// </summary>
        public void MarkFragmented()
        {
            RawData[0] |= 0x80; //set first bit
        }

        /// <summary>
        /// 频道ID
        /// </summary>
        public byte ChannelId
        {
            get { return RawData[3]; }
            set { RawData[3] = value; }
        }

        /// <summary>
        /// 片段ID
        /// </summary>
        public ushort FragmentId
        {
            get { return BitConverter.ToUInt16(RawData, 4); }
            set { FastBitConverter.GetBytes(RawData, 4, value); }
        }

        /// <summary>
        /// 片段部分
        /// </summary>
        public ushort FragmentPart
        {
            get { return BitConverter.ToUInt16(RawData, 6); }
            set { FastBitConverter.GetBytes(RawData, 6, value); }
        }

        /// <summary>
        /// 所有片段
        /// </summary>
        public ushort FragmentsTotal
        {
            get { return BitConverter.ToUInt16(RawData, 8); }
            set { FastBitConverter.GetBytes(RawData, 8, value); }
        }

        /// <summary>
        /// 创建网络包
        /// </summary>
        /// <param name="size">网络包大小</param>
        public NetPacket(int size)
        {
            RawData = new byte[size];
            Size = size;
        }

        /// <summary>
        /// 创建网络包
        /// </summary>
        /// <param name="property">网络包类型</param>
        /// <param name="size">网络包大小</param>
        public NetPacket(PacketProperty property, int size)
        {
            size += GetHeaderSize(property);
            RawData = new byte[size];
            Property = property;
            Size = size;
        }

        /// <summary>
        /// 根据网络包类型获取包头大小
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static int GetHeaderSize(PacketProperty property)
        {
            return HeaderSizes[(int)property];
        }

        /// <summary>
        /// 根据网络包类型获取包头大小
        /// </summary>
        /// <returns></returns>
        public int GetHeaderSize()
        {
            return HeaderSizes[RawData[0] & 0x1F];
        }

        /// <summary>
        /// 从字节数组中创建网络包
        /// </summary>
        /// <param name="data">原始字节数组</param>
        /// <param name="start">开始位置</param>
        /// <param name="packetSize">包体大小</param>
        /// <returns></returns>
        //Packet constructor from byte array
        public bool FromBytes(byte[] data, int start, int packetSize)
        {
            //Reading property
            byte property = (byte)(data[start] & 0x1F);
            bool fragmented = (data[start] & 0x80) != 0;
            int headerSize = HeaderSizes[property];

            if (property > LastProperty || packetSize < headerSize ||
               (fragmented && packetSize < headerSize + NetConstants.FragmentHeaderSize) ||
               data.Length < start + packetSize)
            {
                return false;
            }

            Buffer.BlockCopy(data, start, RawData, 0, packetSize);
            Size = (ushort)packetSize;
            return true;
        }
    }
}