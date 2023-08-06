/*
  作者：LTH
  文件描述：
  文件名：RoomManager
  创建时间：2023/08/06 11:08:SS
*/

using System.Collections.Generic;
using GameMain;
using LogicShared;
using Protos;

public class RoomManager:Singleton<RoomManager>
{
    //key:roomId
    private Dictionary<int, RoomInfo> m_RoomInfoDict = new Dictionary<int, RoomInfo>();

    private RoomUI m_RoomUI;


    public void Init()
    {
        OutsideNetManager.Register<RoomInfoNotify>(OnRoomInfoNotify);
    }
    public void SetRoomUI(RoomUI roomUI)
    {
        m_RoomUI = roomUI;
    }
    
    private void OnRoomInfoNotify(RoomInfoNotify roomInfoNotify)
    {
        SetRoomInfo(roomInfoNotify);
        
        //显示当前房间内所有玩家id
        var roomInfo = RoomManager.Instance.GetRoomInfo(m_RoomUI.m_CurJoinRoomId);
        if (roomInfo != null)
        {
            var playerIds = "";
            for (int i = 0; i < roomInfo.RoomPlayerIdList.Count; i++)
            {
                playerIds += roomInfo.RoomPlayerIdList[i] + ",";
            }

            m_RoomUI.RoomPlayerIdText.text = playerIds;
        }
    }
    
    //全量更新
    public void SetRoomInfo(RoomInfoNotify roomInfoNotify)
    {
        string allRoomId = "";
        m_RoomInfoDict.Clear();
        for (int i = 0; i < roomInfoNotify.RoomInfoList.Count; i++)
        {
            var roomInfo = roomInfoNotify.RoomInfoList[i];
            m_RoomInfoDict.Add(roomInfo.RoomId,roomInfo);
            allRoomId += roomInfo.RoomId +  " ,";
        }

        m_RoomUI.AllRoomIdText.text = allRoomId;
    }

    public RoomInfo GetRoomInfo(int roomId)
    {
        if (m_RoomInfoDict.TryGetValue(roomId, out var roomInfo))
        {
            return roomInfo;
        }

        return null;
    }
}