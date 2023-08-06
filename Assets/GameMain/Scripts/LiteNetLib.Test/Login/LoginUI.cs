
using GameMain;
using LogicShared.LiteNetLib;
using Protos;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public Button ConnectBtn;
    public Button LoginBtn;

    public RoomUI _RoomUI;
    public BattleUI _BattleUI;

    void Start()
    {
        LoginBtn.onClick .AddListener(OnLoginClick);
        ConnectBtn.onClick.AddListener(OnClickConnect);
        
        //监听登录回调
        OutsideNetManager.Register<LoginRes>(OnLoginRes);
       
        //todo 临时处理，等UI框架做好后再调整
        RoomManager.Instance.SetRoomUI(_RoomUI);
        
        LoginBtn.gameObject.SetActive(false);
        _RoomUI.gameObject.SetActive(false);
    }

    //连接服务器
    private void OnClickConnect()
    {
        var localIp = NetUtils.GetLocalIp(LocalAddrType.IPv4);
        OutsideNetManager.Connect(localIp, 10515, "ExampleGame",OnConnected,null);
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

    private void OnConnected()
    {
        ConnectBtn.gameObject.SetActive(false);
        LoginBtn.gameObject.SetActive(true);
    }
    
    //登录请求回包
    private void OnLoginRes(LoginRes loginRes)
    {
        var ret = loginRes.Result;
        Debug.Log("登录结果：" + ret);
        if (ret)
        {
            gameObject.SetActive(false);
            _RoomUI.gameObject.SetActive(true);
        }
    }
}
