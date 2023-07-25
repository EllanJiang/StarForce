/*
* 文件名：Collisions
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:03:40
* 修改记录：
*/

namespace LiteNetLib.Test.Shared
{
    public static class Collisions
    {
        /// <summary>
        /// 判断是否相交
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool CheckIntersection(float x1, float y1, float x2, float y2, BasePlayer player)
        {
            float cx = player.Position.x;
            float cy = player.Position.y;
            float distX = x2-x1;
            float distY = y2-y1;
            float lineLenSqr = distX * distX + distY * distY;
            float dot = ( (cx-x1)*distX + (cy-y1)*distY ) / lineLenSqr;
            float closestX = x1 + dot * distX;
            float closestY = y1 + dot * distY;

            float dcx1 = closestX - x1;
            float dcy1 = closestY - y1;
            float dcx2 = closestX - x2;
            float dcy2 = closestY - y2;
            float distToLineSqr1 = dcx1 * dcx1 + dcy1 * dcy1;
            float distToLineSqr2 = dcx2 * dcx2 + dcy2 * dcy2;
            if (distToLineSqr1 > lineLenSqr || distToLineSqr2 > lineLenSqr)
                return false;
            
            distX = closestX - cx;
            distY = closestY - cy;
            return distX*distX + distY*distY <= BasePlayer.Radius * BasePlayer.Radius;
        }
    }
}