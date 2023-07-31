/*
* 文件名：PacketIDs
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/31 16:48:44
* 修改记录：
*/

using System;
using System.Collections.Generic;

namespace LiteNetLib.LiteNetLib.Protos
{
    public static class PacketIDs
    {
        private static ulong _Id = 0;
        private static readonly Dictionary<Type, ulong> _packetIds = new Dictionary<Type, ulong>();
        static PacketIDs()
        {
            AddId<JoinPacket>();
            AddId<JoinAcceptPacket>();
            AddId<PlayerJoinedPacket>();
            AddId<PlayerLeavedPacket>();
            AddId<SpawnPacket>();
            AddId<ShootPacket>();
            AddId<PlayerInputPacket>();
            AddId<PlayerState>();
            AddId<ServerState>();
        }

        public static ulong TryGetId<T>()
        {
            var type = typeof(T);
            _packetIds.TryGetValue(type, out ulong id);
            return id;
        }
        
        private static void AddId<T>()
        {
            var type = typeof(T);
            if (!_packetIds.ContainsKey(type))
            {
                _packetIds.Add(type,_Id++);
            }
        }
    }
}