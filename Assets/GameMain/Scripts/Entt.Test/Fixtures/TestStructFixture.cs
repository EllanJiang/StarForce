/*
* 文件名：TestStructFixture
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/03 20:14:15
* 修改记录：
*/

using System;
using System.Runtime.Serialization;
using Entt.Entities.Attributes;

namespace Entt.Test.Fixtures
{
    [Serializable]
    [DataContract]
    [EntityComponent]
    public readonly struct TestStructFixture
    {
        [DataMember]
        public readonly int x;
        [DataMember]
        public readonly int y;

        public TestStructFixture(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [EntityComponent]
    public readonly struct Velocity
    {
        public readonly int X;
        public readonly int Y;
    }
}