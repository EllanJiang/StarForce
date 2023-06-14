/*
* 文件名：AutoPackAssetbundle
* 文件描述：自动打AB
* 作者：aronliang
* 创建时间：2023/05/24 16:33:18
* 修改记录：
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFramework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;
public class TempResourceList : ScriptableObject
{
    //在Unity内手动赋值
    public UnityEngine.Object[] TempResources;
}
public static class AutoPackAssetbundle
{
    static ResourceCollection mResourceCollection;
    
    //不打进包里面的文件夹
    private static List<string> notPackedDirs = new List<string>()
    {
        //"Configs", "Lua", "UIAtlas", "UIForms"
    };
    
    //打成二进制的文件夹
    private static List<string> binaryPackedResources = new List<string>()
    {
        "FileAssets",
    };
    
    //不打进包里面的资源GUID
    private static List<string> notPackedGUIDs = new List<string>();
    
    private static void LoadNotPackedGUIDs()
    {
        notPackedGUIDs.Clear();

        TempResourceList tempResourceList = AssetDatabase.LoadAssetAtPath<TempResourceList>("Assets/Settings/TempResourceList.asset");
        if (tempResourceList != null)
        {
            foreach (var r in tempResourceList.TempResources)
            {
                notPackedGUIDs.Add(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetOrScenePath(r)));
            }
        }
    }
    
    // 自动分析资源的依赖，并生成AB的名称
    [MenuItem("Tools/AutoAnalysis")]
    public static void AutoAnalysis()
    {
        mResourceCollection = new ResourceCollection();
        if (!mResourceCollection.Load())
            return;
        var reses = mResourceCollection.GetResources();
        foreach (var res in reses)
        {
            mResourceCollection.RemoveResource(res.Name,res.Variant);
        }
        try
        {
            LoadNotPackedGUIDs();

            //运行时资源:UI预制体，图集，材质，模型，场景，特效，配置文件等
            var dirs = Directory.GetDirectories(Path.Combine(Application.dataPath, "GameMain"), "*", SearchOption.TopDirectoryOnly);

            var progress = 1.0f / dirs.Length;
            float collectProgress = 0;
            foreach (var dir in dirs)
            {
                var tmpDir = Utility.Path.GetRegularPath(dir);
                var dirName = tmpDir.Substring(tmpDir.LastIndexOf('/') + 1);
                //Lua在前面已经处理过了，这里不用再处理
                if (dirName.Equals("Lua") || dirName.Equals(".svn") || dirName.Equals("Scripts"))
                    continue;
                bool notNeedPacked = notPackedDirs.Contains(dirName);
                if (binaryPackedResources.Contains(dirName))
                {
                    //转成二进制
                    AnalysisFilePack(dir);
                }
                else
                {
                    //正常打包
                    AnalysisAndPackRes(dir, !notNeedPacked);
                }

                collectProgress += progress;
                EditorUtility.DisplayProgressBar("资源打包","资源收集中...",collectProgress);
            }
            Debug.Log("analysisGameRes over");
            mResourceCollection.Save();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SignGameAssetForBuild error,msg = {e.Message}");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }
    
    /// <summary>
    /// 处理文件系统的打包
    /// </summary>
    /// <param name="dirPath"></param>
    private static void AnalysisFilePack(string dirPath)
    {
        var files = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories).Where(x =>
        {
            //排除meta文件
            if (x.EndsWith(".meta"))
                return false;
            return true;
        });
        foreach (var item in files)
        {
            var guid = AssetDatabase.AssetPathToGUID(item.Replace(Application.dataPath, "Assets"));

            if (notPackedGUIDs.IndexOf(guid) >= 0)
                continue;

            var resourceName = Path.GetFileNameWithoutExtension(item);
            //打包粒度是以文件夹分组，每个文件夹就是一个AB
            bool success = mResourceCollection.AddResource(resourceName, null, null,
                LoadType.LoadFromBinary, true, null);
            if (!success)
            {
                Debug.Log($"Add resource failed:{resourceName}");
                continue;
            }

            //todo Asset            
            if (!mResourceCollection.AssignAsset(guid, resourceName, null))
            {
                Debug.LogWarning($"assign asset {item} failed");
            }
        }
    }
    
    /// <summary>
    /// 处理其他资源(模型，UI，图集等)的打包
    /// </summary>
    /// <param name="gameDynamicPathName"></param>
    /// <param name="needPacked"></param>
    private static void AnalysisAndPackRes(string gameDynamicPathName,bool needPacked = true)
    {
        var files = System.IO.Directory.GetFiles(gameDynamicPathName, "*", SearchOption.AllDirectories).Where(x =>
        {
            if (x.EndsWith(".meta"))
                return false;
            return true;
        });

        var resourceName = GameFramework.Utility.Path.GetRegularPath(gameDynamicPathName);
        resourceName = resourceName.Substring(resourceName.LastIndexOf('/') + 1);
        
        var result = mResourceCollection.AddResource(resourceName, null, null,
            LoadType.LoadFromFile, needPacked, null);

        Debug.Log($"Add resource :{resourceName},{result}");
        foreach (var item in files)
        {
            var guid = AssetDatabase.AssetPathToGUID(item.Replace(Application.dataPath, "Assets"));

            if (notPackedGUIDs.IndexOf(guid) >= 0)
                continue;
            Debug.Log($"AssignAsset resourceName :{resourceName},AssetGuid: {guid}");
            if (!mResourceCollection.AssignAsset(guid, resourceName, null))
            {
                Debug.LogWarning($"assign asset {item} failed");
            }
        }
    }

}