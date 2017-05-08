using GameFramework;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public static class NetworkExtension
    {
        public static int GetOpCode(this NetworkComponent networkComponent, PacketType packetType, int packetId)
        {
            if (packetId < 0 || packetId > 0x00ffffff)
            {
                throw new GameFrameworkException("Packet id is invalid.");
            }

            return ((int)packetType << 24) | packetId;
        }

        public static void ParseOpCode(this NetworkComponent networkComponent, int opCode, out PacketType packetType, out int packetId)
        {
            packetType = (PacketType)(opCode >> 24);
            packetId = opCode & 0x00ffffff;
        }
    }
}
