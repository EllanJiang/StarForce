/*
* 文件名：FPBox
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 16:00:10
* 修改记录：
*/

using System;
using System.Collections.Generic;
using LogicShared.TrueSync.Math;

namespace LogicShared.TrueSync.Physics
{
    /// <summary>
    /// 方形盒子
    /// </summary>
    [Serializable]
    public struct FixBox : ISupportMappable3D
    {
        private static readonly Fix64 PreCollideAABBMag = Fix64.One + Fix64.EN1; 
        
        private FixVector _halfSize;          // 盒子大小的一半
        private FixVector _offset;            // 相对父节点世界坐标偏移
        private FixVector _parentPosition;    // 父节点世界坐标（默认为原点）
        private FixMatrix _orientation;       // 旋转矩阵
        private FixVector _center;            // 盒子中心世界坐标

        private FixBox2D _box2D;              //在XZ平面上的2D投影
        private FixAABB _preCollideAABB;        //预碰撞AABB

        /// <summary>
        /// 创建盒子
        /// </summary>
        /// <param name="parentPosition"> 父节点世界坐标 </param>
        /// <param name="offset"> 相对父节点世界坐标偏移 </param>
        /// <param name="size"> 盒子大小 </param>
        /// <param name="euler"> 旋转欧拉角 </param>
        public FixBox(FixVector parentPosition, FixVector offset, FixVector size, FixVector euler)
        {
            _parentPosition = parentPosition;
            _offset = offset;
            _halfSize = size * Fix64.Half;
            
            _orientation = CreateBoxTransformMatrixByEuler(euler);
            
            _preCollideAABB = FixAABB.zero;
            _center = parentPosition + offset;
            _box2D = FixBox2D.Zero;
            RefreshAABB();
            RefreshBox2D();
        }

        /// <summary>
        /// 创建盒子
        /// </summary>
        /// <param name="halfSize">盒子一半大小</param>
        /// <param name="offset">相对父节点世界坐标偏移</param>
        /// <param name="parentPosition">父节点世界坐标</param>
        /// <param name="orientation">旋转矩阵</param>
        /// <param name="preCollideAABB">预碰撞盒子</param>
        public FixBox(FixVector halfSize, FixVector offset, FixVector parentPosition, FixMatrix orientation, FixAABB preCollideAABB)
        {
            _halfSize = halfSize;
            _offset = offset;
            _parentPosition = parentPosition;
            _orientation = orientation;
            _preCollideAABB = preCollideAABB;
            _center = parentPosition + offset;
            _box2D = FixBox2D.Zero;
            RefreshAABB();
            RefreshBox2D();
        }

        #region 属性

        /// <summary>
        /// XZ轴方向2D投影形状
        /// </summary>
        public ISupportMappable2D Support2D
        {
            get
            {
                return _box2D;
            }
        }
        
        /// <summary>
        /// 盒子一半大小
        /// </summary>
        public FixVector HalfSize
        {
            get
            {
                return _halfSize;
            }
        }

        /// <summary>
        /// 获取碰撞盒中心点世界坐标
        /// </summary>
        public FixVector Center
        {
            get
            {
                return _center;
            }
        }

        /// <summary>
        /// 获取变换矩阵
        /// </summary>
        public FixMatrix Orientation
        {
            get
            {
                return _orientation;
            }
        }
        
        /// <summary>
        /// 获取或设置盒子大小
        /// </summary>
        public FixVector Size
        {
            get
            {
                return _halfSize * 2;
            }
            set
            {
                _halfSize = value * Fix64.Half;
                RefreshAABB();
                RefreshBox2DHalfSize();
            }
        }
        
