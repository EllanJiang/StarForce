/*
  作者：LTH
  文件描述：
  文件名：BattleUI
  创建时间：2023/08/06 10:08:SS
*/

using LiteNetLib.Test.Client;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI:MonoBehaviour
{
    public ClientLogic ClientLogic;
    public Button QuitRoom;

    private void Start()
    {
        QuitRoom.onClick.AddListener(OnQuitRoom);
    }

    
    private void OnQuitRoom()
    {
        //发送退出房间请求
    }
}