/*
* 文件名：FPOBB
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 15:57:09
* 修改记录：
*/

using System;
using LogicShared.TrueSync.Math;

namespace LogicShared.TrueSync.Physics
{
    /// <summary>
    /// 有向包围盒
    /// </summary>
    [Serializable]
    public struct FixOBB
    {
        private FixVector _size;              // 盒子大小
        private FixVector _offset;            // 距离父节点世界坐标偏移
        private FixVector _parentPosition;    // 父节点世界坐标（默认为原点）
        private FixMatrix _rotateMatrix;       // 旋转矩阵

        /// <summary>
        /// AABB盒对角创建OBB盒
        /// </summary>
        /// <param name="minimum"> 最小顶点 </param>
        /// <param name="maximum"> 最大顶点 </param>
        public FixOBB(FixVector minimum, FixVector maximum)
        {
            _parentPosition = FixVector.zero;
            _offset = minimum + (maximum - minimum) / 2f;
            _size = maximum - _offset;
            _rotateMatrix = FixMatrix.Identity;
        }

        public FixOBB(FixVector parentPosition, FixVector offset, FixVector size, FixVector euler)
        {
            _parentPosition = parentPosition;
            _offset = offset;
            _size = size;
            _rotateMatrix = FixMatrix.Rotate(euler);
        }

        /// <summary>
        /// 获取碰撞盒中心点世界坐标
        /// </summary>
        public FixVector Center => _offset + _parentPosition;

        /// <summary>
        /// 获取旋转矩阵
        /// </summary>
        public FixMatrix RotateMatrix => _rotateMatrix;

        /// <summary>
        /// 获取或设置盒子大小
        /// </summary>
        public FixVector Size
        {
            get => _size;
            set => _size = value;
        }
        
        /// <summary>
        /// 获取或设置距离父节点世界坐标偏移
        /// </summary>
        public FixVector Offset
        {
            get => _offset;
            set => _offset = value;
        }
        
