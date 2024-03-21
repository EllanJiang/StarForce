/*
* 文件名：FileComponent
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/15 18:26:10
* 修改记录：
*/

using System;
using GameMain.FileLoader;
using LogicShared.FileLoader;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public class FileComponent:GameFrameworkComponent
    {
        public bool UseFileSystem;

        private IFileLoader _FileLoader;

        public IFileLoader FileLoader
        {
            get { return _FileLoader; }
        }
        
        private void Start()
        {
#if UNITY_EDITOR
            if (!UseFileSystem)
            {
                _FileLoader = new DefaultFileLoader(FileLoaderUtil.AssetFileRootPath);
            }
            else
            {
                //为了模拟真实环境
                _FileLoader = new FileSystemFileLoader(FileLoaderUtil.AssetFileRootPath,"");
            }
#else
            _FileLoader = new FileSystemFileLoader(Application.persistentDataPath,Application.streamingAssetsPath);
#endif
            FileLoaderUtil.FileLoader = FileLoader;
        }
    }
}