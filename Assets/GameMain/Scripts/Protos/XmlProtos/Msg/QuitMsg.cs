using System.Collections.Generic;
using System.Text;
using System;
namespace Protos
{
	public class QuitMsg:BaseMsg
	{
		public override int GetBytesNum()
		{
			int num = 8;
			return num;
		}
		public override byte[] Writing()
		{
			int index = 0;
			byte[] bytes = new byte[GetBytesNum()];
			WriteInt(bytes,GetMsgID(),ref index);
			WriteInt(bytes,bytes.Length - 8,ref index);
			return bytes;
		}
		public override int Reading(byte[] bytes, int beginIndex = 0)
		{
			int index = beginIndex;
			return index - beginIndex;
		}
		public override int GetMsgID()
		{
			return (int)MsgID.QuitMsgReq;
		}
	}
}