        /// <summary>
        /// 获取或设置父节点世界坐标
        /// </summary>
        public FixVector ParentPosition
        {
            get => _parentPosition;
            set => _parentPosition = value;
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="translation"></param>
        public void Move(FixVector translation)
        {
            _offset += translation;
        }
        
        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="rotMat"> 左乘行向量旋转矩阵 </param>
        public void Rotate(FixMatrix rotMat)
        {
            _rotateMatrix *= rotMat;
        }        

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="scaling"></param>
        public void Scale(FixVector scaling)
        {
            _size.x *= scaling.x;
            _size.y *= scaling.y;
            _size.z *= scaling.z;
        }

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="scaling"></param>
        public void Scale(Fix64 scaling)
        {
            _size *= scaling;
        }

        /// <summary>
        /// 点是否在OBB盒内
        /// </summary>
        /// <param name="point"> 点 </param>
        /// <returns></returns>
        public bool Contains(FixVector point)
        {
            var originPoint = InternalGetOriginPoint(point);

            originPoint.x = FixMath.Abs(originPoint.x);
            originPoint.y = FixMath.Abs(originPoint.y);
            originPoint.z = FixMath.Abs(originPoint.z);

            if (originPoint.x <= _size.x && originPoint.y <= _size.y && originPoint.z <= _size.z)
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 获取OBB盒所有顶点
        /// </summary>
        /// <returns></returns>
        public FixVector[] GetCorners()
        {
            var corners = new FixVector[8];
            var x = _size.x;
            var y = _size.y;
            var z = _size.z;
            corners[0].Set(x, y, z);
            corners[1].Set(x, y, -z);
            corners[2].Set(-x, y, -z);
            corners[3].Set(-x, y, z);
            corners[4].Set(x, -y, z);
            corners[5].Set(x, -y, -z);
            corners[6].Set(-x, -y, -z);
            corners[7].Set(-x, -y, z);

            FixVector.TransformCoordinate(ref corners[0], ref _rotateMatrix, out corners[0]);
            FixVector.TransformCoordinate(ref corners[1], ref _rotateMatrix, out corners[1]);
            FixVector.TransformCoordinate(ref corners[2], ref _rotateMatrix, out corners[2]);
            FixVector.TransformCoordinate(ref corners[3], ref _rotateMatrix, out corners[3]);
            FixVector.TransformCoordinate(ref corners[4], ref _rotateMatrix, out corners[4]);
            FixVector.TransformCoordinate(ref corners[5], ref _rotateMatrix, out corners[5]);
            FixVector.TransformCoordinate(ref corners[6], ref _rotateMatrix, out corners[6]);
            FixVector.TransformCoordinate(ref corners[7], ref _rotateMatrix, out corners[7]);

            for (var i = 0; i < corners.Length; i++)
            {
                corners[i] += _offset;
            }          
            
            return corners;
        }
        
        /// <summary>
        /// 获取OBB盒上距离点最近的点
        /// </summary>
        /// <param name="point"> 位置 </param>
        /// <returns></returns>
        public FixVector ClosedPoint(FixVector point)
        {
            return Contains(point) ? InsideClosedPoint(point) : OutsideClosedPoint(point);
        }

        /// <summary>
        /// OBB盒外获取距离最近的点
        /// </summary>
        /// <param name="point"> 位置 </param>
        /// <returns></returns>
        public FixVector OutsideClosedPoint(FixVector point)
        {
            if (Contains(point))
            {
                return point;
            }
            
            var originPoint = InternalGetOriginPoint(point);
            originPoint.x = FixMath.Clamp(originPoint.x, -_size.x, _size.x);
            originPoint.y = FixMath.Clamp(originPoint.y, -_size.y, _size.y);
            originPoint.z = FixMath.Clamp(originPoint.z, -_size.z, _size.z);
            
            return InternalGetCoordinatePoint(originPoint);
        }
        
        /// <summary>
        /// OBB盒内获取距离盒子最近的点
        /// </summary>
        /// <param name="point"> 位置 </param>
        /// <returns></returns>
        public FixVector InsideClosedPoint(FixVector point)
        {
            if (!Contains(point))
            {
                return point;
            }
            
            var originPoint = InternalGetOriginPoint(point);

            var minOffsetX = FixMath.Abs(-_size.x - originPoint.x);
            var maxOffsetX = FixMath.Abs(_size.x - originPoint.x);
            var minOffsetY = FixMath.Abs(-_size.y - originPoint.y);
            var maxOffsetY = FixMath.Abs(_size.y - originPoint.y);
            var minOffsetZ = FixMath.Abs(-_size.z - originPoint.z);
            var maxOffsetZ = FixMath.Abs(_size.z - originPoint.z);
            var x = minOffsetX < maxOffsetX ? -_size.x : _size.x;
            var y = minOffsetY < maxOffsetY ? -_size.y : _size.y;
            var z = minOffsetZ < maxOffsetZ ? -_size.z : _size.z;
            var minX = FixMath.Min(minOffsetX, maxOffsetX);
            var minY = FixMath.Min(minOffsetY, maxOffsetY);
            var minZ = FixMath.Min(minOffsetZ, maxOffsetZ);

            originPoint.Set(
                minX < minY && minX < minZ ? x : originPoint.x
                , minY < minX && minY < minZ ? y : originPoint.y
                , minZ < minY && minZ < minX ? z : originPoint.z
            );
            
            return InternalGetCoordinatePoint(originPoint);
        }

        private FixVector InternalGetOriginPoint(FixVector point)
        {
            point -= Center;
            FixMatrix.Invert(ref _rotateMatrix, out var invRotMatrix);
            FixVector.TransformCoordinate(ref point, ref invRotMatrix, out var tranRotPoint);
            
            return tranRotPoint;
        }

        private FixVector InternalGetCoordinatePoint(FixVector point)
        {
            FixVector.TransformCoordinate(ref point, ref _rotateMatrix, out var rotPoint);
            
            return rotPoint + Center;
        }
    }
}