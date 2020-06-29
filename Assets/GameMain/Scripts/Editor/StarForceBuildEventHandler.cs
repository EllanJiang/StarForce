//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace StarForce.Editor
{
    public sealed class StarForceBuildEventHandler : IBuildEventHandler
    {
        public bool ContinueOnFailure
        {
            get
            {
                return false;
            }
        }

        public void PreprocessAllPlatforms(string productName, string companyName, string gameIdentifier,
            string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion, BuildAssetBundleOptions buildOptions, bool zip,
            string outputDirectory, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, string buildReportPath)
        {
            string streamingAssetsPath = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "StreamingAssets"));
            string[] fileNames = Directory.GetFiles(streamingAssetsPath, "*", SearchOption.AllDirectories);
            foreach (string fileName in fileNames)
            {
                if (fileName.Contains(".gitkeep"))
                {
                    continue;
                }

                File.Delete(fileName);
            }

            Utility.Path.RemoveEmptyDirectory(streamingAssetsPath);
        }

        public void PostprocessAllPlatforms(string productName, string companyName, string gameIdentifier,
            string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion, BuildAssetBundleOptions buildOptions, bool zip,
            string outputDirectory, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, string buildReportPath)
        {
        }

        public void PreprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath)
        {
        }

        public void PostprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, bool isSuccess)
        {
            if (!outputPackageSelected)
            {
                return;
            }

            if (platform != Platform.Windows)
            {
                return;
            }

            string streamingAssetsPath = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "StreamingAssets"));
            string[] fileNames = Directory.GetFiles(outputPackagePath, "*", SearchOption.AllDirectories);
            foreach (string fileName in fileNames)
            {
                string destFileName = Utility.Path.GetRegularPath(Path.Combine(streamingAssetsPath, fileName.Substring(outputPackagePath.Length)));
                FileInfo destFileInfo = new FileInfo(destFileName);
                if (!destFileInfo.Directory.Exists)
                {
                    destFileInfo.Directory.Create();
                }

                File.Copy(fileName, destFileName);
            }
        }

        /// <summary>
        /// 输出某个平台可更新模式版本列表文件的处理事件。
        /// </summary>
        /// <param name="platform">生成平台。</param>
        /// <param name="versionListPath">可更新模式版本列表文件的路径。</param>
        /// <param name="versionListLength">可更新模式版本列表文件的长度。</param>
        /// <param name="versionListHashCode">可更新模式版本列表文件的校验值。</param>
        /// <param name="versionListZipLength">可更新模式版本列表文件压缩后的长度。</param>
        /// <param name="versionListZipHashCode">可更新模式版本列表文件压缩后的校验值。</param>
        public void OutputUpdatableVersionListData(Platform platform, string versionListPath, int versionListLength, int versionListHashCode, int versionListZipLength, int versionListZipHashCode)
        {
        }
    }
}
