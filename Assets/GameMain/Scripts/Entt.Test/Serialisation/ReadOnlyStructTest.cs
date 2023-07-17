/*
* 文件名：ReadOnlyStructTest
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/17 10:26:14
* 修改记录：
*/

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Entt.Serialization.Xml.Impl;
using FluentAssertions;
using MessagePack;
using UnityEngine;

namespace Entt.Test.Serialisation
{
    /// <summary>
    /// 测试只读结构体的序列化和反序列化
    /// </summary>
    public class ReadOnlyStructTest:MonoBehaviour
    {
        [MessagePackObject]
        [DataContract]
        public readonly struct Model
        {
            [Key(0)]
            [DataMember]
            public readonly int Number;
            [Key(1)]
            [DataMember]
            public readonly string Text;

            public Model(int number, string text)
            {
                Number = number;
                Text = text;
            }
        }

        private void Start()
        {
            TestMessagePack();
            TestDataContractXml();
        }

        //测试MessagePack序列化和反序列化
        public void TestMessagePack()
        {
            var model = new Model(10, "Test");
            //序列化
            var serializeData = MessagePackSerializer.Serialize(model);
            serializeData.Length.Should().Be(7);
            
            //反序列化
            var deserializeData = MessagePackSerializer.Deserialize<Model>(serializeData);
            deserializeData.Number.Should().Be(10);
            deserializeData.Text.Should().Be("Test");
        }

        //测试xml序列化和反序列化
        public void TestDataContractXml()
        {
            var model = new Model(11, "Test1");
            //写入xml文件
            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb, new XmlWriterSettings()
                                                {
                                                    Indent = true,      //要求缩进
                                                    IndentChars = "  "  //缩进符号：两个空格
                                                });
            var dc = new DefaultDataContractWriteHandler<Model>();
            dc.Write(xmlWriter,model);      //model写入xml
            xmlWriter.Flush();
            
            //
            Debug.Log("xml内容--------------------------Start");
            Debug.Log(sb.ToString());
            Debug.Log("xml内容--------------------------End");
            
            //读取xml文件
            var xmlReader = XmlReader.Create(new StringReader(sb.ToString()));
            var dr = new DefaultDataContractReadHandler<Model>();
            var deserializeModel = dr.Read(xmlReader);
            deserializeModel.Number.Should().Be(11);
            deserializeModel.Text.Should().Be("Test1");
        }
    }
}