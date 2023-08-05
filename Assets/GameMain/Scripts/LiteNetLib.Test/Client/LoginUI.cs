
using GameMain;
using LogicShared.LiteNetLib;
using Protos;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public Button ConnectBtn;
    public Button LoginBtn;
    void Start()
    {
        LoginBtn.onClick .AddListener(OnLoginClick);
        ConnectBtn.onClick.AddListener(OnConnect);
        
        //监听登录回调
        OutsideNetManager.Register<LoginRes>(OnLoginRes);
        OutsideNetManager.Register<PlayerInfoNotify>(OnPlayerInfoNotify);
    }

    //连接服务器
    private void OnConnect()
    {
        var localIp = NetUtils.GetLocalIp(LocalAddrType.IPv4);
        OutsideNetManager.Connect(localIp, 10515, "ExampleGame",null,null);
    }
    
    //登录请求
    private void OnLoginClick()
    {
        if (!OutsideNetManager.Connected)
        {
            Debug.LogError("尚未连接服务器，请检查网络！");
            return;
        }

        // TODO 使用对象池创建
        LoginReq loginReq = new LoginReq();
        loginReq.UserName = "测试名称";
        OutsideNetManager.SendPacket(loginReq);
    }

    //登录请求回包
    private void OnLoginRes(LoginRes loginRes)
    {
        var ret = loginRes.Result;
        Debug.Log("登录结果：" + ret);
    }

    private void OnPlayerInfoNotify(PlayerInfoNotify playerInfoNotify)
    {
        Debug.Log($"玩家信息：Id:{playerInfoNotify.PlayerId} Name:{playerInfoNotify.PlayerName}  Gold:{playerInfoNotify.Gold}");
    }
}
