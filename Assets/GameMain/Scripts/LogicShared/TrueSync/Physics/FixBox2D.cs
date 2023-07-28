/*
* 文件名：FPBox2D
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 16:00:41
* 修改记录：
*/

using System.Collections.Generic;
using LogicShared.TrueSync.Math;

namespace LogicShared.TrueSync.Physics
{
	/// <summary>
	/// 2D盒子
	/// </summary>
    public struct FixBox2D : ISupportMappable2D
	{
		private FixVector2 _center;			//盒子中心世界坐标
		private FixVector2 _halfSize;		//盒子一半大小
		private FixMatrix2x2 _orientation;	//盒子旋转矩阵

		public static readonly FixBox2D Zero;

		static FixBox2D()
		{
			Zero = new FixBox2D(FixVector2.zero, FixVector2.zero, Fix64.Zero);
		}
		
		public FixBox2D(FixVector2 center, FixVector2 size, Fix64 radianY)
		{
			_center = center;
			FixVector2.Divide(ref size, 2, out _halfSize);
			FixMatrix2x2.CreateRotation(radianY, out _orientation);
		}

		public FixVector2 Center
		{
			get => _center;
			set => _center = value;
		}

		public FixVector2 Size
		{
			get => _halfSize * 2;
			set => FixVector2.Multiply(ref value, 2, out _halfSize);
		}

		public FixVector2 HalfSize
		{
			get => _halfSize;
			set => _halfSize = value;
		}

		public FixMatrix2x2 Orientation
		{
			get => _orientation;
			set => _orientation = value;
		}

		public void Rotate(Fix64 radianY)
		{
			FixMatrix2x2.CreateRotation(radianY, out _orientation);
		}

		public void Rotate(Fix64 m11, Fix64 m12, Fix64 m21, Fix64 m22)
		{
			_orientation.M11 = m11;
			_orientation.M12 = m12;
			_orientation.M21 = m21;
			_orientation.M22 = m22;
		}

		public void Move(ref FixVector2 offset)
		{
			FixVector2.Add(ref _center, ref offset, out _center);
		}

		public void GetCorners(List<FixVector2> corners)
		{
			if (corners == null)
			{
				return;
			}
            
			var x = _halfSize.x;
			var y = _halfSize.y;

			var c1 = new FixVector2(x, y);
			var c2 = new FixVector2(-x, y);
			var c3 = new FixVector2(-x, -y);
			var c4 = new FixVector2(x, -y);
			InternalGetCoordinatePoint(ref c1);
			InternalGetCoordinatePoint(ref c2);
			InternalGetCoordinatePoint(ref c3);
			InternalGetCoordinatePoint(ref c4);
			corners.Add(c1);
			corners.Add(c2);
			corners.Add(c3);
			corners.Add(c4);
		}

		public void SupportMapping(ref FixVector2 direction, out FixVector2 result)
		{
			result.x = Fix64.Sign(direction.x) * _halfSize.x;
			result.y = Fix64.Sign(direction.y) * _halfSize.y;
		}
		
		private void InternalGetCoordinatePoint(ref FixVector2 point)
		{
			FixVector2.TransformCoordinate(ref point, ref _orientation, out var transPos);
			FixVector2.Add(ref transPos, ref _center, out point);
		}
	}
}