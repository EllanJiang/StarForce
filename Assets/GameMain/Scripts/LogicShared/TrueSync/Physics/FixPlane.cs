/*
* 文件名：FPPlane
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 15:56:17
* 修改记录：
*/
using System;
using LogicShared.TrueSync.Math;

namespace LogicShared.TrueSync.Physics
{
    /// <summary>
    /// 点法式平面方程
    /// 参考：https://zhuanlan.zhihu.com/p/267722955
    /// </summary>
    [Serializable]
	public struct FixPlane
	{
		public FixVector _point;		//平面上任意一点
		public FixVector _normal;		//平面法线

		public static readonly FixPlane Identity = new FixPlane(FixVector.up, FixVector.zero);

		public FixPlane(FixVector point, FixVector normal)
		{
			_point = point;
			_normal = normal;
		}

		public void Set(FixVector point, FixVector normal)
		{
			_point = point;
			_normal = normal;
		}

		public void Move(FixVector moveOffset)
		{
			_point += moveOffset;
		}

		/// <summary>
		/// 点到平面垂点
		/// </summary>
		/// <param name="point"></param>
		/// <param name="plane"></param>
		/// <returns></returns>
		public static FixVector PointToPlane(FixVector point, FixPlane plane)
		{
			return PointToPlane(point, plane._point, plane._normal);
		}
		
		/// <summary>
		/// 点到平面垂点
		/// </summary>
		/// <param name="point"></param>
		/// <param name="planePoint"></param>
		/// <param name="planeNormal"></param>
		/// <returns></returns>
		public static FixVector PointToPlane(FixVector point, FixVector planePoint, FixVector planeNormal)
		{
			var pTarget = point - planePoint;
			var distance = FixVector.Dot(pTarget, planeNormal.normalized);
			var offset = planeNormal * distance;
			
			return point - offset;
		}
		
		/// <summary>
		/// 获取射线与平面的交点
		/// </summary>
		/// <param name="origin"> 射线原点 </param>
		/// <param name="direction"> 射线方向 </param>
		/// <param name="plane"> 平面 </param>
		/// <param name="hitPoint"></param>
		/// <returns></returns>
		public static bool TryGetRayHitPlanePoint(FixVector origin, FixVector direction, FixPlane plane, out FixVector hitPoint)
		{
			return TryGetRayHitPlanePoint(origin, direction, plane._normal, plane._point, out hitPoint);
		}

		/// <summary>
		/// 获取射线与平面的交点
		/// </summary>
		/// <param name="origin"> 射线原点 </param>
		/// <param name="direction"> 射线方向 </param>
		/// <param name="normal"> 平面法线 </param>
		/// <param name="p"> 平面上任意一点 </param>
		/// <param name="hitPoint"></param>
		/// <returns></returns>
		public static bool TryGetRayHitPlanePoint(FixVector origin, FixVector direction, FixVector normal, FixVector p, out FixVector hitPoint)
		{
			hitPoint = FixVector.zero;
			var dotDistance = FixVector.Dot(direction, normal);
			if (FixMath.Abs(dotDistance) < Fix64.Epsilon)	//射线与平面平行，不会相交
			{
				return false;
			}
			
			var t = ((FixVector.Dot(normal, p) - FixVector.Dot(normal, origin)) / dotDistance);
			hitPoint = origin + direction * t;
			
			return true;
		}
	}
}