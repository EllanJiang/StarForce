/*
* 文件名：Extensions
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:01:26
* 修改记录：
*/

using LogicShared.LiteNetLib.Utils;
using UnityEngine;

namespace LiteNetLib.Test.Shared
{
    public static class Extensions
    {      
        public static void Put(this NetDataWriter writer, Vector2 vector)
        {
            writer.Put(vector.x);
            writer.Put(vector.y);
        }

        public static Vector2 GetVector2(this NetDataReader reader)
        {
            Vector2 v;
            v.x = reader.GetFloat();
            v.y = reader.GetFloat();
            return v;
        }

        public static T GetRandomElement<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
    }
}