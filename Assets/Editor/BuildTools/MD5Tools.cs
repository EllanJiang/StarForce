/*
* 文件名：MD5Tools
* 文件描述：
* 作者：aronliang
* 创建时间：2023/05/24 17:10:09
* 修改记录：
*/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class MD5Tools
    {
        private static void GetAllFiles(string dirPath, MemoryStream ms, string aExtension)
        {
            DirectoryInfo root = new DirectoryInfo(dirPath);
            var folders = root.GetDirectories();
            foreach (var folder in folders)
            {
                if(folder.Name != "Gen")//屏蔽自动生成代码
                    GetAllFiles(folder.FullName, ms, aExtension);
            }

            //遍历文件
            foreach (FileInfo NextFile in root.GetFiles())
            {
                if (NextFile.Extension == aExtension || NextFile.Name == "ComponentDef.cs" || NextFile.Name == "LogicVersion.json")
                {
                    //UnityEngine.Debug.LogError($"NextFile.Name = {NextFile.Name}");
                    continue;
                }
                // 获取文件完整路径
                string fileFullName = NextFile.FullName;
                //写入内存流
                using (FileStream f = new FileStream(fileFullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] buffer = new byte[f.Length];
                    f.Read(buffer, 0, buffer.Length);
                    ms.Write(buffer, 0, buffer.Length);
                }
            }
        }
        public static string GetConfigMd5(string dirPath)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                GetAllFiles(dirPath, ms, ".meta");
                DirectoryInfo theFolder = new DirectoryInfo(dirPath);
                if (!theFolder.Exists)
                    return "dirPath_Is_Null";
                long bufferSize = ms.Length;
                byte[] buff = new byte[bufferSize];
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                md5.Initialize();
                long offset = 0;
                while (offset < ms.Length)
                {
                    long readSize = bufferSize;
                    if (offset + readSize > ms.Length)
                        readSize = ms.Length - offset;
                    ms.Read(buff, 0, Convert.ToInt32(readSize));
                    if (offset + readSize < ms.Length)
                        md5.TransformBlock(buff, 0, Convert.ToInt32(readSize), buff, 0);
                    else
                        md5.TransformFinalBlock(buff, 0, Convert.ToInt32(readSize));
                    offset += bufferSize;
                }
                if (offset >= ms.Length)
                {
                    ms.Close();
                    byte[] result = md5.Hash;
                    md5.Clear();
                    StringBuilder sb = new StringBuilder(32);
                    for (int i = 0; i < result.Length; i++)
                        sb.Append(result[i].ToString("X2"));
                    return sb.ToString();
                }
                else
                {
                    ms.Close();
                    return "Length_Error";
                }
            }
        }

        /// <summary>
        /// 获取所有后缀为.cs的文件的md5
        /// </summary>
        /// <param name="aLogicFramePath"></param>
        /// <returns></returns>
        public static string GetLogicScriptsMd5(string aLogicFramePath,string aLogicSharedPath)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                GetAllFiles(aLogicFramePath, ms, ".meta");
                GetAllFiles(aLogicSharedPath, ms, ".meta");
                DirectoryInfo theFolder = new DirectoryInfo(aLogicFramePath);
                if (!theFolder.Exists)
                    return "dirPath_Is_Null";

                long bufferSize = ms.Length; //1048576;
                //UnityEngine.Debug.LogError($"calc md5 file bufferSize:{bufferSize}");
                byte[] buff = new byte[bufferSize];
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                md5.Initialize();
                long offset = 0;
                while (offset < ms.Length)
                {
                    long readSize = bufferSize;
                    if (offset + readSize > ms.Length)
                        readSize = ms.Length - offset;
                    ms.Read(buff, 0, Convert.ToInt32(readSize));
                    if (offset + readSize < ms.Length)
                        md5.TransformBlock(buff, 0, Convert.ToInt32(readSize), buff, 0);
                    else
                        md5.TransformFinalBlock(buff, 0, Convert.ToInt32(readSize));
                    offset += bufferSize;
                }

                if (offset >= ms.Length)
                {
                    ms.Close();
                    byte[] result = md5.Hash;
                    md5.Clear();
                    StringBuilder sb = new StringBuilder(32);
                    for (int i = 0; i < result.Length; i++)
                        sb.Append(result[i].ToString("X2"));
                    return sb.ToString();
                }
                else
                {
                    ms.Close();
                    return "Length_Error";
                }
            }
        }

    }