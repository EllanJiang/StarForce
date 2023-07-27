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
        
        public TSVector Min;
        public TSVector Max;

        //相对父节点的偏移量
        private TSVector _offsetFromParent;
        private TSVector _parentPosition;

        #region 属性

        public TSVector Size
        {
            get { return this.Max - this.Min; }
        }

        public TSVector HalfSize
        {
            get { return Size / 2; }
        }

        public TSVector Center
        {
            get { return (Max + Min) / 2; }
        }

        public FixBox Box
        {
            get
            {
                return new FixBox(Center, TSVector.zero, Size, TSVector.zero);
            }
        }

        public FP Depth
        {
            get { return Max.z - Min.z; }
        }
        
        public FP height
        {
            get { return Max.y - Min.y; }
        }
        
        public FP Width
        {
            get { return Max.x - Min.x; }
        }
        
        public TSVector ParentPosition
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
        
        public TSVector OffsetFromParent
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

        public FixAABB(TSVector min, TSVector max)
        {
            Min = min;
            Max = max;
            _offsetFromParent = (min + max) /2;
            _parentPosition = TSVector.zero;
        }

        public FixAABB(TSVector parentPosition, TSVector offsetFromParent, TSVector halfSize)
        {
            _parentPosition = parentPosition;
            _offsetFromParent = offsetFromParent;
            var center = parentPosition + offsetFromParent;
            Min = center - halfSize;
            Max = center + halfSize;
        }
        
        public FixAABB(TSVector parentPosition, TSVector offsetFromParent, TSVector min,TSVector max)
        {
            _parentPosition = parentPosition;
            _offsetFromParent = offsetFromParent;
            Min = min;
            Max = max;
        }

        static FixAABB()
        {
            zero = new FixAABB(new TSVector(0, 0, 0), new TSVector(0, 0, 0));
        }
        
        public void Refresh()
        {
            var center = _parentPosition + _offsetFromParent;
            var min = center - Size / 2;
            var max = center + Size / 2;
            Min = min;
            Max = max;
        }

        public void Set(TSVector min, TSVector max)
        {
            Min = min;
            Max = max;
        }

        public bool Contains(TSVector position)
        {
            var ret = position.x >= Min.x && position.x <= Max.x &&
                      position.y >= Min.y && position.y <= Max.y &&
                      position.z >= Min.z && position.z <= Max.z;
            return ret;
        }

        //AABB上到目标点的最近点
        public TSVector ClosedPoint(TSVector point)
        {
            return Contains(point) ? InsideClosedPoint(point) : OutsideClosedPoint(point);
        }

        public TSVector OutsideClosedPoint(TSVector point)
        {
            if (Contains(point))
            {
                return point;
            }

            var ret = point;
            ret.x = TSMath.Clamp(ret.x, Min.x, Max.x);
            ret.y = TSMath.Clamp(ret.y, Min.y, Max.y);
            ret.z = TSMath.Clamp(ret.z, Min.z, Max.z);
            return ret;
        }
        
        public TSVector InsideClosedPoint(TSVector point)
        {
            if (!Contains(point))
            {
                return point;
            }

            var minOffsetX = TSMath.Abs(Min.x - point.x);
            var maxOffsetX = TSMath.Abs(Max.x - point.x);
            var minOffsetY = TSMath.Abs(Min.y - point.y);
            var maxOffsetY = TSMath.Abs(Max.y - point.y);
            var minOffsetZ = TSMath.Abs(Min.z - point.z);
            var maxOffsetZ = TSMath.Abs(Max.z - point.z);
            
            var x = minOffsetX < maxOffsetX ? Min.x : Max.x;
            var y = minOffsetY < maxOffsetY ? Min.y : Max.y;
            var z = minOffsetZ < maxOffsetZ ? Min.z : Max.z;
            var minX = TSMath.Min(minOffsetX, maxOffsetX);
            var minY = TSMath.Min(minOffsetY, maxOffsetY);
            var minZ = TSMath.Min(minOffsetZ, maxOffsetZ);
            
            return new TSVector(
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
                var result = new FixAABB(TSVector.zero, TSVector.zero);
                result.Min = new TSVector()
                {
                    x = TSMath.Max(Min.x, aabb.Min.x),
                    y = TSMath.Max(Min.y, aabb.Min.y),
                    z = TSMath.Max(Min.z, aabb.Min.z)
                };
                result.Max = new TSVector()
                {
                    x = TSMath.Min(Max.x, aabb.Max.x),
                    y = TSMath.Min(Max.y, aabb.Max.y),
                    z = TSMath.Min(Max.z, aabb.Max.z)
                };

                return result;
            }

            return zero;
        }

        //获取偏移后的AABB
        public FixAABB OffsetAABB(TSVector offset)
        {
            return new FixAABB(Min + offset, Max + offset);
        }

        //变换到世界坐标系
        public FixAABB ToWorldSpace(TSVector offset, bool mirror)
        {
            FixAABB ret = new FixAABB(Min + offset, Max + offset);
            if (mirror)
            {
                ret.Mirror(offset.x);
            }

            return ret;
        }

        public void Offset(TSVector offset)
        {
            Min += offset;
            Max += offset;
        }

        //X轴反向
        public void Mirror(FP x)
        {
            FP temp = Min.x;
            Min.x = x + x - Max.x;
            Max.x = x + x - temp;
        }
        
        //拓展AABB（并集）
        public void Expand(FixAABB aabb)
        {
            Min.x = TSMath.Min(Min.x, aabb.Min.x);
            Min.y = TSMath.Min(Min.y, aabb.Min.y);
            Min.z = TSMath.Min(Min.z, aabb.Min.z);

            Max.x = TSMath.Max(Max.x, aabb.Max.x);
            Max.y = TSMath.Max(Max.y, aabb.Max.y);
            Max.z = TSMath.Max(Max.z, aabb.Max.z);
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