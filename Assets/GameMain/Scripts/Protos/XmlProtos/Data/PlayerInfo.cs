using System.Collections.Generic;
using System.Text;
using System;
namespace Protos
{
	public class PlayerInfo:BaseData
	{
		public int mPlayerId;
		public override int GetBytesNum()
		{
			int num = 0;
			num += 4;
			return num;
		}
		public override byte[] Writing()
		{
			int index = 0;
			byte[] bytes = new byte[GetBytesNum()];
			WriteInt(bytes,mPlayerId,ref index);;
			return bytes;
		}
		public override int Reading(byte[] bytes, int beginIndex = 0)
		{
			int index = beginIndex;
			mPlayerId = ReadInt(bytes, ref index);
			return index - beginIndex;
		}
	}
}