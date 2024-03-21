/*
* 文件名：INetSerializable
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/21 15:26:22
* 修改记录：
*/

namespace LogicShared.LiteNetLib.Utils
{
    public interface INetSerializable
    {
        void Serialize(NetDataWriter writer);
        void Deserialize(NetDataReader reader);
    }
}