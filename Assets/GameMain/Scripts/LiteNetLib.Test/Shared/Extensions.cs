/*
* 文件名：Extensions
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:01:26
* 修改记录：
*/

using LogicShared.LiteNetLib.Utils;
using LogicShared.TrueSync.Math;

namespace LiteNetLib.Test.Shared
{
    public static class Extensions
    {      
        public static void Put(this NetDataWriter writer, FixVector2 vector)
        {
            writer.Put(vector.x.AsInt());
            writer.Put(vector.y.AsInt());
        }

        public static FixVector2 GetVector2(this NetDataReader reader)
        {
            FixVector2 v;
            v.x = reader.GetInt();
            v.y = reader.GetInt();
            return v;
        }

        public static T GetRandomElement<T>(this T[] array)
        {
            return array[FixRandom.Range(0, array.Length)];
        }
    }
}