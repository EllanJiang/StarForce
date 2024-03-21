/*
* 文件名：IProtoID
* 文件描述：
* 作者：aronliang
* 创建时间：2023/08/04 18:48:20
* 修改记录：
*/

namespace LogicShared.LiteNetLib.Utils
{
    public interface IProtoIDGetter
    {
        public int TryGetId<T>();
    }
}