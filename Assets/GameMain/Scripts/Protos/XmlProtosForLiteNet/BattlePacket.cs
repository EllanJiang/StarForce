using System.Collections.Generic;
using System.Text;
using System;
using LogicShared.LiteNetLib.Utils;
using LogicShared.TrueSync.Math;
using LogicShared;
namespace Protos
{
	public enum EPlayType
	{
		Player = 1,
		Monster,
		Max = 3,
	}


	public class StartBattleReq:INetSerializable,IObjectPool
	{
		public int OwnerPlayerId;
		public int RoomId;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(OwnerPlayerId);
			writer.Put(RoomId);
		}
		public void Deserialize(NetDataReader reader)
		{
			OwnerPlayerId = reader.GetInt();
			RoomId = reader.GetInt();
		}
		public void PutBackPool()
		{
			OwnerPlayerId = default;
			RoomId = default;
		}
	}

	public class StartBattleRes:INetSerializable,IObjectPool
	{
		public bool Result;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(Result);
		}
		public void Deserialize(NetDataReader reader)
		{
			Result = reader.GetBool();
		}
		public void PutBackPool()
		{
			Result = default;
		}
	}

	public class JoinPacket:INetSerializable,IObjectPool
	{
		public string UserName;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(UserName);
		}
		public void Deserialize(NetDataReader reader)
		{
			UserName = reader.GetString();
		}
		public void PutBackPool()
		{
			UserName = default;
		}
	}

	public class JoinAcceptPacket:INetSerializable,IObjectPool
	{
		public byte Id;
		public ushort ServerTick;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(Id);
			writer.Put(ServerTick);
		}
		public void Deserialize(NetDataReader reader)
		{
			Id = reader.GetByte();
			ServerTick = reader.GetUShort();
		}
		public void PutBackPool()
		{
			Id = default;
			ServerTick = default;
		}
	}

	public class PlayerJoinedPacket:INetSerializable,IObjectPool
	{
		public string UserName;
		public bool NewPlayer;
		public byte Health;
		public ushort ServerTick;
		public PlayerState InitialPlayerState;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(UserName);
			writer.Put(NewPlayer);
			writer.Put(Health);
			writer.Put(ServerTick);
			writer.Put(InitialPlayerState);
		}
		public void Deserialize(NetDataReader reader)
		{
			UserName = reader.GetString();
			NewPlayer = reader.GetBool();
			Health = reader.GetByte();
			ServerTick = reader.GetUShort();
			InitialPlayerState = reader.Get<PlayerState>();
		}
		public void PutBackPool()
		{
			UserName = default;
			NewPlayer = default;
			Health = default;
			ServerTick = default;
			InitialPlayerState = default;
		}
	}

	public class PlayerLeavedPacket:INetSerializable,IObjectPool
	{
		public byte Id;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(Id);
		}
		public void Deserialize(NetDataReader reader)
		{
			Id = reader.GetByte();
		}
		public void PutBackPool()
		{
			Id = default;
		}
	}

	public class SpawnPacket:INetSerializable,IObjectPool
	{
		public long PlayerId;
		public FixVector2 Position;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(PlayerId);
			writer.Put(Position);
		}
		public void Deserialize(NetDataReader reader)
		{
			PlayerId = reader.GetLong();
			Position = reader.GetVector2();
		}
		public void PutBackPool()
		{
			PlayerId = default;
			Position = default;
		}
	}

	public class ShootPacket:INetSerializable,IObjectPool
	{
		public byte FromPlayer;
		public ushort CommandId;
		public FixVector2 Hit;
		public ushort ServerTick;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(FromPlayer);
			writer.Put(CommandId);
			writer.Put(Hit);
			writer.Put(ServerTick);
		}
		public void Deserialize(NetDataReader reader)
		{
			FromPlayer = reader.GetByte();
			CommandId = reader.GetUShort();
			Hit = reader.GetVector2();
			ServerTick = reader.GetUShort();
		}
		public void PutBackPool()
		{
			FromPlayer = default;
			CommandId = default;
			Hit = default;
			ServerTick = default;
		}
	}

	public class PlayerInputPacket:INetSerializable,IObjectPool
	{
		public ushort Id;
		public int Keys;
		public float Rotation;
		public ushort ServerTick;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(Id);
			writer.Put(Keys);
			writer.Put(Rotation);
			writer.Put(ServerTick);
		}
		public void Deserialize(NetDataReader reader)
		{
			Id = reader.GetUShort();
			Keys = reader.GetInt();
			Rotation = reader.GetFloat();
			ServerTick = reader.GetUShort();
		}
		public void PutBackPool()
		{
			Id = default;
			Keys = default;
			Rotation = default;
			ServerTick = default;
		}
	}

	public class PlayerState:INetSerializable,IObjectPool
	{
		public byte Id;
		public FixVector2 Position;
		public float Rotation;
		public ushort Tick;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(Id);
			writer.Put(Position);
			writer.Put(Rotation);
			writer.Put(Tick);
		}
		public void Deserialize(NetDataReader reader)
		{
			Id = reader.GetByte();
			Position = reader.GetVector2();
			Rotation = reader.GetFloat();
			Tick = reader.GetUShort();
		}
		public void PutBackPool()
		{
			Id = default;
			Position = default;
			Rotation = default;
			Tick = default;
		}
	}

	public class ServerState:INetSerializable,IObjectPool
	{
		public ushort Tick;
		public ushort LastProcessedCommand;
		public int PlayerStatesCount;
		public PlayerState[] PlayerStates;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(Tick);
			writer.Put(LastProcessedCommand);
			writer.Put(PlayerStatesCount);
			writer.Put(PlayerStates.Length);
			for (int i = 0; i < PlayerStates.Length;i++)
			{
				writer.Put(PlayerStates[i]);
			}
		}
		public void Deserialize(NetDataReader reader)
		{
			Tick = reader.GetUShort();
			LastProcessedCommand = reader.GetUShort();
			PlayerStatesCount = reader.GetInt();
			int PlayerStatesLength = reader.GetInt();
			PlayerStates = new PlayerState[PlayerStatesLength];
			for (int i = 0; i < PlayerStatesLength;i++)
			{
				PlayerStates[i] = reader.Get<PlayerState>();
			}
		}
		public void PutBackPool()
		{
			Tick = default;
			LastProcessedCommand = default;
			PlayerStatesCount = default;
			Array.Clear(PlayerStates,0,PlayerStates.Length);
		}
	}

}