/*
* 文件名：BaseMsg
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/13 16:13:18
* 修改记录：
*/

namespace Protos
{
    public class BaseMsg : BaseData
    {
        public override int GetBytesNum()
        {
            return 0;
        }

        public override byte[] Writing()
        {
            return null;
        }

        public override int Reading(byte[] bytes, int beginIndex = 0)
        {
            return 0;
        }

        //消息id
        public virtual int GetMsgID()
        {
            return 0;
        }
    };
}