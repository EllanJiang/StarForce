/*
* 文件名：ISupportMappable3D
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 15:59:52
* 修改记录：
*/

using LogicShared.TrueSync.Math;

namespace LogicShared.TrueSync.Physics
{
    /// <summary>
    /// 支持Support函数形状接口
    /// </summary>
    public interface ISupportMappable3D
    {
        /// <summary>
        /// 获取AABB包围盒
        /// </summary>
        FixAABB AABB
        {
            get;
        }
		
        /// <summary>
        /// 获取预碰撞AABB包围盒
        /// </summary>
        FixAABB PreCollideAABB
        {
            get;
        }

        /// <summary>
        /// 获取或设置父节点位置
        /// </summary>
        FixVector ParentPosition
        {
            get;
            set;
        }

        FixVector Center
        {
            get;
        }
		
        /// <summary>
        /// 获取朝向
        /// </summary>
        FixMatrix Orientation
        {
            get;
        }

        /// <summary>
        /// XZ轴方向2D投影形状
        /// </summary>
        ISupportMappable2D Support2D
        {
            get;
        }
		
        /// <summary>
        /// 凸包指定方向最远点
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="result"></param>
        void SupportMapping(ref FixVector direction, out FixVector result);

        /// <summary>
        /// 凸包中心点
        /// </summary>
        /// <param name="center"></param>
        void SupportCenter(out FixVector center);
    }
}