/*
  作者：LTH
  文件描述：
  文件名：DummyEnumDataContainer
  创建时间：2023/07/17 22:07:SS
*/

using System.Runtime.Serialization;

namespace Entt.Test.Serialisation.Surrogates
{
    [DataContract]
    public readonly struct DummyEnumDataContainer
    {
        [DataMember] 
        public readonly DummyEnumObject data;

        public DummyEnumDataContainer(DummyEnumObject data)
        {
            this.data = data;
        }
    }
}