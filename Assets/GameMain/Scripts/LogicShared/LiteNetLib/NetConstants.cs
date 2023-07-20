/*
* 文件名：NetConstants
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/20 20:00:06
* 修改记录：
*/

namespace LogicShared.LiteNetLib
{
    /// <summary>
    /// 网络常量
    /// </summary>
    public static class NetConstants
    {
        //can be tuned
        public const int DefaultWindowSize = 64;                //默认窗口大小
        public const int SocketBufferSize = 1024 * 1024;        //1mb
        public const int SocketTTL = 255;                       //TimeToLive 存活时间。指一个封包在经过一个路由器时，可传递的最长距离（跃点数）。
                                                                //每当封包经过一个路由器时，其存活次数就会被减一。当其存活次数为0时，路由器便会取消该封包转发。
                                                                //IP网络的话，会向原封包的发出者传送一个ICMP TTL封包以告知跃点数超限。
                                                                //其设计目的是防止封包因不正确的路由表等原因造成的无限循环而无法送达及耗尽网络资源。

        public const int HeaderSize = 1;                        //包头长度（字节）
        public const int ChanneledHeaderSize = 4;               //频道包头长度（字节）
        public const int FragmentHeaderSize = 6;                //消息片段包头长度（字节）
        public const int FragmentedHeaderTotalSize = ChanneledHeaderSize + FragmentHeaderSize; //包头总长度
        public const ushort MaxSequence = 32768;                //最大序列
        public const ushort HalfMaxSequence = MaxSequence / 2;  

        //protocol
        internal const int ProtocolId = 11;                     //协议ID
        internal const int MaxUdpHeaderSize = 68;               //UDP最大包头长度

        internal static readonly int[] PossibleMtu =            //潜在的最小传输单元大小（字节）
        {
            576  - MaxUdpHeaderSize, //minimal
            1232 - MaxUdpHeaderSize,
            1460 - MaxUdpHeaderSize, //google cloud
            1472 - MaxUdpHeaderSize, //VPN
            1492 - MaxUdpHeaderSize, //Ethernet with LLC and SNAP, PPPoE (RFC 1042)
            1500 - MaxUdpHeaderSize  //Ethernet II (RFC 1191)
        };

        internal static readonly int MaxPacketSize = PossibleMtu[PossibleMtu.Length - 1];   //一个Packet最大尺寸（字节）

        //peer specific
        public const byte MaxConnectionNumber = 4;              //一个NetPeer最大连接数量

        public const int PacketPoolSize = 1000;                 //Packet对象池大小
    }

    /// <summary>
    /// 消息发送类型
    /// </summary>
    public enum DeliveryMethod
    {
        /// <summary>
        /// 不可靠，可丢包，会重复发包，到达顺序不保证
        /// Unreliable. Packets can be dropped, can be duplicated, can arrive without order.
        /// </summary>
        Unreliable = 4,

        /// <summary>
        /// 可靠包，不会丢包，不会重复发包，到达顺序不保证
        /// Reliable. Packets won't be dropped, won't be duplicated, can arrive without order.
        /// </summary>
        ReliableUnordered = 0,

        /// <summary>
        /// 不可靠，可丢包，不会重复发包，保证到达顺序
        /// Unreliable. Packets can be dropped, won't be duplicated, will arrive in order.
        /// </summary>
        Sequenced = 1,

        /// <summary>
        /// 可靠包，不会丢包，不会重复发包，保证到达顺序
        /// Reliable and ordered. Packets won't be dropped, won't be duplicated, will arrive in order.
        /// </summary>
        ReliableOrdered = 2,

        /// <summary>
        /// 只有最后一个包时可靠包，只有最后一个包不会丢包，不会重复发包，保证到达顺序
        /// Reliable only last packet. Packets can be dropped (except the last one), won't be duplicated, will arrive in order.
        /// </summary>
        ReliableSequenced = 3
    }
}