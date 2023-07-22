/*
* 文件名：BaseChannel
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/20 20:26:59
* 修改记录：
*/

using System.Collections.Generic;

namespace LogicShared.LiteNetLib
{
    /// <summary>
    /// 频道基类
    /// </summary>
    internal abstract class BaseChannel
    {
        public BaseChannel Next;                            //下一个频道
        protected readonly NetPeer Peer;                    //网络Peel
        protected readonly Queue<NetPacket> OutgoingQueue;  //即将派发网络包队列
        
        protected BaseChannel(NetPeer peer)
        {
            Peer = peer;
            OutgoingQueue = new Queue<NetPacket>(64);
        }
        /// <summary>
        /// 即将派发出去的包的数量
        /// </summary>
        public int PacketsInQueue
        {
            get { return OutgoingQueue.Count; }
        }
        
        /// <summary>
        /// 添加Packet到待派发队列中
        /// </summary>
        /// <param name="packet"></param>
        public void AddToQueue(NetPacket packet)
        {
            lock (OutgoingQueue)
                OutgoingQueue.Enqueue(packet);
        }

        /// <summary>
        /// 发送下一个包
        /// </summary>
        public abstract void SendNextPackets();
        
        /// <summary>
        /// 处理收到的网络包
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public abstract bool ProcessPacket(NetPacket packet);
    }
}