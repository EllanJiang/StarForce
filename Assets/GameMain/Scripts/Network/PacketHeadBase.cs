namespace StarForce
{
    public abstract class PacketHeadBase
    {
        public PacketHeadBase(PacketType packetType, int packetId)
        {
            Id = GameEntry.Network.GetOpCode(packetType, packetId);
        }

        public int Id
        {
            get;
            private set;
        }
    }
}
