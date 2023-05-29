/*
* 文件名：AGameBuildEventHandler
* 文件描述：自定义打包事件处理器
* 作者：aronliang
* 创建时间：2023/05/24 16:57:33
* 修改记录：
*/

using System.IO;
using GameFramework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

public class AGameBuildEventHandler : IBuildEventHandler
{
    private int internalResourceVersion;
    private string outputDirectory;
    private string applicableGameVersion;
    public bool ContinueOnFailure => true;

    public void OnPreprocessAllPlatforms(string productName, string companyName, string gameIdentifier,
        string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion,
        Platform platforms, AssetBundleCompressionType assetBundleCompression, string compressionHelperTypeName,
        bool additionalCompressionSelected, bool forceRebuildAssetBundleSelected, string buildEventHandlerTypeName,
        string outputDirectory, BuildAssetBundleOptions buildAssetBundleOptions, string workingPath,
        bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath,
        bool outputPackedSelected, string outputPackedPath, string buildReportPath)
    {
        this.internalResourceVersion = internalResourceVersion;
        this.outputDirectory = outputDirectory;
        this.applicableGameVersion = applicableGameVersion;
    }

    public void OnPreprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath,
        bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath)
    {

    }

    public void OnBuildAssetBundlesComplete(Platform platform, string workingPath, bool outputPackageSelected,
        string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected,
        string outputPackedPath, AssetBundleManifest assetBundleManifest)
    {
        
    }

    public void OnOutputUpdatableVersionListData(Platform platform, string versionListPath, int versionListLength,
        int versionListHashCode, int versionListCompressedLength, int versionListCompressedHashCode)
    {
        var dirPath = $"{outputDirectory}/testServer_{platform}/appVersion";
        Debug.LogError("----------------------:"+dirPath);
        if(Directory.Exists(dirPath))
            Directory.Delete(dirPath,true);
        Directory.CreateDirectory(dirPath);
        Debug.LogError("innn "+internalResourceVersion);
        WriteVersionDat($"{dirPath}/version.bat",internalResourceVersion, versionListLength,versionListHashCode,versionListCompressedLength,versionListCompressedHashCode);
    }

    public void OnPostprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath,
        bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, bool isSuccess)
    {
        switch (platform)
        {
            case Platform.Android:
            case Platform.IOS:
            case Platform.Windows64:
            case Platform.Windows:
                // var path = $"{outputDirectory}/testServer_{platform}/appResources";
                // if (System.IO.Directory.Exists(path))
                // {
                //     Directory.Delete(path, true);
                // }
                //
                // //Debug.LogError("uuuuuuuuuuuuuuuuuuuuuuuuu:" + outputFullPath);
                // var files = Directory.GetFiles(outputFullPath);
                //
                // foreach (var file in files)
                // {
                //     var regularName = Utility.Path.GetRegularPath(file);
                //     var abFileName = regularName.Substring(file.LastIndexOf('/') + 1);
                //    // Debug.LogError($"filenameeeeeeee:{file}");
                //     if (abFileName.StartsWith("GameFrameworkVersion"))
                //     {
                //         File.Copy(file,$"{Application.streamingAssetsPath}/GameFrameworkVersion.dat",true);
                //         break;
                //     }
                // }
                //
                // FileUtil.CopyFileOrDirectory(outputFullPath, path);
                //
                // Debug.LogError($"path1:{outputFullPath}  path2:{outputPackagePath}");
                // //把生成的AB复制到StreamingAssets目录下，为打包做准备
                // CopyDirectoryRecurse(outputPackedPath, Application.streamingAssetsPath);
                //     
                // Debug.Log($"资源包转移成功:from {outputPackedPath} to {Application.streamingAssetsPath}");
                
                string streamingAssetsPath = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "StreamingAssets"));
                DirectoryInfo directoryInfo = new DirectoryInfo(streamingAssetsPath);
                directoryInfo.Delete(true);
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
                
                break;
            default:
                break;
        }
    }

    public void OnPostprocessAllPlatforms(string productName, string companyName, string gameIdentifier,
        string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion,
        Platform platforms, AssetBundleCompressionType assetBundleCompression, string compressionHelperTypeName,
        bool additionalCompressionSelected, bool forceRebuildAssetBundleSelected, string buildEventHandlerTypeName,
        string outputDirectory, BuildAssetBundleOptions buildAssetBundleOptions, string workingPath,
        bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath,
        bool outputPackedSelected, string outputPackedPath, string buildReportPath)
    {
        Debug.LogError(111);
    }
    
    void WriteVersionDat(string writePath,int resourceVersion, int listLength, int listHash, int compressedListLength, int compressedListHash)
    {
        VersionInfo info = new VersionInfo()
        {
            ForceGameUpdate = false,
            LatestGameVersion = "",
            InternalGameVersion = 0,
            InternalResourceVersion = resourceVersion,
            GameUpdateUrl = "",
            VersionListLength = listLength,
            VersionListHashCode = listHash,
            VersionListCompressedLength = compressedListLength,
            VersionListCompressedHashCode = compressedListHash
        };

        var json = JsonUtility.ToJson(info);
        if(File.Exists(writePath))
            File.Delete(writePath);

        using (FileStream fileStream = new FileStream(writePath, FileMode.Create, FileAccess.Write))
        {
            using (BinaryWriter stream = new BinaryWriter(fileStream, System.Text.Encoding.UTF8))
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(json);
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
    
    private void CopyDirectoryRecurse(string sourcePath,string targetPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(sourcePath);
            FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();
            foreach (var file in fileInfo)
            {
                if(file is DirectoryInfo)
                    continue;
                File.Copy(file.FullName,$"{targetPath}/{file.Name}",true);
            }
        }
        catch (System.Exception e)
        {
            System.Console.WriteLine(e);
            throw;
        }
    }
}