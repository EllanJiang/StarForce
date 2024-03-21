/*
* 文件名：FPSphere
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 15:59:24
* 修改记录：
*/

using System;
using LogicShared.TrueSync.Math;

namespace LogicShared.TrueSync.Physics
{
    /// <summary>
    /// 球
    /// </summary>
    [Serializable]
    public struct FixSphere
    {
        private Fix64 _radius;				// 半径
        private FixVector _offset;			// 距离父节点世界坐标偏移
        private FixVector _parentPosition;	// 距离父节点世界坐标（默认为原点）

        public FixSphere(Fix64 radius)
        {
            _radius = radius;
            _offset = FixVector.zero;
            _parentPosition = FixVector.zero;
        }
		
        public FixSphere(Fix64 radius, FixVector offset)
        {
            _radius = radius;
            _offset = offset;
            _parentPosition = FixVector.zero;
        }
		
        public FixSphere(Fix64 radius, FixVector offset, FixVector parentPosition)
        {
            _radius = radius;
            _offset = offset;
            _parentPosition = parentPosition;
        }

        public FixVector Offset => _offset;
        public FixVector Center => _offset + _parentPosition;

        public Fix64 Radius
        {
            get => _radius;
            set => _radius = value;
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
        /// 改点是否在球内
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(FixVector point)
        {
            return (point - Center).magnitude <= _radius;
        }
		
        /// <summary>
        /// 球面上距离改点最近的点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public FixVector ClosedPoint(FixVector point)
        {
            var direction = (point - Center).normalized;
			
            return direction * _radius;
        }
    }
}