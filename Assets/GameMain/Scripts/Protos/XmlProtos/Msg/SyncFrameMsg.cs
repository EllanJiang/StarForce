using System.Collections.Generic;
using System.Text;
using System;
namespace Protos
{
	public class SyncFrameMsg:BaseMsg
	{
		public List<BattleInfo> mBattleInfos;
		public override int GetBytesNum()
		{
			int num = 8;
			num += 4;//这4个字节用来存储列表长度
			for (int i = 0; i < mBattleInfos.Count;i++)
			{
				num += mBattleInfos[i].GetBytesNum();
			}
			return num;
		}
		public override byte[] Writing()
		{
			int index = 0;
			byte[] bytes = new byte[GetBytesNum()];
			WriteInt(bytes,GetMsgID(),ref index);
			WriteInt(bytes,bytes.Length - 8,ref index);
			WriteInt(bytes,mBattleInfos.Count,ref index);
			for (int i = 0; i < mBattleInfos.Count;i++)
			{
				WriteData(bytes,mBattleInfos[i],ref index);
			}
			return bytes;
		}
		public override int Reading(byte[] bytes, int beginIndex = 0)
		{
			int index = beginIndex;
			int mBattleInfosCount = ReadInt(bytes, ref index);
			mBattleInfos = new List<BattleInfo>();
			for (int i = 0; i < mBattleInfosCount;i++)
			{
				mBattleInfos.Add(ReadData<BattleInfo>(bytes, ref index));
			}
			return index - beginIndex;
		}
		public override int GetMsgID()
		{
			return (int)MsgID.SyncFrameMsg;
		}
	}
}