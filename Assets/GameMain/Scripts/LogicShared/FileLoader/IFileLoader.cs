/*
* 文件名：IFileLoader
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/15 17:16:24
* 修改记录：
*/

namespace LogicShared.FileLoader
{
    public interface IFileLoader
    {
        static string ExtensionName = "dat";
        
        /// <summary>
        /// 返回的数组是缓存的字节数组
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        byte[] ReadStreamingFile(string filePath,out int length);
        /// <summary>
        /// 返回的是全新的字节数组
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        byte[] ReadStreamingFile(string filePath);
        string ReadString(string filePath);
    }
}