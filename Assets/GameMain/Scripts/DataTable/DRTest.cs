//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2020-07-13 00:03:21.648
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

        public override bool ParseDataRow(string dataRowString, object userData)
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
            BoolValue = bool.Parse(columnStrings[index++]);
            ByteValue = byte.Parse(columnStrings[index++]);
            CharValue = char.Parse(columnStrings[index++]);
            Color32Value = DataTableExtension.ParseColor32(columnStrings[index++]);
            ColorValue = DataTableExtension.ParseColor(columnStrings[index++]);
            index++;
            DateTimeValue = DateTime.Parse(columnStrings[index++]);
            DecimalValue = decimal.Parse(columnStrings[index++]);
            DoubleValue = double.Parse(columnStrings[index++]);
            FloatValue = float.Parse(columnStrings[index++]);
            IntValue = int.Parse(columnStrings[index++]);
            LongValue = long.Parse(columnStrings[index++]);
            QuaternionValue = DataTableExtension.ParseQuaternion(columnStrings[index++]);
            RectValue = DataTableExtension.ParseRect(columnStrings[index++]);
            SByteValue = sbyte.Parse(columnStrings[index++]);
            ShortValue = short.Parse(columnStrings[index++]);
            StringValue = columnStrings[index++];
            UIntValue = uint.Parse(columnStrings[index++]);
            ULongValue = ulong.Parse(columnStrings[index++]);
            UShortValue = ushort.Parse(columnStrings[index++]);
            Vector2Value = DataTableExtension.ParseVector2(columnStrings[index++]);
            Vector3Value = DataTableExtension.ParseVector3(columnStrings[index++]);
            Vector4Value = DataTableExtension.ParseVector4(columnStrings[index++]);

            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
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
                    StringValue = binaryReader.ReadString();
                    UIntValue = binaryReader.Read7BitEncodedUInt32();
                    ULongValue = binaryReader.Read7BitEncodedUInt64();
                    UShortValue = binaryReader.ReadUInt16();
                    Vector2Value = binaryReader.ReadVector2();
                    Vector3Value = binaryReader.ReadVector3();
                    Vector4Value = binaryReader.ReadVector4();
                }
            }

            GeneratePropertyArray();
            return true;
        }

        private void GeneratePropertyArray()
        {

        }
    }
}
