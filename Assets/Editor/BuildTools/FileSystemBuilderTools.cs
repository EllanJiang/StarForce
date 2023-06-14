/*
* 文件名：FileSystemBuilderTools
* 文件描述：文件系统打包
* 作者：aronliang
* 创建时间：2023/05/24 16:22:58
* 修改记录：
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GameFramework;
using GameFramework.FileSystem;
using UnityEditor;
using UnityEngine;

public class FileSystemBuilderTools
{
    private sealed class FileSystemHelper : IFileSystemHelper
    {
        public FileSystemStream CreateFileSystemStream(string fullPath, FileSystemAccess access, bool createNew)
        {
            return new CommonFileSystemStream(fullPath, access, createNew);
        }
    }

    private class FileSystemInfo
    {
        public string Name;
        public string FullName;
        public List<string> Files = new List<string>();  //文件绝对路径
        public IFileSystem FileSystem;
    }

    private static string FileRootDir = "Assets/FileAssets";
    private static string ExetensionName = ".dat";
    private static string TargetDir = "Assets/GameMain/FileAssets";

    private static string LuaRootDir = "Assets/GameMain/Lua";
    private static string LuaByteRootDir = "Assets/FileAssets/Lua";
#if UNITY_EDITOR_OSX
    private static string LuacPath = "luac";
#else
    private static string LuacPath = "Tools/luac.exe";
#endif
    private const int MaxLuacProcessCount = 16;
    
    [MenuItem("Tools/文件系统/Lua预编译")]
    public static void GenLuaByteFiles()
    {
        if (Directory.Exists(LuaByteRootDir))
            Directory.Delete(LuaByteRootDir, true);

        BuildLuaBytes(LuaRootDir);
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    #region 生成Lua二进制Chunk
    //把.lua文件转成.bytes二进制文件
    private static List<Process> runningProcess = new List<Process>();
    private static void BuildLuaBytes(string path)
    {
        path = Utility.Path.GetRegularPath(path);
        string outDir = path.Replace(LuaRootDir, LuaByteRootDir);
        if (!Directory.Exists(outDir))
        {
            Directory.CreateDirectory(outDir);
        }

        string[] dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
        foreach (var dir in dirs)
        {
            if (dir.EndsWith(".vscode"))
                continue;
            BuildLuaBytes(dir);
        }

        string[] files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);

        int total = files.Length;
        EditorUtility.DisplayProgressBar("提示", $"生成{path}内的Lua二进制中 0/{total}", 0);

        int i = 1;
        foreach (var file in files)
        {
            if(Directory.Exists(file))
                continue;
            var filePath = Utility.Path.GetRegularPath(file);
            if(!IsLuaFile(filePath))
                continue;
            string fileOutPath = filePath.Replace(LuaRootDir, LuaByteRootDir);
            fileOutPath = fileOutPath.Replace(".lua.txt", ".lua");
            if (filePath.EndsWith("agame.lua"))
            {
                //agame.lua是主lua文件，需要一开始就加载，因此不需要转成二进制chunk文件
                File.Copy(filePath,fileOutPath);
                continue;
            }

            EditorUtility.DisplayProgressBar("提示", $"生成{path}内的Lua二进制中  {file} {i++}/{total}", 0);

            Process p = GenALuaFile(fileOutPath,filePath);
            if(p != null)
                runningProcess.Add(p);

            WaitLuacProcessCount(MaxLuacProcessCount);
        }

        WaitLuacProcessCount(0);

    }
    private static Process GenALuaFile(string fileOutPath, string filePath)
    {
        try
        {
            Process p = new Process();
#if UNITY_EDITOR_OSX
            p.StartInfo.FileName = LuacPath;
#else
            p.StartInfo.FileName = GetFullPath(LuacPath);
#endif
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = $"-o {GetFullPath(fileOutPath)} {GetFullPath(filePath)}";

            if (!p.Start())
            {
                p.Close();
                p = null;
            }
            return p;
            //p.WaitForExit();
            //p.Close();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e);
            throw;
        }
    }
    private static void WaitLuacProcessCount(int Count)
    {
        void ClearExitedProcess()
        {
            for (int i = runningProcess.Count - 1; i >= 0; i--)
            {
                var p = runningProcess[i];
                if (p.HasExited)
                {
                    p.Close();
                    runningProcess.RemoveAt(i);
                }
            }
        }

        ClearExitedProcess();

        while (runningProcess.Count > Count)
        {
            ClearExitedProcess();
        }
    }
    private static bool IsLuaFile(string path)
    {
        if (path.EndsWith(".lua") || path.EndsWith(".lua.txt"))
            return true;
        return false;
    }
    
    private static string GetFullPath(string path)
    {
        var newPath = Path.Combine(Environment.CurrentDirectory, path);
        return Utility.Path.GetRegularPath(newPath);
    }
    #endregion

    #region 生成文件系统

    
    [MenuItem("Tools/文件系统/生成文件系统")]
    public static void BuildFileSystem()
    {
        if (!Directory.Exists(FileRootDir))
        {
            UnityEngine.Debug.LogError(FileRootDir + "=null");
            return;
        }

        if (Directory.Exists(TargetDir))
        {
            Directory.Delete(TargetDir,true);
        }
        Directory.CreateDirectory(TargetDir);

        IFileSystemManager m_FileSystemManager = null;
        m_FileSystemManager = GameFrameworkEntry.GetModule<IFileSystemManager>();
        m_FileSystemManager.SetFileSystemHelper(new FileSystemHelper());
        
        string[] dirs = Directory.GetDirectories(FileRootDir);
        float count = dirs.Length;
        float index = 0;
        foreach (var path in dirs)
        {
            index++;
            EditorUtility.DisplayProgressBar("Notice","BuildFileSystems",index / count);
            if(!Directory.Exists(path))
                continue;
            var info = CreateFileSystem(m_FileSystemManager,path);
            WriteFile(info);
        }
        
        AssetDatabase.Refresh();
        GameFrameworkEntry.Shutdown();
        
        EditorUtility.ClearProgressBar();
    }
    
    private static FileSystemInfo CreateFileSystem(IFileSystemManager m_FileSystemManager,string path)
    {
        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        FileSystemInfo info = new FileSystemInfo();
        foreach (var filePath in files)
        {
            var extenName = Path.GetExtension(filePath);
            if(!IsFile(extenName))
                continue;
            info.Files.Add(filePath);
        }
        if(info.Files.Count <= 0)
            return null;

        string dirName = Path.GetFileNameWithoutExtension(path);
        string fileSystemPath = $"{TargetDir}/{dirName}{ExetensionName}";
        var fileSystem = m_FileSystemManager.CreateFileSystem(fileSystemPath, FileSystemAccess.Write,info.Files.Count, info.Files.Count);
        info.Name = dirName;
        info.FullName = fileSystemPath;
        info.FileSystem = fileSystem;
        return info;
    }
    
    private static void WriteFile(FileSystemInfo info)
    {
        if(info.FileSystem == null)
            return;
        if(info.Files.Count == 0)
            return;
        foreach (var filePath in info.Files)
        {
            string assetPath = Utility.Path.GetRegularPath(filePath);
            byte[] bytes = File.ReadAllBytes(assetPath);
            info.FileSystem.WriteFile(assetPath, bytes);
        }
    }

    private static bool IsFile(string extenName)
    {
        if(extenName.Equals(".meta") || extenName.Equals(".prefab"))
            return false;
        return true;
    }
    #endregion
}