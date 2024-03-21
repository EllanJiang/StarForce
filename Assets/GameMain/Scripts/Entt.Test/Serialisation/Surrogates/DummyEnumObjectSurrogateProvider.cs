/*
  作者：LTH
  文件描述：
  文件名：DummyEnumObjectSurrogateProvider
  创建时间：2023/07/17 22:07:SS
*/

using System;
using Entt.Serialization.Xml;

namespace Entt.Test.Serialisation.Surrogates
{
    public class DummyEnumObjectSurrogateProvider: SerializationSurrogateProviderBase<DummyEnumObject, SurrogateContainer<string>>
    {
        public override DummyEnumObject GetDeserializedObject(SurrogateContainer<string> surrogate)
        {
            if (surrogate.Content == "A") return DummyEnumObject.OptionA;
            if (surrogate.Content == "B") return DummyEnumObject.OptionB;
            throw new ArgumentException();
        }

        public override SurrogateContainer<string> GetObjectToSerialize(DummyEnumObject obj)
        {
            return new SurrogateContainer<string>(obj.Id);
        }
    }
}