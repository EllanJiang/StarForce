/*
* 文件名：FPCylinder
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 15:53:37
* 修改记录：
*/

using System;
using LogicShared.TrueSync.Math;

namespace LogicShared.TrueSync.Physics
{
    [Serializable]
    public struct FixCylinder
    {
        private Fix64 _radius;
        private Fix64 _height;
        private FixVector _offset;            
        private FixVector _parentPosition;

        public FixCylinder(FixVector parentPosition, FixVector offset, Fix64 radius, Fix64 height)
        {
            _parentPosition = parentPosition;
            _offset = offset;
            _radius = radius;
            _height = height;
        }
		
        public Fix64 Radius
        {
            get => _radius;
            set => _radius = value;
        }
		
        public Fix64 Height
        {
            get => _height;
            set => _height = value;
        }
		
        public FixVector Offset
        {
            get => _offset;
            set => _offset = value;
        }
		
        public FixVector ParentPosition
        {
            get => _parentPosition;
            set => _parentPosition = value;
        }
		
        public FixVector Center => _offset + _parentPosition;

        public FixVector UpCenter => Center + FixVector.up * _height * Fix64.Half;

        public FixVector DownCenter => Center + FixVector.down * _height * Fix64.Half;
    }
}