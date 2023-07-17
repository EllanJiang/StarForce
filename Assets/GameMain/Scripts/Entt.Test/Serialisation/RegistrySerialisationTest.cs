/*
* 文件名：RegistrySerialisationTest
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/17 11:26:52
* 修改记录：
*/

using System;
using System.IO;
using System.Text;
using System.Xml;
using Entt.Entities;
using Entt.Serialization;
using Entt.Serialization.Xml;
using Entt.Serialization.Xml.Impl;
using Entt.Test.Fixtures;
using FluentAssertions;
using UnityEngine;

namespace Entt.Test.Serialisation
{
    /// <summary>
    /// EntityRegistry序列化测试
    /// </summary>
    public class RegistrySerialisationTest:MonoBehaviour
    {
        EntityRegistry<EntityKey> ereg;
        XmlWriteHandlerRegistry wreg;
        XmlReadHandlerRegistry rreg;

        private void Start()
        {
            wreg = new XmlWriteHandlerRegistry();
            wreg.Register(XmlWriteHandlerRegistration.Create<TestStructFixture>(new DefaultDataContractWriteHandler<TestStructFixture>().Write, false));
            wreg.Register(XmlWriteHandlerRegistration.Create<StringBuilder>(new DefaultWriteHandler<StringBuilder>().Write, false));
            wreg.Register(XmlWriteHandlerRegistration.Create<int>(new DefaultWriteHandler<int>().Write, false));
            wreg.Register(XmlWriteHandlerRegistration.Create<float>(new DefaultWriteHandler<float>().Write, false));

            rreg = new XmlReadHandlerRegistry();
            rreg.Register(XmlReadHandlerRegistration.Create(new DefaultDataContractReadHandler<TestStructFixture>().Read, false));
            rreg.Register(XmlReadHandlerRegistration.Create(new DefaultReadHandler<StringBuilder>().Read, false));
            rreg.Register(XmlReadHandlerRegistration.Create(new DefaultReadHandler<int>().Read, false));
            rreg.Register(XmlReadHandlerRegistration.Create(new DefaultReadHandler<float>().Read, false));

            ereg = new EntityRegistry<EntityKey>(EntityKey.MaxAge, EntityKey.Create);
            ereg.Register<TestStructFixture>();
            ereg.Register<StringBuilder>();
            ereg.Register<int>();
            ereg.Register<float>();
            
            ereg.CreateAsActor().AssignComponent<TestStructFixture>(new TestStructFixture());
            ereg.CreateAsActor().AssignComponent<StringBuilder>().AttachTag<int>(100);

            TestPersistentView();
            TestStreamingView();
            TestAutomaticStreamingView();
        }

        //测试永久View
        private void TestPersistentView()
        {
            var str = WriteRegistry(ereg, wreg, true);
            Debug.Log("TestPersistentView str：" + str);

            var newReg = new EntityRegistry<EntityKey>(EntityKey.MaxAge, EntityKey.Create);
            newReg.Register<TestStructFixture>();
            newReg.Register<StringBuilder>();

            var xmlReader = XmlReader.Create(new StringReader(str));
            var reader = new XmlBulkArchiveReader<EntityKey>(rreg);
            var snapshotLoader = newReg.CreateLoader();             //快照加载器
            reader.ReadAll(xmlReader,snapshotLoader,new DefaultEntityKeyMapper().Register<EntityKey>(snapshotLoader.Map));

            var result = WriteRegistry(newReg, wreg, true);
            Debug.Log("TestPersistentView result：" + result);
            result.Should().Be(str);

        }
        
        //测试快照流
        private void TestStreamingView()
        {
            var str = WriteRegistry(ereg, wreg, false);
            Debug.Log("TestStreamingView str：" + str);
            
            var newReg = new EntityRegistry<EntityKey>(EntityKey.MaxAge, EntityKey.Create);
            newReg.Register<TestStructFixture>();
            newReg.Register<StringBuilder>();
            
            var xmlReader = XmlReader.Create(new StringReader(str));
            xmlReader.AdvanceToElement("snapshot");
            
            var snapshotLoader = newReg.CreateLoader();
            var streamReader = new SnapshotStreamReader<EntityKey>(snapshotLoader, new DefaultEntityKeyMapper().Register(snapshotLoader.Map));
            var archiveReader = new XmlEntityArchiveReader<EntityKey>(rreg, xmlReader);
            streamReader.ReadDestroyed(archiveReader)
                .ReadEntities(archiveReader)
                .ReadComponent<TestStructFixture>(archiveReader)
                .ReadComponent<StringBuilder>(archiveReader)
                .ReadTag<int>(archiveReader);

            var result = WriteRegistry(newReg, wreg, false);
            Debug.Log("TestStreamingView result：" + result);
            result.Should().Be(str);
        }
        
        //测试自动快照流
        private void TestAutomaticStreamingView()
        {
            var str = WriteRegistry(ereg, wreg, true);

            Debug.Log("TestAutomaticStreamingView str：" + str);

            var newReg = new EntityRegistry<EntityKey>(EntityKey.MaxAge, EntityKey.Create);
            newReg.Register<TestStructFixture>();
            newReg.Register<StringBuilder>();

            var xmlReader = XmlReader.Create(new StringReader(str));
            xmlReader.AdvanceToElement("snapshot");

            var snapshotLoader = newReg.CreateLoader();
            var streamReader = new SnapshotStreamReader<EntityKey>(snapshotLoader, new DefaultEntityKeyMapper().Register(snapshotLoader.Map));
            var archiveReader = new XmlEntityArchiveReader<EntityKey>(rreg, xmlReader);
            streamReader.ReadAll(archiveReader);

            var result = WriteRegistry(newReg, wreg, true);
            Debug.Log("TestAutomaticStreamingView result：" + result);
            result.Should().Be(str);
        }


        /// <summary>
        /// 保存Registry中所有Entity和Component及Tag
        /// </summary>
        /// <param name="entityRegistry"></param>
        /// <param name="writeHandlerRegistry"></param>
        /// <param name="autoHandlers"></param>
        /// <returns></returns>
        static string WriteRegistry(EntityRegistry<EntityKey> entityRegistry,
            XmlWriteHandlerRegistry writeHandlerRegistry, bool autoHandlers)
        {
            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb, new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "  "
            });

            var writer = new XmlArchiveWriter<EntityKey>(writeHandlerRegistry, xmlWriter);
            var snapshot = entityRegistry.CreateSnapshot();
            if (autoHandlers)  //自动处理
            {
                snapshot.WriteAll(writer);
            }
            else
            {
                //手动写入
                writer.WriteDefaultSnapshotDocumentHeader();
                
                snapshot.WriteDestroyed(writer);
                snapshot.WriteEntities(writer);
                snapshot.WriteComponent<TestStructFixture>(writer);
                snapshot.WriteComponent<StringBuilder>(writer);
                snapshot.WriteTag<int>(writer);
                snapshot.WriteTag<float>(writer);
                
                writer.WriteDefaultSnapshotDocumentFooter();
            }
            
            xmlWriter.Flush();
            return sb.ToString();
        }
    }
}