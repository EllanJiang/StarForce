using System.Collections.Generic;
using System.Text;
using System;
namespace Protos
{
	public class MsgIDToMsgName
	{
		private Dictionary<int,BaseMsg> mMsgIdToMsgNameDict = new Dictionary<int, BaseMsg>();
		public MsgIDToMsgName()
		{
			mMsgIdToMsgNameDict.Add((int)MsgID.UploadFrameMsg,new UpLoadFrameMsg());
			mMsgIdToMsgNameDict.Add((int)MsgID.SyncFrameMsg,new SyncFrameMsg());
			mMsgIdToMsgNameDict.Add((int)MsgID.StartBattleMsg,new StartBattleMsg());
			mMsgIdToMsgNameDict.Add((int)MsgID.PlayerInfoMsg,new PlayerInfoMsg());
			mMsgIdToMsgNameDict.Add((int)MsgID.PlayerMsgReq,new PlayerMsg());
			mMsgIdToMsgNameDict.Add((int)MsgID.HeartBeatMsgReq,new HeartBeatMsg());
			mMsgIdToMsgNameDict.Add((int)MsgID.QuitMsgReq,new QuitMsg());
		}
		public BaseMsg GetMsg(int msgId)
		{
			if(mMsgIdToMsgNameDict.TryGetValue(msgId, out var msg))
				return msg;
			return null;
		}
		public BaseMsg GetMsg(MsgID msgId)
		{
			return GetMsg((int) msgId);
		}
	}
}