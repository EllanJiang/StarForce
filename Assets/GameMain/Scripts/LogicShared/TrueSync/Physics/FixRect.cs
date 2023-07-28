/*
* 文件名：FPRect
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 15:58:56
* 修改记录：
*/

using LogicShared.TrueSync.Math;

namespace LogicShared.TrueSync.Physics
{
    /// <summary>
    /// 矩形
    /// </summary>
    public struct FixRect
    {
        private Fix64 _xMin;
        private Fix64 _yMin;
        private Fix64 _width;
        private Fix64 _height;

        public static FixRect zero =>new FixRect(0,0,0,0);

        public Fix64 x
        {
            get => _xMin;
            set => _xMin = value;
        }

        public Fix64 y
        {
            get => _yMin;
            set => _yMin = value;
        }
        /// <summary>
        /// The X and Y position of the rectangle.
        /// </summary>
        public FixVector2 position
        {
            get => new FixVector2(_xMin, _yMin);
            set
            {
                _xMin = value.x;
                _yMin = value.y;
            }
        }

        public FixVector2 center
        {
            get => new FixVector2(x + _width / 2f, y + _height / 2f);
            set
            {
                _xMin = value.x - _width / 2f;
                _yMin = value.y - _height / 2f;
            }
        }
        public FixVector2 size
        {
            get => new FixVector2(_width, _height);
            set
            {
                _width = value.x;
                _height = value.y;
            }
        }
        public Fix64 xMax
        {
            get => _width + _xMin;
            set => _width = value - _xMin;
        }
        public Fix64 xMin
        {
            get => _xMin;
            set
            {
                Fix64 temp = xMax;
                _xMin = value;
                _width = temp - _xMin;
            }
        }
        public Fix64 width
        {
            get => _width;
            set => _width = value;
        }

        public Fix64 height
        {
            get => _height;
            set => _height = value;
        }

        public Fix64 yMax
        {
            get => _height + _yMin;
            set => _height = value - _yMin;
        }
        
        public Fix64 yMin
        {
            get => _yMin;
            set
            {
                Fix64 temp = yMax;
                _yMin = value;
                _height = temp - _yMin;
            }
        }
        
        public FixRect(Fix64 x, Fix64 y, Fix64 width, Fix64 height)
        {
            _xMin = x;
            _yMin = y;
            _width = width;
            _height = height;
        }

        public FixRect(FixVector2 position, FixVector2 size)
        {
            _xMin = position.x;
            _yMin = position.y;
            _width = size.x;
            _height = size.y;
        }
        public FixRect(FixRect source)
        {
            _xMin = source._xMin;
            _yMin = source._yMin;
            _width = source._width;
            _height = source._height;
        }

        public void Set(Fix64 x, Fix64 y, Fix64 width, Fix64 height)
        {
            _xMin = x;
            _yMin = y;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// 矩形内是否包含改点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(FixVector2 point)
        {
            return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax;
        }

        /// <summary>
        /// 矩形内是否包含改点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(FixVector point)
        {
            return point.x >= xMin && point.x < xMax && point.z >= yMin && point.z < yMax;
        }

        /// <summary>
        /// 矩形是否重叠
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Overlaps(FixRect other)
        {
            return other.xMax > xMin && other.xMin < xMax && other.yMax > yMin && other.yMin < yMax;
        }

        public static bool operator !=(FixRect lhs, FixRect rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(FixRect lhs, FixRect rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
        }
        public override bool Equals(object other)
        {
            if (!(other is FixRect))
            {
                return false;
            }

            return Equals((FixRect)other);
        }

        public bool Equals(FixRect other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && width.Equals(other.width) && height.Equals(other.height);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (width.GetHashCode() << 2) ^ (y.GetHashCode() >> 2) ^ (height.GetHashCode() >> 1);
        }
    }
}