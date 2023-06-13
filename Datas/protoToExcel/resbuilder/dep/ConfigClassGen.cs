using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using UnityEngine;

namespace D11
{
#pragma warning disable 0414
    public class ConfigClassGen
    {
        private HashSet<Type> m_SelectedTypes = new HashSet<Type>(); 

        public void GenerateAllConfigClass(int platform)
        {
            //Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            Type[] types = System.Reflection.Assembly.GetAssembly(typeof(ConfigAttribute)).GetTypes();

            foreach (Type type in types)
            {
                if (type.ToString().IndexOf("Apollo") >= 0)
                {
                    continue;
                }

                // 对应的data类已存在就不需要重复生成。
                if (!IsObjConfigClassExisited(type))
                {
                    FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    for (int i = 0; i < fields.Length; ++i)
                    {
                        // 只要有配置了一个以上[ConfigArrt]属性的类都生成data类
                        object[] AttriList = fields[i].GetCustomAttributes(typeof(ConfigAttribute), false);

                        if (AttriList != null && AttriList.Length > 0)
                        {
                            m_SelectedTypes.Add(type);
                            break;
                        }
                    }
                }
            }

            foreach (Type type in m_SelectedTypes)
            {
                ClassGenerate(platform, type);
            }
        }

        //for format string
        public static string PROTO_HEAD_LINE = "syntax = \"proto3\";\n\n";
        public static string PROTO_NAMESPACE_LINE = "package " + "D11.pjson" + ";\n";
        public static string PROTO_MESSAGE_LINE = "\nmessage #MASSAGE_NAME#\n";
        public static string PROTO_MESSAGE_START_LINE = "{\n";
        public static string PROTO_REPEATED_LINE = "\trepeated #TYPE_NAME# #VAR_NAME# = #VAR_FEILD#;\n";
        public static string PROTO_NORMAL_LINE = "\t#TYPE_NAME# #VAR_NAME# = #VAR_FEILD#;\n";
        public static string PROTO_MESSAGE_END_LINE = "}";
        public static string FATHER_CLASS_REFERENCE_NAME = "parent";
        public static string NOTES_LINE = "\t// #DESC# \n";
        public static string IMPROT_LINE = "import \"#PROTO_NAME#Data.proto\";\n";

        public void ClassGenerate(int platform, Type type)
        {
            object obj = null;

            try
            {
                //使用无参数构造函数构造类的实例
                obj = Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                obj = null;
                UnityEngine.Debug.Log(e);
            }

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            Type baseType = type.BaseType;
            string baseTypeName;
            if (baseType != null && m_SelectedTypes.Contains(baseType))
            {
                baseTypeName = baseType.Name + "Data";
            }
            else
            {
                baseType = null;
                baseTypeName = "";
            }

            //开始构建.proto
            StringBuilder codeTemplate = new StringBuilder();
            codeTemplate.Append(PROTO_HEAD_LINE);
            if (baseType != null)
            {
                codeTemplate.Append(IMPROT_LINE.Replace("#PROTO_NAME#", baseType.Name));
            }
            codeTemplate.Append(PROTO_NAMESPACE_LINE);
            codeTemplate.Append(PROTO_MESSAGE_LINE.Replace("#MASSAGE_NAME#", type.Name + "Data"));
            codeTemplate.Append(PROTO_MESSAGE_START_LINE);

            bool isRepeated;
            string varType;
            string name;
            int count = 1;
            for (int i = 0; i < fields.Length; ++i)
            {
                if (!fields[i].IsLiteral)
                {
                    object[] AttributeList = fields[i].GetCustomAttributes(typeof(ConfigAttribute), false);
                    if (AttributeList != null && AttributeList.Length > 0)
                    {
                        if (baseType != null && baseType.GetField(fields[i].Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null)
                        {
                            continue;
                        }
                        name = fields[i].Name.Substring(2, fields[i].Name.Length - 2);
                        isRepeated = false;
                        // c#类型 -> protobuffer类型
                        if (fields[i].FieldType == typeof(double))
                        {
                            varType = "double";
                        }
                        else if(fields[i].FieldType == typeof(float))
                        {
                            varType = "float";
                        }
                        else if(fields[i].FieldType == typeof(int))
                        {
                            varType = "int32";
                        }
                        else if(fields[i].FieldType == typeof(long))
                        {
                            varType = "int64";
                        }
                        else if(fields[i].FieldType == typeof(uint))
                        {
                            varType = "uint32";
                        }
                        else if(fields[i].FieldType == typeof(ulong))
                        {
                            varType = "uint64";
                        }
                        else if(fields[i].FieldType == typeof(bool))
                        {
                            varType = "bool";
                        }
                        else if(fields[i].FieldType == typeof(string))
                        {
                            varType = "string";
                        }
                        else if (fields[i].FieldType == typeof(List<int>))
                        {
                            varType = "int";
                            isRepeated = true;
                        }
                        else if (fields[i].FieldType == typeof(List<float>))
                        {
                            varType = "float";
                            isRepeated = true;
                        }
                        else
                        {
                            varType = "";
                            UnityEngine.Debug.LogError("error type");
                        }
                        ConfigAttribute att = (ConfigAttribute)Convert.ChangeType(AttributeList[0], typeof(ConfigAttribute));
                        codeTemplate.Append(NOTES_LINE.Replace("#DESC#", att.Desc));
                        if (isRepeated)
                        {
                            codeTemplate.Append(PROTO_REPEATED_LINE.Replace("#TYPE_NAME#", varType).Replace("#VAR_NAME#", name).Replace("#VAR_FEILD#", count.ToString()));
                            ++count;
                        }
                        else
                        {
                            codeTemplate.Append(PROTO_NORMAL_LINE.Replace("#TYPE_NAME#", varType).Replace("#VAR_NAME#", name).Replace("#VAR_FEILD#", count.ToString()));
                            ++count;
                        }
                    }
                }
            }
            //增加子类的一个引用，特殊覆盖
            if (baseType != null)
            {
                codeTemplate.Append(PROTO_NORMAL_LINE.Replace("#TYPE_NAME#", baseTypeName).Replace("#VAR_NAME#", FATHER_CLASS_REFERENCE_NAME).Replace("#VAR_FEILD#", count.ToString()));
                count++;
            }
            codeTemplate.Append(PROTO_MESSAGE_END_LINE);
            if (count != 0)
            {
                string savePath = Application.dataPath + @"/../ConfigData/" + type.Name.ToString() + "Data.proto";

                using (StreamWriter sr = new StreamWriter(savePath))
                {
                    sr.Write(codeTemplate);
                }
            }       
        }

        private Boolean IsObjConfigClassExisited(Type objType)
        {
            //Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(ConfigAttribute));

            try
            {
                System.Object dataObj = assembly.CreateInstance(objType.FullName + "Data");
                return dataObj == null ? false : true;
            }
            catch (MissingMethodException exception)
            {
                UnityEngine.Debug.LogError(exception.Message);
                return false;
            }
        }
    }
}
