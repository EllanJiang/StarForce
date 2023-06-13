/*
* 文件名：HeadMsg
* 文件描述：消息头
* 作者：aronliang
* 创建时间：2023/06/13 16:25:06
* 修改记录：
*/

namespace Protos
{
    public class HeadMsg
    {
        /// <summary>
        /// 消息头长度
        /// </summary>
        public static int HeadMsgLength = 8;
        public int MsgId;
        public int MsgLength;
    }
}