namespace StarForce
{
    public abstract class ClientToServerPacketBase : PacketBase
    {
        public ClientToServerPacketBase()
        {

        }

        public override PacketType PacketType
        {
            get
            {
                return PacketType.ClientToServer;
            }
        }
    }
}
