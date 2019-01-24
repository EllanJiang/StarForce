//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2019-01-24 21:44:02.194
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace StarForce
{
    /// <summary>
    /// 场景配置表。
    /// </summary>
    public class DRScene : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取场景编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取资源名称。
        /// </summary>
        public string AssetName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取背景音乐编号。
        /// </summary>
        public int BackgroundMusicId
        {
            get;
            private set;
        }

        public override bool ParseDataRow(GameFrameworkSegment<string> dataRowSegment)
        {
            string[] text = dataRowSegment.Source.Substring(dataRowSegment.Offset, dataRowSegment.Length).Split('\t');
            int index = 0;
            index++;
            m_Id = int.Parse(text[index++]);
            index++;
            AssetName = text[index++];
            BackgroundMusicId = int.Parse(text[index++]);

            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(GameFrameworkSegment<byte[]> dataRowSegment)
        {
            using (MemoryStream memoryStream = new MemoryStream(dataRowSegment.Source, false))
            {
                return ParseDataRow(new GameFrameworkSegment<Stream>(memoryStream, dataRowSegment.Offset, dataRowSegment.Length));
            }
        }

        public override bool ParseDataRow(GameFrameworkSegment<Stream> dataRowSegment)
        {
            Log.Warning("Not implemented ParseDataRow(GameFrameworkSegment<Stream>)");
            return false;
        }

        private void GeneratePropertyArray()
        {

        }
    }
}
