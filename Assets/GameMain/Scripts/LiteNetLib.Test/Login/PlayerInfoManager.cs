/*
  作者：LTH
  文件描述：
  文件名：PlayerInfoManager
  创建时间：2023/08/06 10:08:SS
*/

using GameMain;
using LogicShared;
using Protos;
using UnityEngine;
public class PlayerInfoManager:Singleton<PlayerInfoManager>
{
    private PlayerInfo m_PlayerInfo = new PlayerInfo();
    public PlayerInfo PlayerInfo => m_PlayerInfo;
    
    public void Init()
    {
        OutsideNetManager.Register<PlayerInfoNotify>(OnPlayerInfoNotify);
    }
    
    private void OnPlayerInfoNotify(PlayerInfoNotify playerInfoNotify)
    {
        Debug.Log($"玩家信息：Id:{playerInfoNotify.PlayerId} Name:{playerInfoNotify.PlayerName}  Gold:{playerInfoNotify.Gold}");
        SetPlayerInfo(playerInfoNotify);
    }
    
    public void SetPlayerInfo(PlayerInfoNotify notify)
    {
        m_PlayerInfo.PlayerName = notify.PlayerName;
        m_PlayerInfo.PlayerId = notify.PlayerId;
        m_PlayerInfo.Gold = notify.Gold;
    }
    
}

public class PlayerInfo
{
    public string PlayerName;
    public int PlayerId;
    public int Gold;
}