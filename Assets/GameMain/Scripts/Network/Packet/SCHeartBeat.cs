using ProtoBuf;
using System;

namespace StarForce
{
    [Serializable, ProtoContract(Name = @"SCHeartBeat")]
    public partial class SCHeartBeat : PacketBase
    {
        public SCHeartBeat()
        {

        }

        public override PacketType PacketType
        {
            get
            {
                return PacketType.ServerToClient;
            }
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
