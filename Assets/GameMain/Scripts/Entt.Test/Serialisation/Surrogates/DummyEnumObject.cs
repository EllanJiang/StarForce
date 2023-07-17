/*
  作者：LTH
  文件描述：
  文件名：DummyEnumObject
  创建时间：2023/07/17 22:07:SS
*/

using System.Runtime.Serialization;

namespace Entt.Test.Serialisation.Surrogates
{
    [DataContract]
    public class DummyEnumObject
    {
        public static readonly DummyEnumObject OptionA = new DummyEnumObject("A");
        public static readonly DummyEnumObject OptionB = new DummyEnumObject("B");

        DummyEnumObject(string id)
        {
            this.Id = id;
        }

        [DataMember] 
        public string Id { get; }
    }
}