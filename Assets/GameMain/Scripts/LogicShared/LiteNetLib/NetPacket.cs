/*
* 文件名：NetPacket
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/20 20:30:51
* 修改记录：
*/

using System;
using System.Net;
using LogicShared.LiteNetLib.Utils;

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
        MtuCheck,           //mtu检查
        MtuOk,              //通过mtu检查
        Broadcast,
        Merged,             //合并数据后再发包
        ShutdownOk,
        PeerNotFound,       //没有找到Peer
        InvalidProtocol,    //协议无效
        NatMessage,         //NAT消息
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
        /// 属性类型
        /// </summary>
        public PacketProperty Property
        {
            //0x1F = 0001 1111 =>[0,4]位用来保存属性类型
            //RawData[0]是一个字节大小，8位。从右到左：位数是[0,7]，十进制是[0,255]，二进制是[0000 0000,1111 1111]
            //第一个字节的前5位用来存储属性类型
            get { return (PacketProperty)(RawData[0] & 0x1F); }   
        
            //0xE0 = 1110,0000  =>[5,7]位用来保存其他数据
            //保存属性类型，因为只有[0,4]位用来保存属性类型，[5,7]位的数据不能修改，所以要先用RawData[0]与0xE0进行与操作，保证[5,7]位的数据没有变化之后
            //才能设置[0,4]位的值
            set { RawData[0] = (byte)((RawData[0] & 0xE0) | (byte)value); }
        }

        /// <summary>
        /// 当前连接次数
        /// </summary>
        public byte ConnectionNumber
        {
            //0x60 = 0110,0000 => [5,6]位用来保存连接次数，因此连接次数最大只能是4（NetConstants.MaxConnectionNumber）
            //为什么要右移5呢？
            //因为0110,0000右移5位之后的数值才是连接次数：0110,0000>>5=0000,0011 
            get { return (byte)((RawData[0] & 0x60) >> 5); }
            //0x9F = 10011111 => 只能修改[5,6]位的数据
            set { RawData[0] = (byte) ((RawData[0] & 0x9F) | (value << 5)); }
        }
        
        /// <summary>
        /// 是否分段发送
        /// </summary>
        public bool IsFragmented
        {
            //0x80 = 1000,0000 = 128 =>[7,7]位用来保存是否分段发送
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
        /// 序列号，两个字节，
        /// </summary>
        public ushort Sequence
        {
            get { return BitConverter.ToUInt16(RawData, 1); }
            set { FastBitConverter.GetBytes(RawData, 1, value); }
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
        /// 当前片段所属哪部分
        /// </summary>
        public ushort FragmentPart
        {
            get { return BitConverter.ToUInt16(RawData, 6); }
            set { FastBitConverter.GetBytes(RawData, 6, value); }
        }

        /// <summary>
        /// 片段总数
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

    /// <summary>
    /// 网络请求连接包
    /// </summary>
    internal sealed class NetConnectRequestPacket
    {
        public const int HeaderSize = 14;           //包头长度
        public readonly long ConnectionTime;        //连接Id(使用发送连接请求包时的时间戳当做连接id)
        public readonly byte ConnectionNumber;      //当前连接次数
        public readonly byte[] TargetAddress;       //连接目标地址
        public readonly NetDataReader Data;         //请求包的数据
        
        private NetConnectRequestPacket(long connectionTime, byte connectionNumber, byte[] targetAddress, NetDataReader data)
        {
            ConnectionTime = connectionTime;
            ConnectionNumber = connectionNumber;
            TargetAddress = targetAddress;
            Data = data;
        }
        
        public static int GetProtocolId(NetPacket packet)
        {
            return BitConverter.ToInt32(packet.RawData, 1);
        }

        /// <summary>
        /// 读取NetPacket中的数据来创建NetConnectRequestPacket
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static NetConnectRequestPacket FromData(NetPacket packet)
        {
            if (packet.ConnectionNumber >= NetConstants.MaxConnectionNumber)
                return null;

            //Getting new id for peer
            long connectionId = BitConverter.ToInt64(packet.RawData, 5);
            
            //Get target address
            int addrSize = packet.RawData[13];
            if (addrSize != 16 && addrSize != 28)  //16是IpV4,28是IpV6
                return null;
            byte[] addressBytes = new byte[addrSize];
            Buffer.BlockCopy(packet.RawData, 14, addressBytes, 0, addrSize);  //读取IP地址

            // Read data and create request
            var reader = new NetDataReader(null, 0, 0);
            if (packet.Size > HeaderSize+addrSize)
                reader.SetSource(packet.RawData, HeaderSize + addrSize, packet.Size);

            return new NetConnectRequestPacket(connectionId, packet.ConnectionNumber, addressBytes, reader);
        }

        /// <summary>
        /// 在NetConnectRequestPacket中创建网络包
        /// </summary>
        /// <param name="connectData">请求连接的数据</param>
        /// <param name="addressBytes">请求连接的地址</param>
        /// <param name="connectId">请求连接的ID</param>
        /// <returns></returns>
        public static NetPacket Make(NetDataWriter connectData, SocketAddress addressBytes, long connectId)
        {
            //Make initial packet
            var packet = new NetPacket(PacketProperty.ConnectRequest, connectData.Length+addressBytes.Size);

            //Add data
            FastBitConverter.GetBytes(packet.RawData, 1, NetConstants.ProtocolId);
            FastBitConverter.GetBytes(packet.RawData, 5, connectId);
            packet.RawData[13] = (byte)addressBytes.Size;
            for (int i = 0; i < addressBytes.Size; i++)
                packet.RawData[14+i] = addressBytes[i];
            Buffer.BlockCopy(connectData.Data, 0, packet.RawData, 14+addressBytes.Size, connectData.Length);
            return packet;
        }
    }

    /// <summary>
    /// 接受网络连接包
    /// </summary>
    internal sealed class NetConnectAcceptPacket
    {
        public const int Size = 11;             //包的大小
        public readonly long ConnectionId;      //请求连接的ID
        public readonly byte ConnectionNumber;  //请求连接次数
        public readonly bool IsReusedPeer;      //是否是复用的Peer
        
        private NetConnectAcceptPacket(long connectionId, byte connectionNumber, bool isReusedPeer)
        {
            ConnectionId = connectionId;
            ConnectionNumber = connectionNumber;
            IsReusedPeer = isReusedPeer;
        }

        /// <summary>
        /// 读取NetPacket中的数据来创建NetConnectAcceptPacket
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static NetConnectAcceptPacket FromData(NetPacket packet)
        {
            if (packet.Size > Size)
                return null;

            long connectionId = BitConverter.ToInt64(packet.RawData, 1);
            //check connect num
            byte connectionNumber = packet.RawData[9];
            if (connectionNumber >= NetConstants.MaxConnectionNumber)
                return null;
            //check reused flag
            byte isReused = packet.RawData[10];
            if (isReused > 1)
                return null;

            return new NetConnectAcceptPacket(connectionId, connectionNumber, isReused == 1);
        }

        /// <summary>
        /// 在NetConnectAcceptPacket中创建网络包
        /// </summary>
        /// <param name="connectId"></param>
        /// <param name="connectNum"></param>
        /// <param name="reusedPeer"></param>
        /// <returns></returns>
        public static NetPacket Make(long connectId, byte connectNum, bool reusedPeer)
        {
            var packet = new NetPacket(PacketProperty.ConnectAccept, 0);
            FastBitConverter.GetBytes(packet.RawData, 1, connectId);
            packet.RawData[9] = connectNum;
            packet.RawData[10] = (byte)(reusedPeer ? 1 : 0);
            return packet;
        }
    }
}