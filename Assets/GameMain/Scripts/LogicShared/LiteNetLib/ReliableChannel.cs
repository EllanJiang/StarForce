/*
  作者：LTH
  文件描述：
  文件名：ReliableChannel
  创建时间：2023/07/22 19:07:SS
*/

using System;

namespace LogicShared.LiteNetLib
{
    /// <summary>
    /// 可靠包频道
    /// </summary>
    internal sealed class ReliableChannel:BaseChannel
    {
        /// <summary>
        /// 待发送的packet
        /// </summary>
        private struct PendingPacket
        {
            private NetPacket _packet;  //网络包      
            private long _timeStamp;    //上次发送时间戳
            private bool _isSent;       //是否已发送

            public override string ToString()
            {
                return _packet == null ? "Empty" : _packet.Sequence.ToString();
            }

            public void Init(NetPacket packet)
            {
                _packet = packet;
                _isSent = false;
            }

            /// <summary>
            /// 向peer发送packet
            /// </summary>
            /// <param name="currentTime">当前发送时间戳</param>
            /// <param name="peer">向该peer发送packet</param>
            public void TrySend(long currentTime, NetPeer peer)
            {
                if (_packet == null)
                    return;
                if (_isSent) //check send time
                {
                    //如果已经发送过了，那么计算距离上次发送的时间间隔(packetHoldTime)，
                    //如果发送间隔大于重发间隔(resendDelay)，那么重新发送
                    double resendDelay = peer.ResendDelay * TimeSpan.TicksPerMillisecond;
                    double packetHoldTime = currentTime - _timeStamp;
                    if (packetHoldTime < resendDelay)
                        return;
                    Logger.Debug($"[RC]Resend: packetHoldTime:{(int)packetHoldTime} > resendDelay:{resendDelay}");
                }
                _timeStamp = currentTime;
                _isSent = true;
                peer.SendUserData(_packet);
            }
            
            public bool Clear(NetPeer peer)
            {
                if (_packet != null)
                {
                    peer.RecycleAndDeliver(_packet);
                    _packet = null;
                    return true;
                }
                return false;
            }
        }
        
        private readonly NetPacket _outgoingAcks;            //for send acks
        private readonly PendingPacket[] _pendingPackets;    //for unacked packets and duplicates
        private readonly NetPacket[] _receivedPackets;       //for order
        private readonly bool[] _earlyReceived;              //for unordered

        private int _localSeqence;
        private int _remoteSequence;
        private int _localWindowStart;
        private int _remoteWindowStart;

        private bool _mustSendAcks;                             //必须发送acks

        private readonly DeliveryMethod _deliveryMethod;
        private readonly bool _ordered;                         //是否有序包
        private readonly int _windowSize;                       //滑动窗口大小
        private const int BitsInByte = 8;
        private readonly byte _id;                              //频道id
        
        public ReliableChannel(NetPeer peer, bool ordered, byte id) : base(peer)
        {
            _id = id;
            _windowSize = NetConstants.DefaultWindowSize;
            _ordered = ordered;
            _pendingPackets = new PendingPacket[_windowSize];
            for (int i = 0; i < _pendingPackets.Length; i++)
                _pendingPackets[i] = new PendingPacket();

            if (_ordered)
            {
                _deliveryMethod = DeliveryMethod.ReliableOrdered;
                _receivedPackets = new NetPacket[_windowSize];
            }
            else
            {
                _deliveryMethod = DeliveryMethod.ReliableUnordered;
                _earlyReceived = new bool[_windowSize];
            }

            _localWindowStart = 0;
            _localSeqence = 0;
            _remoteSequence = 0;
            _remoteWindowStart = 0;
            _outgoingAcks = new NetPacket(PacketProperty.Ack, (_windowSize - 1) / BitsInByte + 2) {ChannelId = id};
        }
        
        /// <summary>
        /// ProcessAck in packet
        /// </summary>
        /// <param name="packet"></param>
        private void ProcessAck(NetPacket packet)
        {
            if (packet.Size != _outgoingAcks.Size)
            {
                Logger.Error("[PA]Invalid acks packet size");
                return;
            }

            ushort ackWindowStart = packet.Sequence;
            int windowRel = NetUtils.RelativeSequenceNumber(_localWindowStart, ackWindowStart);
            if (ackWindowStart >= NetConstants.MaxSequence || windowRel < 0)
            {
                Logger.Error("[PA]Bad window start");
                return;
            }

            //check relevance     
            if (windowRel >= _windowSize)
            {
                Logger.Error("[PA]Old acks");
                return;
            }

            byte[] acksData = packet.RawData;
            lock (_pendingPackets)
            {
                for (int pendingSeq = _localWindowStart;
                    pendingSeq != _localSeqence;
                    pendingSeq = (pendingSeq + 1) % NetConstants.MaxSequence)
                {
                    int rel = NetUtils.RelativeSequenceNumber(pendingSeq, ackWindowStart);
                    if (rel >= _windowSize)
                    {
                        Logger.Error("[PA]REL: " + rel);
                        break;
                    }

                    int pendingIdx = pendingSeq % _windowSize;
                    int currentByte = NetConstants.ChanneledHeaderSize + pendingIdx / BitsInByte;
                    int currentBit = pendingIdx % BitsInByte;
                    if ((acksData[currentByte] & (1 << currentBit)) == 0)
                    {
                        if (Peer.NetManager.EnableStatistics) 
                        {
                            //丢包了
                            Peer.Statistics.IncrementPacketLoss();
                            Peer.NetManager.Statistics.IncrementPacketLoss();
                        }

                        //Skip false ack
                        Logger.Error($"[PA]False ack: {pendingSeq}");
                        continue;
                    }

                    if (pendingSeq == _localWindowStart)
                    {
                        //Move window                
                        _localWindowStart = (_localWindowStart + 1) % NetConstants.MaxSequence;
                    }

                    //clear packet
                    if (_pendingPackets[pendingIdx].Clear(Peer))
                        Logger.Error($"[PA]Removing reliableInOrder ack: {pendingSeq} - true");
                }
            }
        }
        
