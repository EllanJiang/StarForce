/*
* 文件名：FIxBB
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/27 19:47:53
* 修改记录：
*/

using System;
using LogicShared.TrueSync.Math;

namespace LogicShared.TrueSync.Physics
{
    /// <summary>
    /// 定点数版本的轴对齐包围盒(axis aligned bounding box)
    /// </summary>
    [Serializable]
    public struct FixAABB
    {
        public static readonly FixAABB zero;
        
        public FixVector Min;
        public FixVector Max;

        //相对父节点的偏移量
        private FixVector _offsetFromParent;
        private FixVector _parentPosition;

        #region 属性

        public FixVector Size
        {
            get { return this.Max - this.Min; }
        }

        public FixVector HalfSize
        {
            get { return Size / 2; }
        }

        public FixVector Center
        {
            get { return (Max + Min) / 2; }
        }

        public FixBox Box
        {
            get
            {
                return new FixBox(Center, FixVector.zero, Size, FixVector.zero);
            }
        }

        public Fix64 Depth
        {
            get { return Max.z - Min.z; }
        }
        
        public Fix64 height
        {
            get { return Max.y - Min.y; }
        }
        
        public Fix64 Width
        {
            get { return Max.x - Min.x; }
        }
        
        public FixVector ParentPosition
        {
            get
            {
                return _parentPosition;
            }
            set
            {
                _parentPosition = value;
                Refresh();
            }
        }
        
        public FixVector OffsetFromParent
        {
            get
            {
                return _offsetFromParent;
            }
            set
            {
                _offsetFromParent = value;
                Refresh();
            }
        }
        
        #endregion

        public FixAABB(FixVector min, FixVector max)
        {
            Min = min;
            Max = max;
            _offsetFromParent = (min + max) /2;
            _parentPosition = FixVector.zero;
        }

        public FixAABB(FixVector parentPosition, FixVector offsetFromParent, FixVector halfSize)
        {
            _parentPosition = parentPosition;
            _offsetFromParent = offsetFromParent;
            var center = parentPosition + offsetFromParent;
            Min = center - halfSize;
            Max = center + halfSize;
        }
        
        public FixAABB(FixVector parentPosition, FixVector offsetFromParent, FixVector min,FixVector max)
        {
            _parentPosition = parentPosition;
            _offsetFromParent = offsetFromParent;
            Min = min;
            Max = max;
        }

        static FixAABB()
        {
            zero = new FixAABB(new FixVector(0, 0, 0), new FixVector(0, 0, 0));
        }
        
        public void Refresh()
        {
            var center = _parentPosition + _offsetFromParent;
            var min = center - Size / 2;
            var max = center + Size / 2;
            Min = min;
            Max = max;
        }

        public void Set(FixVector min, FixVector max)
        {
            Min = min;
            Max = max;
        }

        public bool Contains(FixVector position)
        {
            var ret = position.x >= Min.x && position.x <= Max.x &&
                      position.y >= Min.y && position.y <= Max.y &&
                      position.z >= Min.z && position.z <= Max.z;
            return ret;
        }

        //AABB上到目标点的最近点
        public FixVector ClosedPoint(FixVector point)
        {
            return Contains(point) ? InsideClosedPoint(point) : OutsideClosedPoint(point);
        }

        public FixVector OutsideClosedPoint(FixVector point)
        {
            if (Contains(point))
            {
                return point;
            }

            var ret = point;
            ret.x = FixMath.Clamp(ret.x, Min.x, Max.x);
            ret.y = FixMath.Clamp(ret.y, Min.y, Max.y);
            ret.z = FixMath.Clamp(ret.z, Min.z, Max.z);
            return ret;
        }
        
        public FixVector InsideClosedPoint(FixVector point)
        {
            if (!Contains(point))
            {
                return point;
            }

            var minOffsetX = FixMath.Abs(Min.x - point.x);
            var maxOffsetX = FixMath.Abs(Max.x - point.x);
            var minOffsetY = FixMath.Abs(Min.y - point.y);
            var maxOffsetY = FixMath.Abs(Max.y - point.y);
            var minOffsetZ = FixMath.Abs(Min.z - point.z);
            var maxOffsetZ = FixMath.Abs(Max.z - point.z);
            
            var x = minOffsetX < maxOffsetX ? Min.x : Max.x;
            var y = minOffsetY < maxOffsetY ? Min.y : Max.y;
            var z = minOffsetZ < maxOffsetZ ? Min.z : Max.z;
            var minX = FixMath.Min(minOffsetX, maxOffsetX);
            var minY = FixMath.Min(minOffsetY, maxOffsetY);
            var minZ = FixMath.Min(minOffsetZ, maxOffsetZ);
            
            return new FixVector(
                minX < minY && minX < minZ ? x : point.x
                , minY < minX && minY < minZ ? y : point.y
                , minZ < minY && minZ < minX ? z : point.z);
        }

        public bool Intersect(FixAABB aabb)
        {
            //空的AABB无法相交
            if (Min == Max || aabb.Min == aabb.Max)
            {
                return false;
            }
            if (Max.x <= aabb.Min.x || aabb.Max.x <= Min.x)
                return false;
            if (Max.y <= aabb.Min.y || aabb.Max.y <= Min.y)
                return false;
            if (Max.z <= aabb.Min.z || aabb.Max.z <= Min.z)
                return false;
            return true;
        }

        //获取交集
        public FixAABB Intersection(FixAABB aabb)
        {
            if(Intersect(aabb))
            {
                var result = new FixAABB(FixVector.zero, FixVector.zero);
                result.Min = new FixVector()
                {
                    x = FixMath.Max(Min.x, aabb.Min.x),
                    y = FixMath.Max(Min.y, aabb.Min.y),
                    z = FixMath.Max(Min.z, aabb.Min.z)
                };
                result.Max = new FixVector()
                {
                    x = FixMath.Min(Max.x, aabb.Max.x),
                    y = FixMath.Min(Max.y, aabb.Max.y),
                    z = FixMath.Min(Max.z, aabb.Max.z)
                };

                return result;
            }

            return zero;
        }

        //获取偏移后的AABB
        public FixAABB OffsetAABB(FixVector offset)
        {
            return new FixAABB(Min + offset, Max + offset);
        }

        //变换到世界坐标系
        public FixAABB ToWorldSpace(FixVector offset, bool mirror)
        {
            FixAABB ret = new FixAABB(Min + offset, Max + offset);
            if (mirror)
            {
                ret.Mirror(offset.x);
            }

            return ret;
        }

        public void Offset(FixVector offset)
        {
            Min += offset;
            Max += offset;
        }

        //X轴反向
        public void Mirror(Fix64 x)
        {
            Fix64 temp = Min.x;
            Min.x = x + x - Max.x;
            Max.x = x + x - temp;
        }
        
        //拓展AABB（并集）
        public void Expand(FixAABB aabb)
        {
            Min.x = FixMath.Min(Min.x, aabb.Min.x);
            Min.y = FixMath.Min(Min.y, aabb.Min.y);
            Min.z = FixMath.Min(Min.z, aabb.Min.z);

            Max.x = FixMath.Max(Max.x, aabb.Max.x);
            Max.y = FixMath.Max(Max.y, aabb.Max.y);
            Max.z = FixMath.Max(Max.z, aabb.Max.z);
        }

        public static bool operator ==(FixAABB value1, FixAABB value2)
        {
            return (((value1.Min == value2.Min) && (value1.Max == value2.Max)));
        }

        public static bool operator !=(FixAABB value1, FixAABB value2)
        {
            return (((value1.Min != value2.Min) || (value1.Max != value2.Max)));
        }
    }
}