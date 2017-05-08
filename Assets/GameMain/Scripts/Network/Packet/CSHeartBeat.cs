using ProtoBuf;
using System;

namespace StarForce
{
    [Serializable, ProtoContract(Name = @"CSHeartBeat")]
    public partial class CSHeartBeat : ClientToServerPacketBase
    {
        public CSHeartBeat()
        {

        }

        public override int PacketId
        {
            get
            {
                return 1;
            }
        }
    }
}
