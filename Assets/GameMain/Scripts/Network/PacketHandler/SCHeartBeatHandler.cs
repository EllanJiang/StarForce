using GameFramework.Network;

namespace StarForce
{
    public class SCHeartBeatHandler : PacketHandlerBase
    {
        public override int Id
        {
            get
            {
                return GameEntry.Network.GetOpCode(PacketType.ServerToClient, 1);
            }
        }

        public override void Handle(object sender, Packet packet)
        {
            SCHeartBeat packetImpl = (SCHeartBeat)packet;
        }
    }
}
