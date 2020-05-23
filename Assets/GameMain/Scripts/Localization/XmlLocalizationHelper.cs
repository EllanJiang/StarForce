//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Xml;
using UnityGameFramework.Runtime;

namespace StarForce
{
    /// <summary>
    /// XML 格式的本地化辅助器。
    /// </summary>
    public class XmlLocalizationHelper : DefaultLocalizationHelper
    {
        /// <summary>
        /// 解析字典。
        /// </summary>
        /// <param name="dictionaryData">要解析的字典数据。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否解析字典成功。</returns>
        public override bool ParseDictionary(object dictionaryData, object userData)
        {
            try
            {
                string dictionaryText = dictionaryData as string;
                if (dictionaryText != null)
                {
                    string currentLanguage = GameEntry.Localization.Language.ToString();
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml((string)dictionaryData);
                    XmlNode xmlRoot = xmlDocument.SelectSingleNode("Dictionaries");
                    XmlNodeList xmlNodeDictionaryList = xmlRoot.ChildNodes;
                    for (int i = 0; i < xmlNodeDictionaryList.Count; i++)
                    {
                        XmlNode xmlNodeDictionary = xmlNodeDictionaryList.Item(i);
                        if (xmlNodeDictionary.Name != "Dictionary")
                        {
                            continue;
                        }

                        string language = xmlNodeDictionary.Attributes.GetNamedItem("Language").Value;
                        if (language != currentLanguage)
                        {
                            continue;
                        }

                        XmlNodeList xmlNodeStringList = xmlNodeDictionary.ChildNodes;
                        for (int j = 0; j < xmlNodeStringList.Count; j++)
                        {
                            XmlNode xmlNodeString = xmlNodeStringList.Item(j);
                            if (xmlNodeString.Name != "String")
                            {
                                continue;
                            }

                            string key = xmlNodeString.Attributes.GetNamedItem("Key").Value;
                            string value = xmlNodeString.Attributes.GetNamedItem("Value").Value;
                            if (!AddRawString(key, value))
                            {
                                Log.Warning("Can not add raw string with key '{0}' which may be invalid or duplicate.", key);
                                return false;
                            }
                        }
                    }

                    return true;
                }

                Log.Warning("Can not parse dictionary data which type '{0}' is invalid.", dictionaryData.GetType().FullName);
                return false;
            }
            catch (Exception exception)
            {
                Log.Warning("Can not parse dictionary data with exception '{0}'.", exception.ToString());
                return false;
            }
        }
    }
}
