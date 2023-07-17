/*
  作者：LTH
  文件描述：
  文件名：DummyEnumResolverTest
  创建时间：2023/07/17 22:07:SS
*/

using System;
using System.IO;
using System.Text;
using System.Xml;
using Entt.Serialization.Xml;
using Entt.Serialization.Xml.Impl;
using FluentAssertions;
using UnityEngine;

namespace Entt.Test.Serialisation.Surrogates
{
    public class DummyEnumResolverTest:MonoBehaviour
    {
        private void Start()
        {
            TestDataContractResolving();
        }

        public void TestDataContractResolving()
        {
            var model = new DummyEnumDataContainer(DummyEnumObject.OptionA);
            var surrogate = new ObjectSurrogateResolver();
            surrogate.Register(typeof(DummyEnumObject), new DummyEnumObjectSurrogateProvider());

            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb,
                new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "  "
                });


            var dc = new DefaultDataContractWriteHandler<DummyEnumDataContainer>(surrogate);
            dc.Write(xmlWriter, model);
            xmlWriter.Flush();

            Debug.Log("----------------------");
            Debug.Log(sb.ToString());
            Debug.Log("----------------------");

            var xmlReader = XmlReader.Create(new StringReader(sb.ToString()));
            var dr = new DefaultDataContractReadHandler<DummyEnumDataContainer>(surrogate);
            var loaded = dr.Read(xmlReader);
            loaded.data.Should().BeSameAs(DummyEnumObject.OptionA);
        }
    }
}