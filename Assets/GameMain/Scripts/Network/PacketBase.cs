using GameFramework;
using GameFramework.Network;
using ProtoBuf;

namespace StarForce
{
    public abstract class PacketBase : Packet, IExtensible
    {
        private IExtension m_ExtensionObject;

        public PacketBase()
        {
            m_ExtensionObject = null;
        }

        public abstract PacketType PacketType
        {
            get;
        }

        public abstract int PacketId
        {
            get;
        }

        public override int Id
        {
            get
            {
                return GameEntry.Network.GetOpCode(PacketType, PacketId);
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref m_ExtensionObject, createIfMissing);
        }
    }
}
