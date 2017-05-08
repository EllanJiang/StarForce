using ProtoBuf;
using System;

namespace StarForce
{
    [Serializable, ProtoContract(Name = @"SCHeartBeat")]
    public partial class SCHeartBeat : ServerToClientPacketBase
    {
        public SCHeartBeat()
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
