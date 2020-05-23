//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2020-04-27 17:07:19.110
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
    /// 测试表格生成。
    /// </summary>
    public class DRTest : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取Bool值。
        /// </summary>
        public bool BoolValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Byte值。
        /// </summary>
        public byte ByteValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Char值。
        /// </summary>
        public char CharValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Color32值。
        /// </summary>
        public Color32 Color32Value
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Color值。
        /// </summary>
        public Color ColorValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取DateTime值。
        /// </summary>
        public DateTime DateTimeValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Decimal值。
        /// </summary>
        public decimal DecimalValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Double值。
        /// </summary>
        public double DoubleValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Float值。
        /// </summary>
        public float FloatValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Int值。
        /// </summary>
        public int IntValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Long值。
        /// </summary>
        public long LongValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Quaternion值。
        /// </summary>
        public Quaternion QuaternionValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Rect值。
        /// </summary>
        public Rect RectValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取SByte值。
        /// </summary>
        public sbyte SByteValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Short值。
        /// </summary>
        public short ShortValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取String值。
        /// </summary>
        public string StringValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取UInt值。
        /// </summary>
        public uint UIntValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取ULong值。
        /// </summary>
        public ulong ULongValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取UShort值。
        /// </summary>
        public ushort UShortValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Vector2值。
        /// </summary>
        public Vector2 Vector2Value
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Vector3值。
        /// </summary>
        public Vector3 Vector3Value
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Vector4值。
        /// </summary>
        public Vector4 Vector4Value
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
                BoolValue = bool.Parse(columnTexts[index++]);
                ByteValue = byte.Parse(columnTexts[index++]);
                CharValue = char.Parse(columnTexts[index++]);
                Color32Value = DataTableExtension.ParseColor32(columnTexts[index++]);
                ColorValue = DataTableExtension.ParseColor(columnTexts[index++]);
                index++;
                DateTimeValue = DateTime.Parse(columnTexts[index++]);
                DecimalValue = decimal.Parse(columnTexts[index++]);
                DoubleValue = double.Parse(columnTexts[index++]);
                FloatValue = float.Parse(columnTexts[index++]);
                IntValue = int.Parse(columnTexts[index++]);
                LongValue = long.Parse(columnTexts[index++]);
                QuaternionValue = DataTableExtension.ParseQuaternion(columnTexts[index++]);
                RectValue = DataTableExtension.ParseRect(columnTexts[index++]);
                SByteValue = sbyte.Parse(columnTexts[index++]);
                ShortValue = short.Parse(columnTexts[index++]);
                StringValue = columnTexts[index++];
                UIntValue = uint.Parse(columnTexts[index++]);
                ULongValue = ulong.Parse(columnTexts[index++]);
                UShortValue = ushort.Parse(columnTexts[index++]);
                Vector2Value = DataTableExtension.ParseVector2(columnTexts[index++]);
                Vector3Value = DataTableExtension.ParseVector3(columnTexts[index++]);
                Vector4Value = DataTableExtension.ParseVector4(columnTexts[index++]);
            }
            else if (dataType == typeof(byte[]))
            {
                string[] strings = (string[])dataTableUserData;
                using (MemoryStream memoryStream = new MemoryStream((byte[])dataRowSegment.Data, dataRowSegment.Offset, dataRowSegment.Length, false))
                {
                    using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                    {
                        m_Id = binaryReader.Read7BitEncodedInt32();
                        BoolValue = binaryReader.ReadBoolean();
                        ByteValue = binaryReader.ReadByte();
                        CharValue = binaryReader.ReadChar();
                        Color32Value = binaryReader.ReadColor32();
                        ColorValue = binaryReader.ReadColor();
                        DateTimeValue = binaryReader.ReadDateTime();
                        DecimalValue = binaryReader.ReadDecimal();
                        DoubleValue = binaryReader.ReadDouble();
                        FloatValue = binaryReader.ReadSingle();
                        IntValue = binaryReader.Read7BitEncodedInt32();
                        LongValue = binaryReader.Read7BitEncodedInt64();
                        QuaternionValue = binaryReader.ReadQuaternion();
                        RectValue = binaryReader.ReadRect();
                        SByteValue = binaryReader.ReadSByte();
                        ShortValue = binaryReader.ReadInt16();
                        StringValue = strings[binaryReader.Read7BitEncodedInt32()];
                        UIntValue = binaryReader.Read7BitEncodedUInt32();
                        ULongValue = binaryReader.Read7BitEncodedUInt64();
                        UShortValue = binaryReader.ReadUInt16();
                        Vector2Value = binaryReader.ReadVector2();
                        Vector3Value = binaryReader.ReadVector3();
                        Vector4Value = binaryReader.ReadVector4();
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
