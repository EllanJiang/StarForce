using System.Collections.Generic;
using System.Text;
using System;
using LogicShared.LiteNetLib.Utils;
using LogicShared.TrueSync.Math;
using LogicShared;
namespace Protos
{

	public class OpenRoomReq:INetSerializable,IObjectPool
	{
		public int PlayerId;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(PlayerId);
		}
		public void Deserialize(NetDataReader reader)
		{
			PlayerId = reader.GetInt();
		}
		public void PutBackPool()
		{
			PlayerId = default;
		}
	}

	public class OpenRoomRes:INetSerializable,IObjectPool
	{
		public bool Result;
		public int RoomId;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(Result);
			writer.Put(RoomId);
		}
		public void Deserialize(NetDataReader reader)
		{
			Result = reader.GetBool();
			RoomId = reader.GetInt();
		}
		public void PutBackPool()
		{
			Result = default;
			RoomId = default;
		}
	}

	public class JoinRoomReq:INetSerializable,IObjectPool
	{
		public int PlayerId;
		public int RoomId;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(PlayerId);
			writer.Put(RoomId);
		}
		public void Deserialize(NetDataReader reader)
		{
			PlayerId = reader.GetInt();
			RoomId = reader.GetInt();
		}
		public void PutBackPool()
		{
			PlayerId = default;
			RoomId = default;
		}
	}

	public class JoinRoomRes:INetSerializable,IObjectPool
	{
		public bool Result;
		public int RoomId;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(Result);
			writer.Put(RoomId);
		}
		public void Deserialize(NetDataReader reader)
		{
			Result = reader.GetBool();
			RoomId = reader.GetInt();
		}
		public void PutBackPool()
		{
			Result = default;
			RoomId = default;
		}
	}

	public class RoomInfoNotify:INetSerializable,IObjectPool
	{
		public List<RoomInfo> RoomInfoList;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(RoomInfoList.Count);
			for (int i = 0; i < RoomInfoList.Count;i++)
			{
				writer.Put(RoomInfoList[i]);
			}
		}
		public void Deserialize(NetDataReader reader)
		{
			int RoomInfoListCount = reader.GetInt();
			RoomInfoList = new List<RoomInfo>();
			for (int i = 0; i < RoomInfoListCount;i++)
			{
				RoomInfoList.Add(reader.Get<RoomInfo>());
			}
		}
		public void PutBackPool()
		{
			RoomInfoList.Clear();
		}
	}

	public class RoomInfo:INetSerializable,IObjectPool
	{
		public int OwnerPlayerId;
		public int RoomId;
		public List<int> RoomPlayerIdList;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(OwnerPlayerId);
			writer.Put(RoomId);
			writer.Put(RoomPlayerIdList.Count);
			for (int i = 0; i < RoomPlayerIdList.Count;i++)
			{
				writer.Put(RoomPlayerIdList[i]);
			}
		}
		public void Deserialize(NetDataReader reader)
		{
			OwnerPlayerId = reader.GetInt();
			RoomId = reader.GetInt();
			int RoomPlayerIdListCount = reader.GetInt();
			RoomPlayerIdList = new List<int>();
			for (int i = 0; i < RoomPlayerIdListCount;i++)
			{
				RoomPlayerIdList.Add(reader.GetInt());
			}
		}
		public void PutBackPool()
		{
			OwnerPlayerId = default;
			RoomId = default;
			RoomPlayerIdList.Clear();
		}
	}

}