using System.Collections.Generic;
using System.Text;
using System;
namespace Protos
{
	public class BattleInfo:BaseData
	{
		public int mBattleId;
		public int mPlayerId;
		public FrameInfo mFrameInfo;
		public override int GetBytesNum()
		{
			int num = 0;
			num += 4;
			num += 4;
			num += mFrameInfo.GetBytesNum();
			return num;
		}
		public override byte[] Writing()
		{
			int index = 0;
			byte[] bytes = new byte[GetBytesNum()];
			WriteInt(bytes,mBattleId,ref index);;
			WriteInt(bytes,mPlayerId,ref index);;
			WriteData(bytes,mFrameInfo,ref index);;
			return bytes;
		}
		public override int Reading(byte[] bytes, int beginIndex = 0)
		{
			int index = beginIndex;
			mBattleId = ReadInt(bytes, ref index);
			mPlayerId = ReadInt(bytes, ref index);
			mFrameInfo = ReadData<FrameInfo>(bytes, ref index);
			return index - beginIndex;
		}
	}
}