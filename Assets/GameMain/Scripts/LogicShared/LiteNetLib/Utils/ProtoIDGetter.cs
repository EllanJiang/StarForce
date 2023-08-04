/*
* 文件名：ProtoID
* 文件描述：
* 作者：aronliang
* 创建时间：2023/08/04 18:49:49
* 修改记录：
*/

namespace LogicShared.LiteNetLib.Utils
{
    public class ProtoIDGetter
    {
        public static IProtoIDGetter protoIdGetter;

        public static ulong TryGetId<T>()
        {
            return (ulong)protoIdGetter?.TryGetId<T>();
        }
    }
}