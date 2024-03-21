using System.Collections.Generic;
using System.Text;
using System;
namespace Protos
{
	public class PlayerData:BaseData
	{
		public int id;
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
			WriteInt(bytes,id,ref index);;
			return bytes;
		}
		public override int Reading(byte[] bytes, int beginIndex = 0)
		{
			int index = beginIndex;
			id = ReadInt(bytes, ref index);
			return index - beginIndex;
		}
	}
}