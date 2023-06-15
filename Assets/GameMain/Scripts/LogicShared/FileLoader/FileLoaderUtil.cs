/*
* 文件名：FileLoaderUtil
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/15 18:07:56
* 修改记录：
*/

namespace LogicShared.FileLoader
{
    public class FileLoaderUtil
    {
        //文件系统根目录路径
        public static string AssetFileRootPath = "Assets/FileAssets";
        
        private static IFileLoader s_FileLoader;
        public static IFileLoader FileLoader
        {
            get
            {
                if (s_FileLoader == null)
                {
                    s_FileLoader = new DefaultFileLoader(AssetFileRootPath);
                }

                return s_FileLoader;
            }

            set
            {
                s_FileLoader = value;
            }
        }


        /// <summary>
        /// 读取文件，返回二进制数组
        /// </summary>
        /// <param name="filePath">Assets/FileAsset下的的文件路径</param>
        /// <returns></returns>
        public static byte[] ReadStreamingFile(string filePath)
        {
            return FileLoader.ReadStreamingFile(filePath);
        }

        /// <summary>
        /// 读取文件，返回字符串
        /// </summary>
        /// <param name="filePath">Assets/FileAsset下的的文件路径</param>
        /// <returns></returns>
        public static string GetString(string filePath)
        {
            return FileLoader.ReadString(filePath);
        }

        /// <summary>
        /// 返回Assets/FileAssets下的文件路径。
        /// 比如传入 DataTables/LubanByteDatas/luban_tbitem.bytes，返回 Assets/FileAssets/DataTables/LubanByteDatas/luban_tbitem.bytes
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileAssetsPath(string filePath)
        {
            return $"{AssetFileRootPath}/{filePath}";
        }
     }
}