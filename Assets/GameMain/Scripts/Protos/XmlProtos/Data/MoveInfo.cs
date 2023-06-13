using System.Collections.Generic;
using System.Text;
using System;
namespace Protos
{
	public class MoveInfo:BaseData
	{
		public float mMoveDirX;
		public float mMoveDirY;
		public float mMoveDirZ;
		public override int GetBytesNum()
		{
			int num = 0;
			num += 4;
			num += 4;
			num += 4;
			return num;
		}
		public override byte[] Writing()
		{
			int index = 0;
			byte[] bytes = new byte[GetBytesNum()];
			WriteFloat(bytes,mMoveDirX,ref index);;
			WriteFloat(bytes,mMoveDirY,ref index);;
			WriteFloat(bytes,mMoveDirZ,ref index);;
			return bytes;
		}
		public override int Reading(byte[] bytes, int beginIndex = 0)
		{
			int index = beginIndex;
			mMoveDirX = ReadFloat(bytes, ref index);
			mMoveDirY = ReadFloat(bytes, ref index);
			mMoveDirZ = ReadFloat(bytes, ref index);
			return index - beginIndex;
		}
	}
}