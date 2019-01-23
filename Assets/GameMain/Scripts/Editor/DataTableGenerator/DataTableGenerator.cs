using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityGameFramework.Editor.DataTableTools;

namespace StarForce.Editor.DataTableTools
{
    public sealed partial class DataTableGenerator
    {
        private const string DataTablePath = "Assets/GameMain/DataTables";
        private const string CSharpCodePath = "Assets/GameMain/Scripts/DataTable";
        private const string CSharpCodeTemplateFileName = "Assets/GameMain/Configs/DataTableCodeTemplate.txt";
        private static readonly Regex EndWithNumberRegex = new Regex(@"\d+$");

        public static DataTableProcessor CreateDataTableProcessor(string dataTableName)
        {
            return new DataTableProcessor(Utility.Path.GetCombinePath(DataTablePath, dataTableName + ".txt"), Encoding.Default, 1, 2, null, 3, 4, 1);
        }

        public static void GenerateDataFile(DataTableProcessor dataTableProcessor, string dataTableName)
        {
            string binaryDataFileName = Utility.Path.GetCombinePath(DataTablePath, dataTableName + ".bytes");
            if (!dataTableProcessor.GenerateDataFile(binaryDataFileName, Encoding.UTF8) && File.Exists(binaryDataFileName))
            {
                File.Delete(binaryDataFileName);
            }
        }

        public static void GenerateCodeFile(DataTableProcessor dataTableProcessor, string dataTableName)
        {
            dataTableProcessor.SetCodeTemplate(CSharpCodeTemplateFileName, Encoding.UTF8);
            dataTableProcessor.SetCodeGenerator(DataTableCodeGenerator);

            string csharpCodeFileName = Utility.Path.GetCombinePath(CSharpCodePath, "DR" + dataTableName + ".cs");
            Data data = new Data()
            {
                CreateTime = DateTime.Now,
                NameSpace = "StarForce",
                ClassName = "DR" + dataTableName,
            };

            if (!dataTableProcessor.GenerateCodeFile(csharpCodeFileName, Encoding.UTF8, data) && File.Exists(csharpCodeFileName))
            {
                File.Delete(csharpCodeFileName);
            }
        }

