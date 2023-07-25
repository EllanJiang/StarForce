/*
* 文件名：NetworkGeneral
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 10:58:19
* 修改记录：
*/

using System;

namespace LiteNetLib.Test.Shared
{
    public static class NetworkGeneral
    {
        public const int ProtocolId = 1;
        public static readonly int PacketTypesCount = Enum.GetValues(typeof(PacketType)).Length;

        public const int MaxGameSequence = 1024;
        public const int HalfMaxGameSequence = MaxGameSequence / 2;

        /// <summary>
        /// 在MaxGameSequence范围内计算计算a-b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>如果返回值小于0，说明a小于b;如果返回值等于0，说明a等于b；否则a大于b</returns>
        public static int SeqDiff(int a, int b)
        {
            return Diff(a, b, HalfMaxGameSequence);
        }
        
        //获取a和b的差值
        //为什么不直接用a - b？
        //因为要处理某些特殊情况：例如 a = 1，b = MaxGameSequence - 1.这种情况下，直接用a - b得出的结果是错误的（结果是2-MaxGameSequence）
        //正确的结果应该是2，这是因为a或b的最大值只能是MaxGameSequence，一旦a或b超过最大值，就会掉过头来，从0开始计数，因此a和b实际的差值就是2
        public static int Diff(int a, int b, int halfMax)
        {
            return (a - b + halfMax*3) % (halfMax*2) - halfMax;
        }
    }
}