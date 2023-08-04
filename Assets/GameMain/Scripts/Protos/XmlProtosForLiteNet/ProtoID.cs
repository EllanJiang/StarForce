using System.Collections.Generic;
using System.Text;
using System;
using LogicShared.LiteNetLib.Utils;
using LogicShared.TrueSync.Math;
namespace Protos
{
	public class ProtoID : IProtoIDGetter
	{
		private static readonly Dictionary<Type,ulong> _protoIds = new Dictionary<Type,ulong>();
		public ProtoID()
		{
			TryAddId<JoinPacket>(1000);
			TryAddId<JoinAcceptPacket>(1001);
			TryAddId<PlayerJoinedPacket>(1002);
			TryAddId<PlayerLeavedPacket>(1003);
			TryAddId<SpawnPacket>(1004);
			TryAddId<ShootPacket>(1005);
			TryAddId<PlayerInputPacket>(1006);
			TryAddId<PlayerState>(1007);
			TryAddId<ServerState>(1008);
		}

		public ulong TryGetId<T>()
		{
			var type = typeof(T);
			_protoIds.TryGetValue(type,out ulong id);
			return id;
		}

		public void TryAddId<T>(ulong id)
		{
			var type = typeof(T);
			if(!_protoIds.ContainsKey(type))
			{
				_protoIds.Add(type,id);
			}
		}
	}
}