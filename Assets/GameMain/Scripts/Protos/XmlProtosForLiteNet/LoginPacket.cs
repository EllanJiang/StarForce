using System.Collections.Generic;
using System.Text;
using System;
using LogicShared.LiteNetLib.Utils;
using LogicShared.TrueSync.Math;
namespace Protos
{

	public class LoginReq:INetSerializable
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
	}

	public class LoginRes:INetSerializable
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
	}

	public class PlayerInfoNotify:INetSerializable
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
	}

}