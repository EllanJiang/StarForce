/*
* 文件名：NetStatistics
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/21 20:30:40
* 修改记录：
*/

using System.Threading;

namespace LogicShared.LiteNetLib
{
    /// <summary>
    /// 统计网络包传送过程中的各种参数
    /// </summary>
    public sealed class NetStatistics
    {
        private long _packetsSent;      //已发送网络包个数
        private long _packetsReceived;  //已接收网络包个数
        private long _bytesSent;        //已发送字节个数
        private long _bytesReceived;    //已接收字节个数
        private long _packetLoss;       //丢包个数

        public long PacketsSent 
        {
            get { return Interlocked.Read(ref _packetsSent); }
        }

        public long PacketsReceived 
        {
            get { return Interlocked.Read(ref _packetsReceived); }
        }

        public long BytesSent 
        { 
            get { return Interlocked.Read(ref _bytesSent); }
        }
        public long BytesReceived 
        { 
            get { return Interlocked.Read(ref _bytesReceived); }
        }
        public long PacketLoss 
        { 
            get { return Interlocked.Read(ref _packetLoss); }
        }
        
        public long PacketLossPercent
        {
            get 
            {
                long sent = PacketsSent, loss = PacketLoss;
                
                return sent == 0 ? 0 : loss * 100 / sent;
            }
        }
        
        public void Reset() 
        {
            Interlocked.Exchange(ref _packetsSent, 0);
            Interlocked.Exchange(ref _packetsReceived, 0);
            Interlocked.Exchange(ref _bytesSent, 0);
            Interlocked.Exchange(ref _bytesReceived, 0);
            Interlocked.Exchange(ref _packetLoss, 0);
        }

        public void IncrementPacketsSent() 
        {
            Interlocked.Increment(ref _packetsSent);
        }

        public void IncrementPacketsReceived() 
        {
            Interlocked.Increment(ref _packetsReceived);
        }

        public void AddBytesSent(long bytesSent) 
        {
            Interlocked.Add(ref _bytesSent, bytesSent);
        }

        public void AddBytesReceived(long bytesReceived) 
        {
            Interlocked.Add(ref _bytesReceived, bytesReceived);
        }

        public void IncrementPacketLoss() 
        {
            Interlocked.Increment(ref _packetLoss);
        }

        public void AddPacketLoss(long packetLoss) 
        {
            Interlocked.Add(ref _packetLoss, packetLoss);
        }
        
        public override string ToString()
        {
            return string.Format(
                    "BytesReceived: {0}\nPacketsReceived: {1}\nBytesSent: {2}\nPacketsSent: {3}\nPacketLoss: {4}\nPacketLossPercent: {5}\n",
                    BytesReceived,
                    PacketsReceived,
                    BytesSent,
                    PacketsSent,
                    PacketLoss,
                    PacketLossPercent);
        }
    }
}