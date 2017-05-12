using GameFramework;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.AssetBundleTools;

namespace StarForce.Editor
{
    public sealed class StarForceBuildEventHandler : IBuildEventHandler
    {
        public void PreProcessBuildAll(string productName, string companyName, string gameIdentifier,
            string applicableGameVersion, int internalResourceVersion, string unityVersion, BuildAssetBundleOptions buildOptions, bool zip,
            string outputDirectory, string workingPath, string outputPackagePath, string outputFullPath, string outputPackedPath, string buildReportPath)
        {
            string streamingAssetsPath = Utility.Path.GetCombinePath(Application.dataPath, "StreamingAssets");
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

        public void PostProcessBuildAll(string productName, string companyName, string gameIdentifier,
            string applicableGameVersion, int internalResourceVersion, string unityVersion, BuildAssetBundleOptions buildOptions, bool zip,
            string outputDirectory, string workingPath, string outputPackagePath, string outputFullPath, string outputPackedPath, string buildReportPath)
        {

        }

        public void PreProcessBuild(BuildTarget buildTarget, string workingPath, string outputPackagePath, string outputFullPath, string outputPackedPath)
        {

        }

        public void PostProcessBuild(BuildTarget buildTarget, string workingPath, string outputPackagePath, string outputFullPath, string outputPackedPath)
        {
            if (buildTarget != BuildTarget.StandaloneWindows)
            {
                return;
            }

            string streamingAssetsPath = Utility.Path.GetCombinePath(Application.dataPath, "StreamingAssets");
            string[] fileNames = Directory.GetFiles(outputPackagePath, "*", SearchOption.AllDirectories);
            foreach (string fileName in fileNames)
            {
                string destFileName = Utility.Path.GetCombinePath(streamingAssetsPath, fileName.Substring(outputPackagePath.Length));
                FileInfo destFileInfo = new FileInfo(destFileName);
                if (!destFileInfo.Directory.Exists)
                {
                    destFileInfo.Directory.Create();
                }

                File.Copy(fileName, destFileName);
            }
        }
    }
}
