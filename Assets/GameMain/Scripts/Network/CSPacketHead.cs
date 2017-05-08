using ProtoBuf;

namespace StarForce
{
    [ProtoContract]
    public class CSPacketHead : PacketHeadBase
    {
        public CSPacketHead(int packetId)
            : base(PacketType.ClientToServer, packetId)
        {

        }
    }
}
