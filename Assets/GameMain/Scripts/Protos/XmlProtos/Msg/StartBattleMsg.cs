using System.Collections.Generic;
using System.Text;
using System;
namespace Protos
{
	public class StartBattleMsg:BaseMsg
	{
		public int mRandomSeek;
		public override int GetBytesNum()
		{
			int num = 8;
			num += 4;
			return num;
		}
		public override byte[] Writing()
		{
			int index = 0;
			byte[] bytes = new byte[GetBytesNum()];
			WriteInt(bytes,GetMsgID(),ref index);
			WriteInt(bytes,bytes.Length - 8,ref index);
			WriteInt(bytes,mRandomSeek,ref index);;
			return bytes;
		}
		public override int Reading(byte[] bytes, int beginIndex = 0)
		{
			int index = beginIndex;
			mRandomSeek = ReadInt(bytes, ref index);
			return index - beginIndex;
		}
		public override int GetMsgID()
		{
			return (int)MsgID.StartBattleMsg;
		}
	}
}