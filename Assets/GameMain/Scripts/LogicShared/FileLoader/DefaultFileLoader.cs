/*
* 文件名：DefaultFileLoader
* 文件描述：默认文件加载器
* 作者：aronliang
* 创建时间：2023/06/15 17:17:23
* 修改记录：
*/

using System.IO;
using System.Text;

namespace LogicShared.FileLoader
{
    public class DefaultFileLoader:IFileLoader
    {
        private string AssetRootPath;
        public DefaultFileLoader(string assetRootPath)
        {
            AssetRootPath = assetRootPath;
        }
        
        public byte[] ReadStreamingFile(string filePath, out int length)
        {
            length = 0;
            var assetFilePath = FileLoaderUtil.GetFileAssetsPath(filePath);
            if (!File.Exists(assetFilePath))
            {
                return null;
            }

            var bytes = File.ReadAllBytes(assetFilePath);
            length = bytes.Length;
            return bytes;
        }

        public byte[] ReadStreamingFile(string filePath)
        {
            var assetFilePath = FileLoaderUtil.GetFileAssetsPath(filePath);
            if (!File.Exists(assetFilePath))
            {
                return null;
            }

            return File.ReadAllBytes(assetFilePath);
        }

        public string ReadString(string filePath)
        {
            var bytes = ReadStreamingFile(filePath,out int length);
            if (bytes != null)
            {
                return Encoding.UTF8.GetString(bytes, 0, length);
            }

            return string.Empty;
        }
    }
}