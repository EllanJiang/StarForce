/*
  作者：LTH
  文件描述：
  文件名：RoomUI
  创建时间：2023/08/06 10:08:SS
*/

using System;
using GameMain;
using LogicShared;
using Protos;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI:MonoBehaviour
{
    public GameObject BeforeGo, AfterGo;

    public Button OpenRoomBtn, JoinRoomBtn;
    public Button StartBattleBtn, ShowAllRoomBtn;
   
    public InputField JoinRoomIdText;
    public Text AllRoomIdText,RoomPlayerIdText;


    //当前加入的房间ID
    public int m_CurJoinRoomId;
    private void Start()
    {
        OpenRoomBtn.onClick.AddListener(OnOpenRoom);
        JoinRoomBtn.onClick.AddListener(OnJoinRoom);
        
        StartBattleBtn.onClick.AddListener(OnStartBattle);
        ShowAllRoomBtn.onClick.AddListener(OnShowAllRoom);
        
        OutsideNetManager.Register<OpenRoomRes>(OnOpenRoomRes);
        OutsideNetManager.Register<JoinRoomRes>(OnJoinRoomRes);
    }

    private void OnOpenRoom()
    {
        //发送开房间请求
        OpenRoomReq openRoomReq = ObjectPool.GetFromPool<OpenRoomReq>();
        openRoomReq.PlayerId = PlayerInfoManager.Instance.PlayerInfo.PlayerId;
        OutsideNetManager.SendPacket(openRoomReq);
        ObjectPool.PutBackPool(openRoomReq);
    }

    private void OnJoinRoom()
    {
        //加入房间请求
        JoinRoomReq joinRoomReq = ObjectPool.GetFromPool<JoinRoomReq>();
        joinRoomReq.PlayerId = PlayerInfoManager.Instance.PlayerInfo.PlayerId;
        joinRoomReq.RoomId = Convert.ToInt32(JoinRoomIdText.text);
        OutsideNetManager.SendPacket(joinRoomReq);
        ObjectPool.PutBackPool(joinRoomReq);
    }

    private void OnStartBattle()
    {
        //开始战斗（只有房主才能开始战斗）
        var roomInfo = RoomManager.Instance.GetRoomInfo(m_CurJoinRoomId);
        if (roomInfo == null)
        {
            Debug.LogError($"当前房间不存在:{m_CurJoinRoomId}");
            return;
        }

        if (roomInfo.OwnerPlayerId != PlayerInfoManager.Instance.PlayerInfo.PlayerId)
        {
            //不是房主，不能开启战斗
            Debug.LogError("不是房主，不能开启战斗");
        }
        
        StartBattleReq startBattleReq = ObjectPool.GetFromPool<StartBattleReq>();
        startBattleReq.RoomId = m_CurJoinRoomId;
        startBattleReq.OwnerPlayerId = roomInfo.OwnerPlayerId;
        OutsideNetManager.SendPacket(startBattleReq);
        ObjectPool.PutBackPool(startBattleReq);
    }
    
    private void OnShowAllRoom()
    {
        BeforeGo.SetActive(true);
        AfterGo.SetActive(false);
    }
    
    //
    private void OnOpenRoomRes(OpenRoomRes openRoomRes)
    {
        Debug.Log("开房间请求回包：" + openRoomRes.Result + " RoomId:" + openRoomRes.RoomId);
        if (openRoomRes.Result)
        {
            //房主自动设置房间id
            JoinRoomIdText.text = openRoomRes.RoomId.ToString();
        }
    }

    private void OnJoinRoomRes(JoinRoomRes joinRoomRes)
    {
        Debug.Log("加入房间请求回包：" + joinRoomRes.Result + " RoomId:" + joinRoomRes.RoomId);
        if (joinRoomRes.Result)  //加入房间成功，只显示当前房间信息
        {
            BeforeGo.SetActive(false);
            AfterGo.SetActive(true);
            m_CurJoinRoomId = joinRoomRes.RoomId;
        }
    }
}