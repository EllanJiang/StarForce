/*
* 文件名：TableToolWindow
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/15 15:57:03
* 修改记录：
*/

using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;

namespace Editor.TableTools
{
    public class TableToolWindow:EditorWindow
    {
        private static string ConfigPath;
        //shift+o 快捷键
        [MenuItem("Tools/转表 #o")]
        static void Init()
        {
            ConfigPath = Application.dataPath + "/../Datas/Configs/";
            var window = GetWindow<TableToolWindow>("LuBan转表");
            window.minSize = new Vector2(400, 300);
        }

        private void OnGUI()
        {
            //if (GUILayout.Button(new Rect(new Vector2(0, 0), new Vector2(400, 50)),"打开表格所在目录")) ;
            GUILayout.BeginVertical();
            if (GUILayout.Button("打开表格所在目录",GUILayout.Width(400),GUILayout.Height(50)))
            {
                Application.OpenURL(ConfigPath +"/Datas");
            }
            
            if (GUILayout.Button("转表",GUILayout.Width(400),GUILayout.Height(50)))
            {
                //打开命令行工具
                var processStartInfo = new ProcessStartInfo("cmd.exe");
                processStartInfo.WorkingDirectory = ConfigPath;
                processStartInfo.FileName = "genByte.bat";
                processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                var process = Process.Start(processStartInfo);
                
                AssetDatabase.Refresh();
            }
            GUILayout.EndVertical();
        }
    }
}