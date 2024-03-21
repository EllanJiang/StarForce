using System.Collections.Generic;
using System.Text;
using System;
using LogicShared.LiteNetLib.Utils;
using LogicShared.TrueSync.Math;
using LogicShared;
namespace Protos
{

	public class LoginReq:INetSerializable,IObjectPool
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

	public class LoginRes:INetSerializable,IObjectPool
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

	public class PlayerInfoNotify:INetSerializable,IObjectPool
	{
		public int PlayerId;
		public string PlayerName;
		public int Gold;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(PlayerId);
			writer.Put(PlayerName);
			writer.Put(Gold);
		}
		public void Deserialize(NetDataReader reader)
		{
			PlayerId = reader.GetInt();
			PlayerName = reader.GetString();
			Gold = reader.GetInt();
		}
		public void PutBackPool()
		{
			PlayerId = default;
			PlayerName = default;
			Gold = default;
		}
	}

}