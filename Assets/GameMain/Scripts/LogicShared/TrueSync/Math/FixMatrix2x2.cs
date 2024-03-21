/*
* 文件名：FPMatrix2x2
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 15:46:31
* 修改记录：
*/

namespace LogicShared.TrueSync.Math
{
    /// <summary>
    /// 2x2 Matrix
    /// </summary>
    public struct FixMatrix2x2
    {
        public Fix64 M11, M12;
        public Fix64 M21, M22;

        public static readonly FixMatrix2x2 Identity;
        public static readonly FixMatrix2x2 Zero;

        public FixMatrix2x2(Fix64 m11, Fix64 m12, Fix64 m21, Fix64 m22)
        {
            M11 = m11;
            M12 = m12;
            M21 = m21;
            M22 = m22;
        }
		
        static FixMatrix2x2()
        {
            Identity = new FixMatrix2x2 {M11 = Fix64.One, M22 = Fix64.One};
            Zero = new FixMatrix2x2();
        }

        public static FixMatrix2x2 CreateRotation(Fix64 radians)
        {
            CreateRotation(radians, out var result);
            return result;
        }
		
        public static void CreateRotation(Fix64 radians, out FixMatrix2x2 result)
        {
            var num1 = Fix64.Cos(radians);
            var num2 = Fix64.Sin(radians);
            result.M11 = num1;
            result.M12 = -num2;
            result.M21 = num2;
            result.M22 = num1;
        }
    }
}