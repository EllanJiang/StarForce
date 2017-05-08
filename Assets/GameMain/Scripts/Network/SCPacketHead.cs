using ProtoBuf;

namespace StarForce
{
    [ProtoContract]
    public class SCPacketHead : PacketHeadBase
    {
        public SCPacketHead(int packetId)
            : base(PacketType.ServerToClient, packetId)
        {

        }
    }
}
