/*
* 文件名：TestDependency
* 文件描述：用于测试资源依赖
* 作者：aronliang
* 创建时间：2023/05/19 15:49:23
* 修改记录：
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace GameLogic
{
  
    
    public class TestDependency
    {
        //必须是以Assets开头，且必须有后缀，如 Assets/GameMain/UI/UIForms/SettingForm.prefab
        public static string assetName = @"Assets/GameMain/UI/UIForms/SettingForm.prefab";
       
        [MenuItem("Tools/ShowDependency")]
        private static void ShowDependency()
        {
            string[] dependencyAssets = AssetDatabase.GetDependencies(assetName, false);
            foreach (var dependencyAsset in dependencyAssets)
            {
                Debug.Log(dependencyAsset);
            }
        }

        //获取当前项目所有脚本路径，如Assets/GameFramework/Scripts/Editor/Inspector/FsmComponentInspector.cs
        private  static void GetAllScripts()
        {
            var scripts = AssetDatabase.FindAssets("t:Script");
            HashSet<string> scriptNames = new HashSet<string>();
            foreach (var scriptGuid in scripts)
            {
                scriptNames.Add(AssetDatabase.GUIDToAssetPath(scriptGuid));
            }
            Debug.Log($"count={scriptNames.Count}");
            foreach (var scriptName in scriptNames)
            {
                Debug.Log(scriptName);
            }
        }

        private static HashSet<Stamp> m_AnalyzedStamps = new HashSet<Stamp>();
        private static Stamp[] m_Stamps;

        private static void AddTestStamp()
        {
            m_AnalyzedStamps.Add(new Stamp("a", "b")); //a依赖b
            m_AnalyzedStamps.Add(new Stamp("b", "a")); //b依赖a ，构成循环依赖
            m_Stamps = m_AnalyzedStamps.ToArray();
        }
        //检测循环依赖
        private static void CheckCircleDependency()
        {
            AddTestStamp();
            //所有主资源
            HashSet<string> hosts = new HashSet<string>();
            foreach (var stamp in m_Stamps)
            {
                hosts.Add(stamp.HostAssetName);
            }

            //检查循环依赖路径
            List<string[]> results = new List<string[]>();
            foreach (var host in hosts)
            {
                LinkedList<string> route = new LinkedList<string>();    //依赖路径
                HashSet<string> visited = new HashSet<string>();        //已经访问过的资源名
                if (CheckDependency(host, route, visited))
                {
                    results.Add(route.ToArray());
                }
            }
            
            //显示循环依赖路径
            //显示结果：a/b/a 和 b/a/b
            foreach (var result in results)
            {
                string path = "";
                bool first = true;
                foreach (var item in result)
                {
                    if (!first)
                    {
                        path += "/";
                    }
                    path += item;
                    first = false;
                }
                Debug.Log("循环依赖路径：" + path);
            }
        }

        private static bool CheckDependency(string host, LinkedList<string> route, HashSet<string> visited)
        {
            visited.Add(host);
            route.AddLast(host);

            foreach (var stamp in m_Stamps)
            {
                if (stamp.HostAssetName != host)
                {
                    continue;
                }

                if (visited.Contains(stamp.DependencyAssetName))
                {
                    route.AddLast(stamp.DependencyAssetName);
                    return true;
                }

                if (CheckDependency(stamp.DependencyAssetName, route, visited))
                {
                    return true;
                }
            }
            
            route.RemoveLast();
            visited.Remove(host);
            return false;
        }
        
        
        [StructLayout(LayoutKind.Auto)]
        private struct Stamp
        {
            private readonly string m_HostAssetName;
            private readonly string m_DependencyAssetName;

            public Stamp(string hostAssetName, string dependencyAssetName)
            {
                m_HostAssetName = hostAssetName;
                m_DependencyAssetName = dependencyAssetName;
            }

            public string HostAssetName
            {
                get
                {
                    return m_HostAssetName;
                }
            }

            public string DependencyAssetName
            {
                get
                {
                    return m_DependencyAssetName;
                }
            }
        }
    }
}