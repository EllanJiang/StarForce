namespace StarForce
{
    public abstract class ServerToClientPacketBase : PacketBase
    {
        public ServerToClientPacketBase()
        {

        }

        public override PacketType PacketType
        {
            get
            {
                return PacketType.ServerToClient;
            }
        }
    }
}