        /// <summary>
        /// 发送下一个包
        /// </summary>
        public override void SendNextPackets()
        {
            if (_mustSendAcks)
            {
                _mustSendAcks = false;
                Logger.Debug("[RR]SendAcks");
                lock(_outgoingAcks)
                    Peer.SendUserData(_outgoingAcks);
            }

            long currentTime = DateTime.UtcNow.Ticks;
            lock (_pendingPackets)
            {
                //get packets from queue
                lock (OutgoingQueue)
                {
                    while (OutgoingQueue.Count > 0)
                    {
                        int relate = NetUtils.RelativeSequenceNumber(_localSeqence, _localWindowStart);
                        if (relate >= _windowSize)
                            break;

                        var netPacket = OutgoingQueue.Dequeue();
                        netPacket.Sequence = (ushort) _localSeqence;
                        netPacket.ChannelId = _id;
                        _pendingPackets[_localSeqence % _windowSize].Init(netPacket);
                        _localSeqence = (_localSeqence + 1) % NetConstants.MaxSequence;
                    }
                }
                //send
                for (int pendingSeq = _localWindowStart;
                     pendingSeq != _localSeqence;
                     pendingSeq = (pendingSeq + 1) % NetConstants.MaxSequence)
                {
                    _pendingPackets[pendingSeq % _windowSize].TrySend(currentTime, Peer);
                }
            }
        }

        /// <summary>
        /// Process incoming packet
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public override bool ProcessPacket(NetPacket packet)
        {
            if (packet.Property == PacketProperty.Ack)
            {
                ProcessAck(packet);
                return false;
            }
            int seq = packet.Sequence;
            if (seq >= NetConstants.MaxSequence)
            {
                Logger.Error("[RR]Bad sequence");
                return false;
            }

            int relate = NetUtils.RelativeSequenceNumber(seq, _remoteWindowStart);
            int relateSeq = NetUtils.RelativeSequenceNumber(seq, _remoteSequence);

            if (relateSeq > _windowSize)
            {
                Logger.Error("[RR]Bad sequence");
                return false;
            }

            //Drop bad packets
            if (relate < 0)
            {
                //Too old packet doesn't ack
                Logger.Error("[RR]ReliableInOrder too old");
                return false;
            }
            if (relate >= _windowSize * 2)
            {
                //Some very new packet
                Logger.Error("[RR]ReliableInOrder too new");
                return false;
            }

            //If very new - move window
            int ackIdx;
            int ackByte;
            int ackBit;
            lock (_outgoingAcks)
            {
                if (relate >= _windowSize)
                {
                    //New window position
                    int newWindowStart = (_remoteWindowStart + relate - _windowSize + 1) % NetConstants.MaxSequence;
                    _outgoingAcks.Sequence = (ushort) newWindowStart;

                    //Clean old data
                    while (_remoteWindowStart != newWindowStart)
                    {
                        ackIdx = _remoteWindowStart % _windowSize;
                        ackByte = NetConstants.ChanneledHeaderSize + ackIdx / BitsInByte;
                        ackBit = ackIdx % BitsInByte;
                        _outgoingAcks.RawData[ackByte] &= (byte) ~(1 << ackBit);
                        _remoteWindowStart = (_remoteWindowStart + 1) % NetConstants.MaxSequence;
                    }
                }

                //Final stage - process valid packet
                //trigger acks send
                _mustSendAcks = true;
                ackIdx = seq % _windowSize;
                ackByte = NetConstants.ChanneledHeaderSize + ackIdx / BitsInByte;
                ackBit = ackIdx % BitsInByte;
                if ((_outgoingAcks.RawData[ackByte] & (1 << ackBit)) != 0)
                {
                    Logger.Error("[RR]ReliableInOrder duplicate");
                    return false;
                }

                //save ack
                _outgoingAcks.RawData[ackByte] |= (byte) (1 << ackBit);
            }

            //detailed check
            if (seq == _remoteSequence)
            {
                Logger.Error("[RR]ReliableInOrder packet succes");
                Peer.AddReliablePacket(_deliveryMethod, packet);
                _remoteSequence = (_remoteSequence + 1) % NetConstants.MaxSequence;

                if (_ordered)
                {
                    NetPacket p;
                    while ((p = _receivedPackets[_remoteSequence % _windowSize]) != null)
                    {
                        //process holden packet
                        _receivedPackets[_remoteSequence % _windowSize] = null;
                        Peer.AddReliablePacket(_deliveryMethod, p);
                        _remoteSequence = (_remoteSequence + 1) % NetConstants.MaxSequence;
                    }
                }
                else
                {
                    while (_earlyReceived[_remoteSequence % _windowSize])
                    {
                        //process early packet
                        _earlyReceived[_remoteSequence % _windowSize] = false;
                        _remoteSequence = (_remoteSequence + 1) % NetConstants.MaxSequence;
                    }
                }
                return true;
            }

            //holden packet
            if (_ordered)
            {
                _receivedPackets[ackIdx] = packet;
            }
            else
            {
                _earlyReceived[ackIdx] = true;
                Peer.AddReliablePacket(_deliveryMethod, packet);
            }
            return true;
        }
    }
}