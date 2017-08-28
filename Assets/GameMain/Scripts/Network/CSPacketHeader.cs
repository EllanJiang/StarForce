using ProtoBuf;

namespace StarForce
{
    [ProtoContract]
    public class CSPacketHeader : PacketHeaderBase
    {
        public CSPacketHeader(int packetId)
            : base(PacketType.ClientToServer, packetId)
        {

        }
    }
}
