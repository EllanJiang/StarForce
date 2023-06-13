using System.Collections.Generic;
using System.Text;
using System;
namespace Protos
{
	public class FrameInfo:BaseData
	{
		public int mFrameId;
		public MoveInfo mMoveInfo;
		public override int GetBytesNum()
		{
			int num = 0;
			num += 4;
			num += mMoveInfo.GetBytesNum();
			return num;
		}
		public override byte[] Writing()
		{
			int index = 0;
			byte[] bytes = new byte[GetBytesNum()];
			WriteInt(bytes,mFrameId,ref index);;
			WriteData(bytes,mMoveInfo,ref index);;
			return bytes;
		}
		public override int Reading(byte[] bytes, int beginIndex = 0)
		{
			int index = beginIndex;
			mFrameId = ReadInt(bytes, ref index);
			mMoveInfo = ReadData<MoveInfo>(bytes, ref index);
			return index - beginIndex;
		}
	}
}