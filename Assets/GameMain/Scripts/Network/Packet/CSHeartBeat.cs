using ProtoBuf;
using System;

namespace StarForce
{
    [Serializable, ProtoContract(Name = @"CSHeartBeat")]
    public partial class CSHeartBeat : PacketBase
    {
        public CSHeartBeat()
        {

        }

        public override PacketType PacketType
        {
            get
            {
                return PacketType.ClientToServer;
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
