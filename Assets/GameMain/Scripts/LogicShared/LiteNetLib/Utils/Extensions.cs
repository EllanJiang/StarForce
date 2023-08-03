/*
* 文件名：Extensions
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:01:26
* 修改记录：
*/

using LogicShared.LiteNetLib.Utils;
using LogicShared.TrueSync.Math;

namespace LogicShared.LiteNetLib.Utils
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

        #region Random

        private static FixRandom Random = FixRandom.New(0);

        private static int randomSeed = 1;
        private static int GetRandomSeed()
        {
            return randomSeed++;
        }
        public static T GetRandomElement<T>(this T[] array)
        {
            Random.Initialize(GetRandomSeed());
            return array[Random.Next(0, array.Length)];
        }
        
        public static Fix64 RandomRange(Fix64 min, Fix64 max)
        {
            Random.Initialize(GetRandomSeed());
            return Random.Next(min, max + 1);
        }
        #endregion
        
    }
}