        private static void DataTableCodeGenerator(DataTableProcessor dataTableProcessor, StringBuilder codeContent, object userData)
        {
            Data data = (Data)userData;

            codeContent.Replace("__DATA_TABLE_CREATE_TIME__", data.CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            codeContent.Replace("__DATA_TABLE_NAME_SPACE__", data.NameSpace);
            codeContent.Replace("__DATA_TABLE_CLASS_NAME__", data.ClassName);
            codeContent.Replace("__DATA_TABLE_COMMENT__", dataTableProcessor.GetValue(0, 1) + "。");
            codeContent.Replace("__DATA_TABLE_ID_COMMENT__", "获取" + dataTableProcessor.GetComment(dataTableProcessor.IdColumn) + "。");
            codeContent.Replace("__DATA_TABLE_PROPERTIES__", GenerateDataTableProperties(dataTableProcessor, data));
            codeContent.Replace("__DATA_TABLE_PROPERTY_ARRAY__", GenerateDataTablePropertyArray(dataTableProcessor, data));
            codeContent.Replace("__DATA_TABLE_STRING_PARSER__", GenerateDataTableStringParser(dataTableProcessor, data));
            codeContent.Replace("__DATA_TABLE_BYTES_PARSER__", GenerateDataTableBytesParser(dataTableProcessor, data));
            codeContent.Replace("__DATA_TABLE_STREAM_PARSER__", GenerateDataTableStreamParser(dataTableProcessor, data));
        }

        private static string GenerateDataTableProperties(DataTableProcessor dataTableProcessor, Data data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool firstProperty = true;
            for (int i = 0; i < dataTableProcessor.RawColumnCount; i++)
            {
                if (dataTableProcessor.IsCommentColumn(i))
                {
                    // 注释列
                    continue;
                }

                if (i == dataTableProcessor.IdColumn)
                {
                    // 编号列
                    continue;
                }

                if (firstProperty)
                {
                    firstProperty = false;
                }
                else
                {
                    stringBuilder.AppendLine().AppendLine();
                }

                stringBuilder
                    .AppendLine("        /// <summary>")
                    .AppendFormat("        /// 获取{0}。", dataTableProcessor.GetComment(i)).AppendLine()
                    .AppendLine("        /// </summary>")
                    .AppendFormat("        public {0} {1}", dataTableProcessor.GetStandardType(i), dataTableProcessor.GetName(i)).AppendLine()
                    .AppendLine("        {")
                    .AppendLine("            get;")
                    .AppendLine("            private set;")
                    .Append("        }");
            }

            return stringBuilder.ToString();
        }

        private static string GenerateDataTablePropertyArray(DataTableProcessor dataTableProcessor, Data data)
        {
            Dictionary<string, string> propertyType = new Dictionary<string, string>();
            Dictionary<string, List<KeyValuePair<int, string>>> propertyArray = new Dictionary<string, List<KeyValuePair<int, string>>>();
            for (int i = 0; i < dataTableProcessor.RawColumnCount; i++)
            {
                if (dataTableProcessor.IsCommentColumn(i))
                {
                    // 注释列
                    continue;
                }

                if (i == dataTableProcessor.IdColumn)
                {
                    // 编号列
                    continue;
                }

                string name = dataTableProcessor.GetName(i);
                if (!EndWithNumberRegex.IsMatch(name))
                {
                    continue;
                }

                string arrayName = EndWithNumberRegex.Replace(name, string.Empty);
                int id = int.Parse(EndWithNumberRegex.Match(name).Value);

                List<KeyValuePair<int, string>> property = null;
                if (!propertyArray.TryGetValue(arrayName, out property))
                {
                    propertyType.Add(arrayName, dataTableProcessor.GetStandardType(i));
                    property = new List<KeyValuePair<int, string>>();
                    propertyArray.Add(arrayName, property);
                }

                property.Add(new KeyValuePair<int, string>(id, name));
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, List<KeyValuePair<int, string>>> i in propertyArray)
            {
                stringBuilder
                    .AppendFormat("        private KeyValuePair<int, {0}>[] m_{1} = null;", propertyType[i.Key], i.Key).AppendLine()
                    .AppendFormat("        public int Get{0}(int id)", i.Key).AppendLine()
                    .AppendLine("        {")
                    .AppendFormat("            foreach (KeyValuePair<int, {0}> i in m_{1})", propertyType[i.Key], i.Key).AppendLine()
                    .AppendLine("            {")
                    .AppendLine("                if (i.Key == id)")
                    .AppendLine("                {")
                    .AppendFormat("                    return i.Value;", propertyType[i.Key], i.Key).AppendLine()
                    .AppendLine("                }")
                    .AppendLine("            }")
                    .AppendLine()
                    .AppendFormat("            throw new GameFrameworkException(Utility.Text.Format(\"Invalid id '{{0}}' for {0}.\", id.ToString()));", i.Key).AppendLine()
                    .AppendLine("        }");
            }

            return stringBuilder.ToString();
        }

        private static string GenerateDataTableStringParser(DataTableProcessor dataTableProcessor, Data data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder
                .AppendLine("        public override bool ParseDataRow(GameFrameworkSegment<string> dataRowSegment)")
                .AppendLine("        {")
                .AppendLine("            string[] text = DataTableExtension.SplitDataRow(dataRowSegment);")
                .AppendLine("            int index = 0;");

            for (int i = 0; i < dataTableProcessor.RawColumnCount; i++)
            {
                if (dataTableProcessor.IsCommentColumn(i))
                {
                    // 注释列
                    stringBuilder.AppendLine("            index++;");
                    continue;
                }

                if (i == dataTableProcessor.IdColumn)
                {
                    // 编号列
                    stringBuilder.AppendLine("            m_Id = int.Parse(text[index++]);");
                    continue;
                }

                string standardType = dataTableProcessor.GetStandardType(i);
                switch (standardType)
                {
                    case "string":
                        stringBuilder.AppendFormat("            {0} = text[index++];", dataTableProcessor.GetName(i)).AppendLine();
                        break;
                    default:
                        stringBuilder.AppendFormat("            {0} = {1}.Parse(text[index++]);", dataTableProcessor.GetName(i), standardType).AppendLine();
                        break;

                }
            }

            stringBuilder
                .AppendLine()
                .AppendLine("            return true;")
                .Append("        }");

            return stringBuilder.ToString();
        }

        private static string GenerateDataTableBytesParser(DataTableProcessor dataTableProcessor, Data data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder
                .AppendLine("        public override bool ParseDataRow(GameFrameworkSegment<byte[]> dataRowSegment)")
                .AppendLine("        {")
                .AppendLine("            Log.Warning(\"Not implemented ParseDataRow(GameFrameworkSegment<byte[]>)\");")
                .AppendLine("            return false;")
                .Append("        }");

            return stringBuilder.ToString();
        }

        private static string GenerateDataTableStreamParser(DataTableProcessor dataTableProcessor, Data data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder
                .AppendLine("        public override bool ParseDataRow(GameFrameworkSegment<Stream> dataRowSegment)")
                .AppendLine("        {")
                .AppendLine("            Log.Warning(\"Not implemented ParseDataRow(GameFrameworkSegment<Stream>)\");")
                .AppendLine("            return false;")
                .Append("        }");

            return stringBuilder.ToString();
        }
    }
}
