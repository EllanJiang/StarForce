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
			TryAddId<JoinPacket>(10000);
			TryAddId<JoinAcceptPacket>(10001);
			TryAddId<PlayerJoinedPacket>(10002);
			TryAddId<PlayerLeavedPacket>(10003);
			TryAddId<SpawnPacket>(10004);
			TryAddId<ShootPacket>(10005);
			TryAddId<PlayerInputPacket>(10006);
			TryAddId<PlayerState>(10007);
			TryAddId<ServerState>(10008);
			TryAddId<LoginReq>(10009);
			TryAddId<LoginRes>(10010);
			TryAddId<PlayerInfoNotify>(10011);
			TryAddId<OpenRoomReq>(20001);
			TryAddId<OpenRoomRes>(20002);
			TryAddId<RoomInfoNotify>(20003);
			TryAddId<JoinRoomReq>(20004);
			TryAddId<JoinRoomRes>(20005);
			TryAddId<StartBattleReq>(30001);
			TryAddId<StartBattleNotify>(30002);
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