//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

namespace StarForce
{
    public abstract class CSPacketBase : PacketBase
    {
        public override PacketType PacketType
        {
            get
            {
                return PacketType.ClientToServer;
            }
        }
    }
}
