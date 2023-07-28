/*
* 文件名：FPLine
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 15:54:00
* 修改记录：
*/

using System;
using LogicShared.TrueSync.Math;

namespace LogicShared.TrueSync.Physics
{
	/// <summary>
	/// 两点确定一条线段
	/// </summary>
    [Serializable]
	public struct FixLine
	{
		public FixVector StartPoint;
		public FixVector EndPoint;

		public static readonly FixLine Zero = new FixLine(FixVector.zero, FixVector.zero);
		public static readonly FixLine Up = new FixLine(FixVector.zero, FixVector.up);
		public static readonly FixLine Right = new FixLine(FixVector.zero, FixVector.right);
		public static readonly FixLine Down = new FixLine(FixVector.zero, FixVector.down);

		public FixLine(FixVector startPoint, FixVector endPoint)
		{
			StartPoint = startPoint;
			EndPoint = endPoint;
		}
		
		public void Set(FixVector startPoint, FixVector endPoint)
		{
			StartPoint = startPoint;
			EndPoint = endPoint;
		}

		/// <summary>
		/// 获取线段长度
		/// </summary>
		public Fix64 Length => FixVector.Distance(StartPoint, EndPoint);

		/// <summary>
		/// 直线是否平行
		/// </summary>
		/// <param name="line0"></param>
		/// <param name="line1"></param>
		/// <returns></returns>
		public static bool IsParallel(FixLine line0, FixLine line1)
		{
			var segment0 = line0.StartPoint - line0.EndPoint;
			var segment1 = line1.StartPoint - line1.EndPoint;
			//平行的两条直线的叉乘结果等于0
			return FixVector.Cross(segment0, segment1) == FixVector.zero;
		}

		/// <summary>
		/// 点到直线的最近点
		/// </summary>
		/// <param name="line"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public static FixVector ClosestPointToLine(FixLine line, FixVector target)
		{
			return ClosestPointToLine(line.StartPoint, line.EndPoint, target);
		}
		
		/// <summary>
        /// 点到直线的最近点
        /// 参考：https://www.zhihu.com/question/273507403
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static FixVector ClosestPointToLine(FixVector p1, FixVector p2, FixVector target)
        {
            var p12 = p2 - p1;
            var pTarget1 = target - p1;
            var offset = p12 * pTarget1 / FixVector.DistanceSqr(p1, p2) * p12;
            
            return p1 + offset;
        }

        /// <summary>
        /// 两条直线之间最近点
        /// </summary>
        public static Fix64 NearestPointsBetweenTwoLines(FixLine line0, FixLine line1, out FixVector resultA, out FixVector resultB)
        {
            return NearestPointsBetweenTwoLines(line0.StartPoint, line0.EndPoint, line1.StartPoint, line1.EndPoint, out resultA, out resultB);
        }
        
        /// <summary>
        /// 两条直线之间最近点
        /// 参考：https://wenku.baidu.com/view/45d10141e45c3b3567ec8bcf?pcf=2&bfetype=new&bfetype=new&_wkts_=1690525560653
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="b0"></param>
        /// <param name="b1"></param>
        /// <param name="resultA"></param>
        /// <param name="resultB"></param>
        /// <returns></returns>
        public static Fix64 NearestPointsBetweenTwoLines(FixVector a0, FixVector a1, FixVector b0, FixVector b1, out FixVector resultA, out FixVector resultB)
        {
            if (FixVector.Distance(a0, a1) < Fix64.Epsilon || FixVector.Distance(b0, b1) < Fix64.Epsilon)
            {
                resultA = FixVector.zero;
                resultB = FixVector.zero;
                return Fix64.Zero;
            }
            
            var r = b0 - a0;
            var a10 = a1 - a0;
            var b10 = b1 - b0;

            var ra = FixVector.Dot(r, a10);
            var rb = FixVector.Dot(r, b10);
            var aa = FixVector.Dot(a10, a10);
            var ab = FixVector.Dot(a10, b10);
            var bb = FixVector.Dot(b10, b10);

            var det = aa * bb - ab * ab;
                
            Fix64 s;
            Fix64 t;
            if (det < Fix64.EN6 * aa * bb)
            {
                s = FixMath.Clamp(ra / aa, 0, 1);
                t = 0;
            }
            else
            {
                s = FixMath.Clamp((ra * bb - rb * ab) / det, 0, 1);
                t = FixMath.Clamp((ra * ab - rb * aa) / det, 0, 1);
            }

            var offsetA = FixMath.Clamp((t * ab + ra) / aa, 0, 1);
            var offsetB = FixMath.Clamp((s * ab - rb) / bb, 0, 1);

            resultA = a0 + offsetA * a10;
            resultB = b0 + offsetB * b10;

            return FixVector.Distance(resultA, resultB);
        }

        /// <summary>
        /// 点是否在线段上
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsPointOnSegment(FixVector a0, FixVector a1, FixVector target)
        {
	        var segmentLength = FixVector.Distance(a0, a1);
	        var closeLength = FixVector.Distance(a0, target) + FixVector.Distance(a1, target);
	        if (closeLength >= segmentLength - Fix64.EN3 && closeLength <= segmentLength + Fix64.EN3)
	        {
		        return true;
	        }
	        
            return false;
        }
	}
}