﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.IO;
using UnityEngine;

namespace StarForce.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class Vector2Processor : GenericDataProcessor<Vector2>
        {
            public override bool IsSystem
            {
                get
                {
                    return false;
                }
            }

            public override string LanguageKeyword
            {
                get
                {
                    return "Vector2";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "vector2",
                    "unityengine.vector2"
                };
            }

            public override Vector2 Parse(string value)
            {
                string[] splitedValue = value.Split(',');
                return new Vector2(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]));
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                Vector2 vector2 = Parse(value);
                binaryWriter.Write(vector2.x);
                binaryWriter.Write(vector2.y);
            }
        }
    }
}
