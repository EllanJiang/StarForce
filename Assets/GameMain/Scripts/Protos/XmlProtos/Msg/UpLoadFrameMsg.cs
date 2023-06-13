using System.Collections.Generic;
using System.Text;
using System;
namespace Protos
{
	public class UpLoadFrameMsg:BaseMsg
	{
		public BattleInfo mBattleInfo;
		public override int GetBytesNum()
		{
			int num = 8;
			num += mBattleInfo.GetBytesNum();
			return num;
		}
		public override byte[] Writing()
		{
			int index = 0;
			byte[] bytes = new byte[GetBytesNum()];
			WriteInt(bytes,GetMsgID(),ref index);
			WriteInt(bytes,bytes.Length - 8,ref index);
			WriteData(bytes,mBattleInfo,ref index);;
			return bytes;
		}
		public override int Reading(byte[] bytes, int beginIndex = 0)
		{
			int index = beginIndex;
			mBattleInfo = ReadData<BattleInfo>(bytes, ref index);
			return index - beginIndex;
		}
		public override int GetMsgID()
		{
			return (int)MsgID.UploadFrameMsg;
		}
	}
}