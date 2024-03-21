/*
* 文件名：UiController
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 12:02:54
* 修改记录：
*/

using LiteNetLib.Test.Server;
using LogicShared.LiteNetLib;
using UnityEngine;
using UnityEngine.UI;

namespace LiteNetLib.Test.Client
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UiController : MonoBehaviour
    {
        [SerializeField] private GameObject _uiObject;
        [SerializeField] private ClientLogic _clientLogic;
        [SerializeField] private ServerLogic _serverLogic;
        [SerializeField] private InputField _ipField;
        [SerializeField] private Text _disconnectInfoField;

        private void Awake()
        {
            _ipField.text = NetUtils.GetLocalIp(LocalAddrType.IPv4);
        }

        //启动服务器
        public void OnHostClick()
        {
            _serverLogic.StartServer();
            _uiObject.SetActive(false);
            //_clientLogic.Connect("localhost", OnDisconnected);
        }

        //某个客户端断开连接了
        private void OnDisconnected(DisconnectInfo info)
        {
            _uiObject.SetActive(true);
            _disconnectInfoField.text = info.Reason.ToString();
        }

        //启动客户端，并连入服务器
        public void OnConnectClick()
        {
            _uiObject.SetActive(false);
            _clientLogic.Connect(_ipField.text, OnDisconnected);
        }
    }
}