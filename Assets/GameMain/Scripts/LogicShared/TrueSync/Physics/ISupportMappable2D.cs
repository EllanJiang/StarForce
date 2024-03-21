/*
* 文件名：ISupportMappable2D
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 15:45:32
* 修改记录：
*/

using LogicShared.TrueSync.Math;

namespace LogicShared.TrueSync.Physics
{
    public interface ISupportMappable2D
    {
        FixVector2 Center
        {
            get;
        }

        FixMatrix2x2 Orientation
        {
            get;
        }
		
        /// <summary>
        /// 凸包指定方向最远点
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="result"></param>
        void SupportMapping(ref FixVector2 direction, out FixVector2 result);
    }
}