/*
  作者：LTH
  文件描述：
  文件名：BattleManager
  创建时间：2023/08/07 23:08:SS
*/

using GameMain;
using LiteNetLib.Test.Client;
using LogicShared;
using Protos;
using UnityEngine;

public class BattleManager:Singleton<BattleManager>
{
    private ClientLogic m_ClientLogic;
    private BattleUI m_BattleUI;
    private RoomUI m_RoomUI;
    public void Init()
    {
        OutsideNetManager.Register<StartBattleNotify>(OnStartBattleNotify);
    }

    public void SetRoomUI(RoomUI roomUI)
    {
        m_RoomUI = roomUI;
    }
    public void SetBattleUI(BattleUI battleUI)
    {
        m_BattleUI = battleUI;
    }
    
    private void OnStartBattleNotify(StartBattleNotify startBattleNotify)
    {
        //开始战斗，连接战斗服
        if (!startBattleNotify.Result)
        {
            return;
        }
        var roomId = startBattleNotify.RoomId;
        var roomInfo = RoomManager.Instance.GetRoomInfo(roomId);
        if (roomInfo == null)
        {
            Debug.LogError($"开始战斗失败，找不到该房间：{roomId}");
            return;
        }
        
        Debug.Log("开始战斗，连接战斗服：" + roomInfo.Ip + " port:" + roomInfo.Port);
        m_RoomUI.gameObject.SetActive(false);
        m_BattleUI.gameObject.SetActive(true);
        m_ClientLogic = m_BattleUI.ClientLogic;
        m_ClientLogic.Connect(roomInfo.Ip,roomInfo.Port,null);
    }
}