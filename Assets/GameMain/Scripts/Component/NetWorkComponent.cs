/*
* 文件名：NetWorkComponent
* 文件描述：
* 作者：aronliang
* 创建时间：2023/08/04 18:56:28
* 修改记录：
*/


using UnityGameFramework.Runtime;

namespace GameMain
{
    public class NetWorkComponent:GameFrameworkComponent
    {
        protected override void Awake()
        {
            base.Awake();
            //设置协议id
            var protoId = new Protos.ProtoID();
            LogicShared.LiteNetLib.Utils.ProtoIDGetter.protoIdGetter = protoId;
        }
    }
}