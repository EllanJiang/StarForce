/*
* 文件名：Components
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/28 19:47:19
* 修改记录：
*/

namespace Entt.Test
{
    //位置组件
    public readonly struct Position
    {
        public readonly float X;
        public readonly float Y;

        public Position(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
    
    //速度组件
    public readonly struct Velocity
    {
        public readonly float DeltaX;
        public readonly float DeltaY;

        public Velocity(float deltaX, float deltaY)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
        }
    }
}