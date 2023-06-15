/*
* 文件名：FileSystemFileLoader
* 文件描述：文件系统的文件加载器
* 作者：aronliang
* 创建时间：2023/06/15 17:21:03
* 修改记录：
*/

using System;
using System.IO;
using System.Text;
using GameFramework;
using GameFramework.FileSystem;
using LogicShared.FileLoader;
using StarForce;

namespace GameMain.FileLoader
{
    public class FileSystemFileLoader:IFileLoader
    {
        private string m_PersistentPath;
        private string m_StreamingAssetsPath;
        
        private byte[] m_CacheBytes;

        public FileSystemFileLoader(string persistentPath, string streamingAssetsPath)
        {
            m_PersistentPath = persistentPath;
            m_StreamingAssetsPath = streamingAssetsPath;
        }

        //从文件路径中获取文件系统的名字
        //正常传入基本是这样的：filePath=“DataTables/LubanByteDatas/luban_tbitem.bytes"
        private string GetFileSystemName(string filePath)
        {
            filePath = Utility.Path.GetRegularPath(filePath);
            if (filePath.IndexOf('/') < 0)
            {
                return filePath;
            }
            
            return filePath.Substring(0, filePath.IndexOf('/'));
        }

        //根据文件系统名获取文件系统路径
        private string GetFileSystemPath(string fileSystemName)
        {
            string fullPath = String.Empty;
            fullPath = $"{m_PersistentPath}/{fileSystemName}.{IFileLoader.ExtensionName}";
            if (!File.Exists(fullPath))
            {
                fullPath = string.Empty;
            }

            if (string.IsNullOrEmpty(fullPath))
            {
                fullPath = $"{m_StreamingAssetsPath}/{fileSystemName}.{IFileLoader.ExtensionName}";
            }

            return fullPath;
        }

        private IFileSystem GetFileSystem(string fileSystemPath)
        {
            if (GameEntry.FileSystem.HasFileSystem(fileSystemPath))
            {
                return GameEntry.FileSystem.GetFileSystem(fileSystemPath);
            }

            var fileSystem = GameEntry.FileSystem.LoadFileSystem(fileSystemPath, FileSystemAccess.Read);
            return fileSystem;
        }

        private byte[] GetCacheBytes(int lenght)
        {
            if (m_CacheBytes != null && m_CacheBytes.Length >= lenght)
            {
                return m_CacheBytes;
            }

            m_CacheBytes = new byte[lenght * 2];
            return m_CacheBytes;
        }
        
        public byte[] ReadStreamingFile(string filePath,out int length)
        {
            string fileSystemName = GetFileSystemName(filePath);
            if (string.IsNullOrEmpty(fileSystemName))
            {
                throw new Exception("无法获取文件系统名称，文件路径是:" + filePath);
            }

            string fileSystemPath = GetFileSystemPath(fileSystemName);
            var fileSystem = GetFileSystem(fileSystemPath);
            if (fileSystem == null)
            {
                throw new Exception("无法获取文件系统，文件系统路径是:" + fileSystemPath);
            }

            length = 0;
            var realFilePath = FileLoaderUtil.GetFileAssetsPath(filePath);
            var fileInfo = fileSystem.GetFileInfo(realFilePath);
            if (fileInfo.Length == 0)
            {
                return null;
            }

            byte[] bytes = GetCacheBytes(fileInfo.Length);
            length = fileSystem.ReadFile(realFilePath, bytes);
            return bytes;
        }

        public byte[] ReadStreamingFile(string filePath)
        {
            string fileSystemName = GetFileSystemName(filePath);
            if (string.IsNullOrEmpty(fileSystemName))
            {
                throw new Exception("无法获取文件系统名称，文件路径是:" + filePath);
            }

            string fileSystemPath = GetFileSystemPath(fileSystemName);
            var fileSystem = GetFileSystem(fileSystemPath);
            if (fileSystem == null)
            {
                throw new Exception("无法获取文件系统，文件系统路径是:" + fileSystemPath);
            }
            var realFilePath = FileLoaderUtil.GetFileAssetsPath(filePath);;
            return fileSystem.ReadFile(realFilePath);
        }

        public string ReadString(string filePath)
        {
            var bytes = ReadStreamingFile(filePath, out int length);
            return bytes == null ? null : Encoding.UTF8.GetString(bytes,0, length);
        }
    }
}