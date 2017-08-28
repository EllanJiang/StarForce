namespace StarForce
{
    public abstract class PacketHeaderBase
    {
        public PacketHeaderBase(PacketType packetType, int packetId)
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
