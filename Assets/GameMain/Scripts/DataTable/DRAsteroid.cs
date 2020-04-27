//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2020-04-27 17:07:19.132
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
    /// 小行星表。
    /// </summary>
    public class DRAsteroid : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取小行星编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取最大生命。
        /// </summary>
        public int MaxHP
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取冲击力。
        /// </summary>
        public int Attack
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取速度。
        /// </summary>
        public float Speed
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取角速度。
        /// </summary>
        public float AngularSpeed
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取死亡特效编号。
        /// </summary>
        public int DeadEffectId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取死亡声音编号。
        /// </summary>
        public int DeadSoundId
        {
            get;
            private set;
        }

        public override bool ParseDataRow(GameFrameworkDataSegment dataRowSegment, object dataTableUserData)
        {
            Type dataType = dataRowSegment.DataType;
            if (dataType == typeof(string))
            {
                string[] columnTexts = ((string)dataRowSegment.Data).Substring(dataRowSegment.Offset, dataRowSegment.Length).Split(DataTableExtension.DataSplitSeparators);
                for (int i = 0; i < columnTexts.Length; i++)
                {
                    columnTexts[i] = columnTexts[i].Trim(DataTableExtension.DataTrimSeparators);
                }

                int index = 0;
                index++;
                m_Id = int.Parse(columnTexts[index++]);
                index++;
                MaxHP = int.Parse(columnTexts[index++]);
                Attack = int.Parse(columnTexts[index++]);
                Speed = float.Parse(columnTexts[index++]);
                AngularSpeed = float.Parse(columnTexts[index++]);
                DeadEffectId = int.Parse(columnTexts[index++]);
                DeadSoundId = int.Parse(columnTexts[index++]);
            }
            else if (dataType == typeof(byte[]))
            {
                string[] strings = (string[])dataTableUserData;
                using (MemoryStream memoryStream = new MemoryStream((byte[])dataRowSegment.Data, dataRowSegment.Offset, dataRowSegment.Length, false))
                {
                    using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                    {
                        m_Id = binaryReader.Read7BitEncodedInt32();
                        MaxHP = binaryReader.Read7BitEncodedInt32();
                        Attack = binaryReader.Read7BitEncodedInt32();
                        Speed = binaryReader.ReadSingle();
                        AngularSpeed = binaryReader.ReadSingle();
                        DeadEffectId = binaryReader.Read7BitEncodedInt32();
                        DeadSoundId = binaryReader.Read7BitEncodedInt32();
                    }
                }
            }
            else
            {
                Log.Warning("Can not parse data row which type '{0}' is invalid.", dataType.FullName);
                return false;
            }

            GeneratePropertyArray();
            return true;
        }

        private void GeneratePropertyArray()
        {

        }
    }
}