        /// <summary>
        /// 获取或设置距离父节点世界坐标偏移
        /// </summary>
        public FixVector Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
                FixVector.Add(ref _offset, ref _parentPosition, out _center);
                RefreshAABB();
                RefreshBox2DCenter();
            }
        }
        
        /// <summary>
        /// 获取或设置父节点世界坐标
        /// </summary>
        public FixVector ParentPosition
        {
            get
            {
                return _parentPosition;
            }
            set
            {
                _parentPosition = value;
                FixVector.Add(ref _offset, ref _parentPosition, out _center);
                RefreshAABB();
                RefreshBox2DCenter();
            }
        }
        
        /// <summary>
        /// 获取AABB包围盒
        /// </summary>
        public FixAABB AABB
        {
            get
            {
                return GetAABB();
            }
        }

        /// <summary>
        /// 获取预碰撞AABB包围盒
        /// </summary>
        public FixAABB PreCollideAABB
        {
            get
            {
                return _preCollideAABB;
            }
        }
        
        #endregion

        #region 变换

        /// <summary>
        /// 复制盒子
        /// </summary>
        /// <param name="otherBox"></param>
        public void CopyBox(FixBox otherBox)
        {
            _halfSize = otherBox._halfSize;
            _offset = otherBox._offset;
            _parentPosition = otherBox._parentPosition;
            _orientation = otherBox._orientation;
            _center = _parentPosition + _offset;
            _box2D = FixBox2D.Zero;
            RefreshAABB();
            RefreshBox2D();
        }
        
        /// <summary>
        /// 设置旋转
        /// </summary>
        /// <param name="euler">欧拉角</param>
        public void SetRotate(FixVector euler)
        {
            _orientation = CreateBoxTransformMatrixByEuler(euler);

            RefreshAABB();
            RefreshBox2DOrientation();
        }
        
        /// <summary>
        /// 缩放盒子
        /// </summary>
        /// <param name="scaling"></param>
        public void Scale(FixVector scaling)
        {
            _halfSize.x *= scaling.x;
            _halfSize.y *= scaling.y;
            _halfSize.z *= scaling.z;
            RefreshAABB();
            RefreshBox2DHalfSize();
        }

        /// <summary>
        /// 缩放盒子
        /// </summary>
        /// <param name="scaling"></param>
        public void Scale(Fix64 scaling)
        {
            _halfSize *= scaling;
            RefreshAABB();
            RefreshBox2DHalfSize();
        }

        /// <summary>
        /// 刷新预碰撞AABB包围盒
        /// </summary>
        private void RefreshAABB()
        {
            FixMath.Absolute(ref _orientation, out var abs);
            FixVector.Transform(ref _halfSize, ref abs, out var temp);

            temp *= PreCollideAABBMag;
            _preCollideAABB.Max = temp;
            FixVector.Negate(ref temp, out _preCollideAABB.Min);
            FixVector.Add(ref _preCollideAABB.Min, ref _center, out _preCollideAABB.Min);
            FixVector.Add(ref _preCollideAABB.Max, ref _center, out _preCollideAABB.Max);
        }

        /// <summary>
        /// 获取AABB包围盒
        /// </summary>
        public FixAABB GetAABB()
        {
            FixMath.Absolute(ref _orientation, out var abs);
            FixVector.Transform(ref _halfSize, ref abs, out var temp);

            var result = new FixAABB {Max = temp};
            FixVector.Negate(ref temp, out result.Min);
            FixVector.Add(ref result.Min, ref _center, out result.Min);
            FixVector.Add(ref result.Max, ref _center, out result.Max);

            return result;
        }

        /// <summary>
        /// 刷新XZ轴方向2D投影方盒
        /// </summary>
        private void RefreshBox2D()
        {
            RefreshBox2DCenter();
            RefreshBox2DOrientation();
            RefreshBox2DHalfSize();
        }

        /// <summary>
        /// 刷新XZ轴方向2D投影方盒的中心
        /// </summary>
        private void RefreshBox2DCenter()
        {
            _box2D.Center = new FixVector2(_center.x, _center.z);
        }

        /// <summary>
        /// 刷新XZ轴方向2D投影方盒的旋转矩阵
        /// </summary>
        private void RefreshBox2DOrientation()
        {
            if (_orientation.M32 != Fix64.Zero)
            {
                _box2D.Orientation = FixMatrix2x2.Identity;
            }
            else if (_orientation.M32 == Fix64.Zero && _orientation.M12 == Fix64.Zero)
            {
                _box2D.Orientation = new FixMatrix2x2(_orientation.M11, _orientation.M13, _orientation.M31, _orientation.M33);
            }
            else if (_orientation.M12 != Fix64.Zero)
            {
                _box2D.Orientation = FixMatrix2x2.Identity;
            }
        }

        /// <summary>
        /// 刷新XZ轴方向2D投影方盒的一半大小
        /// </summary>
        private void RefreshBox2DHalfSize()
        {
            _box2D.HalfSize = new FixVector2(_halfSize.x, _halfSize.z);
        }
        
        #endregion
        
        /// <summary>
        /// 从欧拉角创建Box的变换矩阵
        /// </summary>
        /// <param name="euler"></param>
        /// <returns></returns>
        public static FixMatrix CreateBoxTransformMatrixByEuler(FixVector euler)
        {
            if (FixMath.Abs(euler.x) > Fix64.Epsilon)
            {
                var x = euler.x % 90 == 0 ? euler.x + Fix64.Epsilon : euler.x;
                return new FixMatrix(
                    1, 0, 0
                    , 0, 1, 0
                    , 0, -FixMath.Tan(x * Fix64.Deg2Rad), 1);
            }

            if ((FixMath.Abs(euler.y) > Fix64.Epsilon))
            {
                euler.z = 0;
                return FixMatrix.Rotate(euler);
            }

            if((FixMath.Abs(euler.z) > Fix64.Epsilon))
            {
                var z = euler.z % 90 == 0 ? euler.z + Fix64.Epsilon : euler.z;
                return new FixMatrix(
                    1, FixMath.Tan(z * Fix64.Deg2Rad), 0
                    , 0, 1, 0
                    , 0, 0, 1);
            }

            return FixMatrix.Identity;
        }

        public override bool Equals(object obj)
        {
            if (obj is not FixBox box)
            {
                return false;
            }
            
            return _halfSize.Equals(box._halfSize) &&
                   _offset.Equals(box._offset) &&
                   _parentPosition.Equals(box._parentPosition) &&
                   _orientation.Equals(box._orientation) &&
                   _center.Equals(box._center);
        }

        /// <summary>
        /// 相交检测
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool Intersect(FixBox box)
        {
            return false;
        }

        /// <summary>
        /// 点是否在Box内
        /// </summary>
        /// <param name="point"> 点 </param>
        /// <returns></returns>
        public bool Contains(FixVector point)
        {
            var originPoint = InternalGetOriginPoint(point);

            originPoint.x = FixMath.Abs(originPoint.x);
            originPoint.y = FixMath.Abs(originPoint.y);
            originPoint.z = FixMath.Abs(originPoint.z);

            if (originPoint.x <= _halfSize.x + Fix64.Epsilon && originPoint.y <= _halfSize.y + Fix64.Epsilon && originPoint.z <= _halfSize.z + Fix64.Epsilon)
            {
                return true;
            }
            
            return false;
        }
        
        private bool OriginContains(FixVector point)
        {
            point.x = FixMath.Abs(point.x);
            point.y = FixMath.Abs(point.y);
            point.z = FixMath.Abs(point.z);

            if (point.x <= _halfSize.x + Fix64.Epsilon && point.y <= _halfSize.y + Fix64.Epsilon && point.z <= _halfSize.z + Fix64.Epsilon)
            {
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// 获取Box所有顶点
        /// </summary>
        /// <param name="resultList"></param>
        public void GetCorners(List<FixVector> resultList)
        {
            GetUpCorners(resultList);
            GetDownCorners(resultList);
        }

        /// <summary>
        /// 获取Box顶面四个顶点
        /// </summary>
        /// <param name="resultList"></param>
        public void GetUpCorners(List<FixVector> resultList)
        {
            if (resultList == null)
            {
                return;
            }
            
            var x = _halfSize.x;
            var y = _halfSize.y;
            var z = _halfSize.z;

            var v1 = new FixVector(x, y, z);
            var v2 = new FixVector(x, y, -z);
            var v3 = new FixVector(-x, y, -z);
            var v4 = new FixVector(-x, y, z);
            InternalGetCoordinatePoint(ref v1);
            InternalGetCoordinatePoint(ref v2);
            InternalGetCoordinatePoint(ref v3);
            InternalGetCoordinatePoint(ref v4);
            resultList.Add(v1);
            resultList.Add(v2);
            resultList.Add(v3);
            resultList.Add(v4);
        }

        /// <summary>
        /// 获取Box底面四个顶点
        /// </summary>
        /// <param name="resultList"></param>
        public void GetDownCorners(List<FixVector> resultList)
        {
            if (resultList == null)
            {
                return;
            }
            
            var x = _halfSize.x;
            var y = _halfSize.y;
            var z = _halfSize.z;

            var v1 = new FixVector(x, -y, z);
            var v2 = new FixVector(x, -y, -z);
            var v3 = new FixVector(-x, -y, -z);
            var v4 = new FixVector(-x, -y, z);
            InternalGetCoordinatePoint(ref v1);
            InternalGetCoordinatePoint(ref v2);
            InternalGetCoordinatePoint(ref v3);
            InternalGetCoordinatePoint(ref v4);
            resultList.Add(v1);
            resultList.Add(v2);
            resultList.Add(v3);
            resultList.Add(v4);
        }

        /// <summary>
        /// 获取忽略y轴的四边形顶点
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        /// <param name="c4"></param>
        public void GetCorners2D(out FixVector2 c1, out FixVector2 c2, out FixVector2 c3, out FixVector2 c4)
        {
            var x = _halfSize.x;
            var y = _halfSize.y;
            var z = _halfSize.z;
            
            var corner1 = InternalGetCoordinatePoint(new FixVector(x, y, z));
            var corner2 = InternalGetCoordinatePoint(new FixVector(x, y, -z));
            var corner3 = InternalGetCoordinatePoint(new FixVector(-x, y, -z));
            var corner4 = InternalGetCoordinatePoint(new FixVector(-x, y, z));

            c1 = new FixVector2(corner1.x, corner1.z);
            c2 = new FixVector2(corner2.x, corner2.z);
            c3 = new FixVector2(corner3.x, corner3.z);
            c4 = new FixVector2(corner4.x, corner4.z);
        }

        /// <summary>
        /// 获取忽略y轴的四边形顶点
        /// </summary>
        /// <param name="resultList"></param>
        public void GetCorners2D(List<FixVector2> resultList)
        {
            var x = _halfSize.x;
            var y = _halfSize.y;
            var z = _halfSize.z;
            
            var corner1 = InternalGetCoordinatePoint(new FixVector(x, y, z));
            var corner2 = InternalGetCoordinatePoint(new FixVector(x, y, -z));
            var corner3 = InternalGetCoordinatePoint(new FixVector(-x, y, -z));
            var corner4 = InternalGetCoordinatePoint(new FixVector(-x, y, z));

            resultList.Add(new FixVector2(corner1.x, corner1.z));
            resultList.Add(new FixVector2(corner2.x, corner2.z));
            resultList.Add(new FixVector2(corner3.x, corner3.z));
            resultList.Add(new FixVector2(corner4.x, corner4.z));
        }

        /// <summary>
        /// 获取所有面
        /// </summary>
        /// <param name="resultList"></param>
        public void GetAllPlane(List<FixPlane> resultList)
        {
            if (resultList == null)
            {
                return;
            }

            GetUpPlane(resultList);
            GetAroundPlane(resultList);
            GetDownPlane(resultList);
        }

        /// <summary>
        /// 获取上平面
        /// </summary>
        /// <param name="plane"></param>
        public void GetUpPlane(out FixPlane plane)
        {
            var point = InternalGetCoordinatePoint(_halfSize);
            var normal = FixVector.TransformCoordinate(FixVector.up, FixMatrix.Inverse(FixMatrix.Transpose(_orientation)));
            plane = new FixPlane(point, normal);
        }
        
        /// <summary>
        /// 获取上平面
        /// </summary>
        /// <param name="resultList"></param>
        public void GetUpPlane(List<FixPlane> resultList)
        {
            if (resultList == null)
            {
                return;
            }

            var point = InternalGetCoordinatePoint(_halfSize);
            var normal = FixVector.TransformCoordinate(FixVector.up, FixMatrix.Inverse(FixMatrix.Transpose(_orientation)));
            
            resultList.Add(new FixPlane(point, normal));
        }
        
        /// <summary>
        /// 获取下平面
        /// </summary>
        /// <param name="resultList"></param>
        public void GetDownPlane(List<FixPlane> resultList)
        {
            if (resultList == null)
            {
                return;
            }

            var point = InternalGetCoordinatePoint(_halfSize * -1);
            var normal = FixVector.TransformCoordinate(FixVector.down, FixMatrix.Inverse(FixMatrix.Transpose(_orientation)));
            
            resultList.Add(new FixPlane(point, normal));
        }
        
        /// <summary>
        /// 获取上下平面外所有平面
        /// </summary>
        /// <param name="resultList"></param>
        public void GetAroundPlane(List<FixPlane> resultList)
        {
            if (resultList == null)
            {
                return;
            }

            var vertex = _halfSize;
            for (var i = 0; i < 2; i++)
            {
                FixVector point;
                FixVector normal;
                
                point = InternalGetCoordinatePoint(vertex);
                normal = FixVector.TransformCoordinate(FixVector.right, FixMatrix.Inverse(FixMatrix.Transpose(_orientation)));
                resultList.Add(new FixPlane(point, normal));
                
                point = InternalGetCoordinatePoint(vertex);
                normal = FixVector.TransformCoordinate(FixVector.forward, FixMatrix.Inverse(FixMatrix.Transpose(_orientation)));
                resultList.Add(new FixPlane(point, normal));

                vertex *= -1;
            }
        }
        
        /// <summary>
        /// 直线与Box边上的最近点
        /// </summary>
        /// <param name="a0"> 直线点a0 </param>
        /// <param name="a1"> 直线中一点a1 </param>
        /// <param name="resultA"> 直线上的最近点 </param>
        /// <param name="resultB"> Box边上的最近点 </param>
        /// <returns></returns>
        public Fix64 NearestPointsBetweenToBorders(FixVector a0, FixVector a1, out FixVector resultA, out FixVector resultB)
        {
            resultA = FixVector.zero;
            resultB = FixVector.zero;
            if (FixVector.Distance(a0, a1) < Fix64.Epsilon || _halfSize.magnitude < Fix64.Epsilon)
            {
                return Fix64.Zero;
            }

            var targetLine = new FixLine(InternalGetOriginPoint(a0), InternalGetOriginPoint(a1));

            var borders = GetOriginBorders();
            var tempDistance = Fix64.MaxValue;
            var closestA = FixVector.zero;
            var closestB = FixVector.zero;
            foreach (var border in borders)
            {
                var distance = FixLine.NearestPointsBetweenTwoLines(targetLine, border, out var aPoint, out var bPoint);
                if (distance < tempDistance)
                {
                    tempDistance = distance;
                    closestA = aPoint;
                    closestB = bPoint;
                }
            }

            resultA = InternalGetCoordinatePoint(closestA);
            resultB = InternalGetCoordinatePoint(closestB);

            return tempDistance;
        }

        private FixLine[] GetOriginBorders()
        {
            var x = _halfSize.x;
            var y = _halfSize.y;
            var z = _halfSize.z;
            var corner1 = new FixVector(x, y, z);
            var corner2 = new FixVector(-x, -y, z);
            var corner3 = new FixVector(x, -y, -z);
            var corner4 = new FixVector(-x, y, -z);

            var lines = new[]
            {
                new FixLine(corner1, new FixVector(-corner1.x, corner1.y, corner1.z)),
                new FixLine(corner1, new FixVector(corner1.x, -corner1.y, corner1.z)),
                new FixLine(corner1, new FixVector(corner1.x, corner1.y, -corner1.z)),
                new FixLine(corner2, new FixVector(-corner2.x, corner2.y, corner2.z)),
                new FixLine(corner2, new FixVector(corner2.x, -corner2.y, corner2.z)),
                new FixLine(corner2, new FixVector(corner2.x, corner2.y, -corner2.z)),
                new FixLine(corner3, new FixVector(-corner3.x, corner3.y, corner3.z)),
                new FixLine(corner3, new FixVector(corner3.x, -corner3.y, corner3.z)),
                new FixLine(corner3, new FixVector(corner3.x, corner3.y, -corner3.z)),
                new FixLine(corner4, new FixVector(-corner4.x, corner4.y, corner4.z)),
                new FixLine(corner4, new FixVector(corner4.x, -corner4.y, corner4.z)),
                new FixLine(corner4, new FixVector(corner4.x, corner4.y, -corner4.z)),
            };

            return lines;
        }

        /// <summary>
        /// 获取射线交点
        /// </summary>
        /// <param name="origin"> 射线原点 </param>
        /// <param name="direction"> 射线方向向量 </param>
        /// <param name="closeHitPoint"> 相交近点 </param>
        /// <param name="farHitPoint"> 相交远点 </param>
        /// <returns></returns>
        public bool TryGetRayHitPoint(FixVector origin, FixVector direction, out FixVector closeHitPoint, out FixVector farHitPoint)
        {
            farHitPoint = default;
            closeHitPoint = default;
            return false;
        }
        
        /// <summary>
        /// 线段与平面的交点是否在Box上
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="plane"></param>
        /// <param name="box"></param>
        /// <param name="hitPoint"></param>
        /// <returns></returns>
        public static bool SegmentToBoxPlaneTest(FixVector origin, FixVector target, FixPlane plane, FixBox box, out FixVector hitPoint)
        {
            var rayDirection = (target - origin).normalized;
            if (!FixPlane.TryGetRayHitPlanePoint(origin, rayDirection, plane, out hitPoint))
            {
                return false;
            }

            if (!box.Contains(hitPoint))
            {
                return false;
            }

            if (!FixLine.IsPointOnSegment(origin, target, hitPoint))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private FixVector InternalGetOriginPoint(FixVector point)
        {
            point -= Center;
            FixMatrix.Invert(ref _orientation, out var invRotMatrix);
            
            return FixVector.TransformCoordinate(point, invRotMatrix);
        }

        private FixVector InternalGetCoordinatePoint(FixVector point)
        {
            FixVector.TransformCoordinate(ref point, ref _orientation, out var transPos);
            FixVector.Add(ref transPos, ref _center, out var result);
            return result;
        }
        
        private void InternalGetCoordinatePoint(ref FixVector point)
        {
            FixVector.TransformCoordinate(ref point, ref _orientation, out var transPos);
            FixVector.Add(ref transPos, ref _center, out point);
        }

        public void SupportMapping(ref FixVector direction, out FixVector result)
        {
            result.x = Fix64.Sign(direction.x) * _halfSize.x;
            result.y = Fix64.Sign(direction.y) * _halfSize.y;
            result.z = Fix64.Sign(direction.z) * _halfSize.z;
        }

        public void SupportCenter(out FixVector center)
        {
            center = Center;
        }
    }
}