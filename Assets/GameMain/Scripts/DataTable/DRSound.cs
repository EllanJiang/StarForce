//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2020-07-12 14:49:13.492
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace StarForce
{
    /// <summary>
    /// 声音配置表。
    /// </summary>
    public class DRSound : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取声音编号。
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
        /// 获取优先级（默认0，128最高，-128最低）。
        /// </summary>
        public int Priority
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取是否循环。
        /// </summary>
        public bool Loop
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取音量（0~1）。
        /// </summary>
        public float Volume
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取声音空间混合量（0为2D，1为3D，中间值混合效果）。
        /// </summary>
        public float SpatialBlend
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取声音最大距离。
        /// </summary>
        public float MaxDistance
        {
            get;
            private set;
        }

        public override bool ParseDataRow(string dataRowString)
        {
            string[] columnStrings = dataRowString.Split(DataTableExtension.DataSplitSeparators);
            for (int i = 0; i < columnStrings.Length; i++)
            {
                columnStrings[i] = columnStrings[i].Trim(DataTableExtension.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnStrings[index++]);
            index++;
            AssetName = columnStrings[index++];
            Priority = int.Parse(columnStrings[index++]);
            Loop = bool.Parse(columnStrings[index++]);
            Volume = float.Parse(columnStrings[index++]);
            SpatialBlend = float.Parse(columnStrings[index++]);
            MaxDistance = float.Parse(columnStrings[index++]);

            GeneratePropertyArray();
            return true;
        }

        private void GeneratePropertyArray()
        {

        }
    }
}
