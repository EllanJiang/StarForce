/*
* 文件名：TestBuildAB
* 文件描述：
* 作者：aronliang
* 创建时间：2023/05/24 10:19:07
* 修改记录：
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFramework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace GameLogic
{
    public class TestBuildAB
    {
        public static string SettingFormPath = @"Assets/GameMain/UI/UIForms/SettingForm.prefab";
        public static string SettingFormPath2 = @"Assets/GameMain/UI/UIForms/SettingForm2.prefab";
        public static string OutPutPath = @"D:\Learn\Unity_Learn\UGF_StarForce\StarForce\Build\AB3";
        public static string RootPath = Application.dataPath.Replace("Assets", "");

        [MenuItem("Tools/BuildAB")]
        public static void TestBuildDependency()
        {
            BuildAssetBundleOptions bundleOptions = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle;
            
           
            //图集
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            AssetBundleBuild assetBundleBuild3 = new AssetBundleBuild();
            assetBundleBuild3.assetBundleName = "UIAtlas";
            assetBundleBuild3.assetNames = GetAtlas().ToArray();
            builds.Add(assetBundleBuild3);
            
            
            //AssetBundleManifest assetBundleManifest3 = BuildPipeline.BuildAssetBundles(GetPath(OutPutPath3), builds3.ToArray(), bundleOptions, BuildTarget.StandaloneWindows64);
            
            //UI
            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            assetBundleBuild.assetBundleName = "settingui";
            assetBundleBuild.assetNames = new[] { SettingFormPath2 };
            
            builds.Add(assetBundleBuild);
            if (Directory.Exists(OutPutPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(OutPutPath);
                directoryInfo.Delete(true);
            }
            Directory.CreateDirectory(OutPutPath);
            
           
            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(GetPath(OutPutPath), builds.ToArray(), bundleOptions, BuildTarget.StandaloneWindows64);
            foreach (var dependency in assetBundleManifest.GetAllDependencies("settingui"))
            {
                Debug.Log(dependency);
            }

        }

        [MenuItem("Tools/ShowSprites")]
        private static List<string> GetTextures()
        {
            string path = @"Assets\GameMain\UI\UISprites";
            string realPath = GetPath(path);
            realPath = Path.Combine(RootPath, realPath);
            var dirs =  Directory.GetDirectories(realPath);
            List<string> fileAssets = new List<string>();
            foreach (var dir in dirs)
            {
                if (dir.EndsWith(".meta"))
                {
                    continue;
                }
                //Debug.Log(dir);
                GetTextureFiles(dir,fileAssets);
            }
            
            return fileAssets;
        }
        
        private static void GetTextureFiles(string directory,List<string> fileAssets)
        {
            var dirs =  Directory.GetDirectories(directory);
            foreach (var dir in dirs)
            {
                if (dir.EndsWith(".meta"))
                {
                    continue;
                }
                //Debug.Log(dir);
                GetTextureFiles(dir,fileAssets);
            }
            var files = Directory.GetFiles(directory);
            foreach (var file in files)
            {
                if (file.EndsWith(".meta"))
                {
                    continue;
                }
                
                string filePath = GetPath(file.Replace(RootPath, ""));
                Debug.Log(filePath);
                fileAssets.Add(filePath);
            }
        }
        
        [MenuItem("Tools/ShowAtlas")]
        private static List<string> GetAtlas()
        {
            string path = @"Assets\GameMain\UI\UISprites2";
            string realPath = GetPath(path);
            realPath = Path.Combine(RootPath, realPath);
            List<string> fileAssets = new List<string>();
            GetAtlasFiles(realPath,fileAssets);
            var dirs =  Directory.GetDirectories(realPath);
            foreach (var dir in dirs)
            {
                if (dir.EndsWith(".meta"))
                {
                    continue;
                }
                Debug.LogWarning(dir);
                GetTextureFiles(dir,fileAssets);
            }
            return fileAssets;
        }

        private static void GetAtlasFiles(string directory,List<string> fileAssets)
        {
            var dirs =  Directory.GetDirectories(directory);
            foreach (var dir in dirs)
            {
                if (dir.EndsWith(".meta"))
                {
                    continue;
                }
                //Debug.Log(dir);
                GetAtlasFiles(dir,fileAssets);
            }
            var files = Directory.GetFiles(directory);
            foreach (var file in files)
            {
                if (file.EndsWith(".meta"))
                {
                    continue;
                }
                if (!file.EndsWith(".spriteatlas"))
                {
                    continue;
                }

                string filePath = GetPath(file.Replace(RootPath, ""));
                Debug.LogWarning(filePath);
                fileAssets.Add(filePath);
            }
        }

        private static string GetPath(string originPath)
        {
            return originPath.Replace(@"\",@"/" );
        }
        [MenuItem("Tools/AnalysisAndPackRes")]
        private static void AnalysisAndPackRes()
        {
            var dirs = Directory.GetDirectories(Path.Combine(Application.dataPath, "GameMain"), "*", SearchOption.TopDirectoryOnly);

            var progress = 1.0f / dirs.Length;
            float collectProgress = 0;
            foreach (var dir in dirs)
            {
                var tmpDir = Utility.Path.GetRegularPath(dir);
                var dirName = tmpDir.Substring(tmpDir.LastIndexOf('/') + 1);
                //Lua在前面已经处理过了，这里不用再处理
                if (dirName.Equals("Lua") || dirName.Equals(".svn"))
                    continue;
                
                var files = System.IO.Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Where(x =>
                {
                    if (x.EndsWith(".meta"))
                        return false;
                    return true;
                });

                //文件名就是一个AB
                var resourceName = GameFramework.Utility.Path.GetRegularPath(dir);
                resourceName = resourceName.Substring(resourceName.LastIndexOf('/') + 1);
                Debug.Log($"Add resource :{resourceName}");
                foreach (var item in files)
                {
                    var guid = AssetDatabase.AssetPathToGUID(item.Replace(Application.dataPath, "Assets"));
                    Debug.Log($"AssignAsset resourceName :{resourceName},AssetGuid: {guid}");
                }

                collectProgress += progress;
                EditorUtility.DisplayProgressBar("资源打包","资源收集中...",collectProgress);
            }
            
            EditorUtility.ClearProgressBar();
        }
    }
}