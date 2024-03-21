/*
* 文件名：buildTool
* 文件描述：出包，打包AB工具类
* 作者：aronliang
* 创建时间：2023/05/24 16:13:26
* 修改记录：
*/

using System;
using System.Diagnostics;
using System.IO;
using GameFramework;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Runtime;
using Debug = UnityEngine.Debug;

public class buildTool
{
    //解决方案路径
    private static string m_resolutionPath = Application.dataPath.Replace("/Assets", "");
    //APP输出路径
    private static string m_appBuildPath = $"{m_resolutionPath}/Build/Application";
    //AB输出路径
    private static string m_abBuildPath = $"{m_resolutionPath}/Build/ResourceAB/";
    
    //打包相关配置
    private static string m_CompanyName = "Demo";
    private static string m_buildVersion = "0.1";
    private static bool m_buildAB = true;
    private static bool m_compressAB = true;
    private static string m_productName = "Demo";//包名
    private static string m_buildOption = "Debug";//debug or release
    private static string m_svnVersion = "svn_none";
    private static bool m_useCcache = false;

    
    //AB相关配置
    private static BuildTarget m_buildTarget = BuildTarget.Android;
    private static BuildOptions m_buildOpt = BuildOptions.None;
    private static Platform m_platforms = Platform.Android;
    
    //版本号相关
    private static string versionText = "";
    class VersionData
    {
        public int android;
        public int ios;
    }
    private static VersionData _versionData;
    
    //资源版本号
    private static int m_resourceVersion
    {
        get
        {
            if (PlayerPrefs.HasKey("ResourceBuildVersion"))
                return PlayerPrefs.GetInt("ResourceBuildVersion");
            PlayerPrefs.SetInt("ResourceBuildVersion", 1);
            return 1;
        }
        set
        {
            PlayerPrefs.SetInt("ResourceBuildVersion", value);
        }
    }
    
    //读取版本号信息
    static void ReadGameVersion()
    {
        m_svnVersion = "848484"; //todo 临时设置，真正打包的时候由命令行参数传进来
        m_buildVersion = "1.1";
        m_CompanyName = "MyGame";
        m_productName = "MyGame_1";
        
        versionText = $"{m_buildVersion}.{m_svnVersion}";
        if (m_buildOption == "Release")
        {
            string filePath = $"{m_resolutionPath}/BuildScripts/Version.txt";
            if (File.Exists(filePath))
            {
                return;
            }
            string allText = File.ReadAllText(filePath);
            _versionData = JsonUtility.FromJson<VersionData>(allText);
            if (m_buildTarget == BuildTarget.Android)
            {
                _versionData.android += 1;
            }
            else if (m_buildTarget == BuildTarget.iOS)
            {
                _versionData.ios += 1;
            }
        }
    }
    //记录版本号
    static void WriteGameVersion()
    {
        if (_versionData != null)
        {
            string filePath = $"{m_resolutionPath}/BuildScripts/Version.txt";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            string allText = JsonUtility.ToJson(_versionData);
            File.WriteAllText(filePath, allText);
        }
    }

    #region 出包命令
    [MenuItem("Tools/Builder/AB-APP(会打AB)/Android")]
    public static void BuildAB_APK()
    {
        m_buildTarget = BuildTarget.Android;
        m_platforms = Platform.Android;
        m_buildOpt = BuildOptions.Development;
        BuildAB(); //先打AB
        ReadGameVersion();   //接着读取游戏版本号
        bool success = BuildApplication();  //最后出包
        if (success)
        {
            WriteGameVersion();
        }
    }

    
    [MenuItem("Tools/Builder/AB-APP(会打AB)/Windows64")]
    public static void BuildAB_Exe64()
    {
        m_buildTarget = BuildTarget.StandaloneWindows64;
        m_platforms = Platform.Windows64;
        m_buildOpt = BuildOptions.Development;
        BuildAB();
        BuildApplication();
    }
    [MenuItem("Tools/Builder/AB-APP(会打AB)/Windows32")]
    public static void BuildAB_Exe32()
    {
        m_buildTarget = BuildTarget.StandaloneWindows;
        m_platforms = Platform.Windows;
        m_buildOpt = BuildOptions.Development;
        BuildAB();
        BuildApplication();
    }

    [MenuItem("Tools/Builder/Application/Android")]
    public static void BuildApk()
    {
        m_buildTarget = BuildTarget.Android;
        BuildApplication();
    }

    [MenuItem("Tools/Builder/SyncTest/LinuxServer")]
    public static void BuildSyncLinuxServer()
    {
        m_buildTarget = BuildTarget.StandaloneLinux64;
        BuildServer();
    }

    [MenuItem("Tools/Builder/SyncTest/Android")]
    public static void BuildSyncApk()
    {
        m_buildTarget = BuildTarget.Android;
        BuildSyncApplication();
    }

    [MenuItem("Tools/Builder/SyncTest/Window")]
    public static void BuildSyncWin()
    {
        m_buildTarget = BuildTarget.StandaloneWindows64;
        m_buildOpt = BuildOptions.Development;
        BuildSyncApplication();
    }

    [MenuItem("Tools/Builder/SyncTest/IOS")]
    public static void BuildSyncIOS()
    {
        m_buildTarget = BuildTarget.iOS;
        BuildSyncApplication();
    }

    [MenuItem("Tools/Builder/SyncTest/Version")]
    public static void BuildSyncVersion()
    {
        BuildLogicVersion();
    }

    #endregion
    
     //打AB
     public static void BuildAB()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        //生成lua预编译文件
        //FileSystemBuilderTools.GenLuaByteFiles();
        
        //0、生成文件系统的文件
        //FileSystemBuilderTools.BuildFileSystem();

        //1、资源分析，在这一步做打包策略
        Debug.Log("Start Build AssetBundle");
        AutoPackAssetbundle.AutoAnalysis();
        
        //2、创建文件夹
        if (Directory.Exists(m_abBuildPath))
        {
            Directory.Delete(m_abBuildPath, true);
            Directory.CreateDirectory(m_abBuildPath);
        }
        else
        {
            Directory.CreateDirectory(m_abBuildPath);
        }

        ResourceBuilderController controller = new ResourceBuilderController();
        controller.CompressionHelperTypeName = typeof(DefaultCompressionHelper).FullName;
        controller.BuildEventHandlerTypeName = typeof(AGameBuildEventHandler).FullName;

        controller.RefreshBuildEventHandler();
        controller.RefreshCompressionHelper();

        controller.AssetBundleCompression = m_compressAB ? AssetBundleCompressionType.LZ4 : AssetBundleCompressionType.Uncompressed;

        controller.ForceRebuildAssetBundleSelected = false;
        
        //3、开始打AB
        StartBuildAB(controller, m_resourceVersion, m_platforms, m_abBuildPath);
        //每次打AB完之后都把资源版本号++
        m_resourceVersion++;

        sw.Stop();
        Debug.Log($"BuildAB time cost: {sw.Elapsed.TotalSeconds}s");

        //3、打AB
        //Run(m_resourceVersion,m_platforms,m_abBuildPath,typeof(AGameBuildEventHandler).FullName);

        // //4、文件拷贝
        // string abPath = $"{m_abBuildPath}/Package/0_1_{m_resourceVersion}/{m_platforms.ToString()}";
        // string abOutPath = Application.streamingAssetsPath;
        // CopyDirectoryRecurse(abPath, abOutPath);
        // Debug.Log("COPY FILE OVER,From:"+abPath+",To:"+abOutPath);
        // Debug.Log("Build AssetBundle Success");
        //
        // //4、文件拷贝
        // string abPath = $"{m_abBuildPath}/Package/0_1_{m_resourceVersion}/{m_platforms.ToString()}";
        // string abOutPath = Application.streamingAssetsPath;
        // CopyDirectoryRecurse(abPath, abOutPath);
    }
     
     private static void StartBuildAB(ResourceBuilderController controller, int internalResourceVersion, Platform platforms, string outputDirectory)
     {
         controller.InternalResourceVersion = internalResourceVersion;
         if (platforms != Platform.Undefined)
         {
             controller.Platforms = platforms;
         }

         if (outputDirectory != null)
         {
             controller.OutputDirectory = outputDirectory;
         }

         if (!controller.IsValidOutputDirectory)
         {
             throw new GameFrameworkException(Utility.Text.Format("Output directory '{0}' is invalid.", controller.OutputDirectory));
         }

         //调用buildController接口开始构建资源
         if (!controller.BuildResources())
         {
             throw new GameFrameworkException(" error Build resources failure.");
             Debug.Log("Build resources failure.");
         }
         else
         {
             Debug.Log("Build resources success.");
             //构建成功后保存资源
             controller.Save();
         }
     }

     private static void CopyDirectoryRecurse(string sourcePath, string targetPath, string aExcludeExtension)
     {
         try
         {
             DirectoryInfo dir = new DirectoryInfo(sourcePath);
             FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();
             foreach (var file in fileInfo)
             {
                 if (file is DirectoryInfo)
                 {
                     string aTargetPath = targetPath + "/" + file.Name;
                     if (!Directory.Exists(aTargetPath))
                     {
                         Directory.CreateDirectory(aTargetPath);
                     }
                     CopyDirectoryRecurse(sourcePath + "/" + file.Name, aTargetPath, aExcludeExtension);
                 }
                 else if (file.Extension != aExcludeExtension)
                 {
                     File.Copy(file.FullName, $"{targetPath}/{file.Name}", true);
                 }

             }
         }
         catch (Exception e)
         {
             Console.WriteLine(e);
             throw;
         }
     }
     
     #region 出包
     
     public static bool BuildApplication()
     {
         Debug.Log("Start Build Application");
         Stopwatch sw = new Stopwatch();
         sw.Start();
         //1、创建文件夹
         if (!Directory.Exists(m_appBuildPath))
         {
             Directory.CreateDirectory(m_appBuildPath);
         }
         else
         {
             Directory.Delete(m_appBuildPath, true);
             Directory.CreateDirectory(m_appBuildPath);
         }
         
         BuildLogicVersion();

         //2、打包参数设置
         BuildPlayerOptions opt = new BuildPlayerOptions();
         //启动场景
         opt.scenes = new[] { $"{Application.dataPath}/GameMain/Scenes/StarForceLauncher.unity" };
         opt.options = m_buildOpt;
         opt.target = m_buildTarget;
         var pattern = "apk";
         
         if (m_buildTarget == BuildTarget.StandaloneWindows64 || m_buildTarget == BuildTarget.StandaloneWindows)
         {
             pattern = "exe";
         }
         else if (m_buildTarget == BuildTarget.iOS)
         {
             pattern = "ipa";
         }
         opt.locationPathName = $"{m_appBuildPath}/{m_productName}"; //设置包名
         if (m_buildTarget != BuildTarget.iOS)
         {
             opt.locationPathName += $".{pattern}";
         }
         
         PlayerSettings.companyName = m_CompanyName;  //设置公司名
         PlayerSettings.productName = m_productName;  //设置产品名
         PlayerSettings.bundleVersion = "0.1.1.0";//versionText;  //设置版本号

         //3、开始打包
         BuildReport report = BuildPipeline.BuildPlayer(opt);
         BuildSummary summary = report.summary;
         sw.Stop();
         Debug.Log($"BuildApplication time cost: {sw.Elapsed.TotalSeconds}s");
         if (summary.result == BuildResult.Succeeded)
         {
             Debug.Log("BuildApplicationSuccess");
             return true;
         }
         else
         {
             Debug.Log("BuildApplicationFailed");
             throw new Exception("BuildApplicationFailed");
         }

         return false;
     }
     
     //构建逻辑脚本的版本信息
     static void BuildLogicVersion()
     {
         return;
         string aScriptsMD5 = "";
         string aConfigMD5 = "";
         SyncVersionInfo aSyncVersionInfo = new SyncVersionInfo();

         string filePath = $"{Application.dataPath}/FileAssets/Json/LogicVersion.json";

         string logicFramePath = $"{Application.dataPath}/Scripts/LogicFrame";

         string logicSharedPath = $"{Application.dataPath}/Scripts/LogicShared";

         aScriptsMD5 = MD5Tools.GetLogicScriptsMd5(logicFramePath, logicSharedPath);

         string configPath = $"{Application.dataPath}/FileAssets";

         aConfigMD5 = MD5Tools.GetConfigMd5(configPath);

         aSyncVersionInfo.mSciptsVersion = aScriptsMD5;
         aSyncVersionInfo.mConfigVersion = aConfigMD5;

         if (File.Exists(filePath))
         {
             File.Delete(filePath);
         }
         string allText = JsonUtility.ToJson(aSyncVersionInfo);
         File.WriteAllText(filePath, allText);

         UnityEngine.Debug.Log($"LogicWorld Scripts MD5 = {aScriptsMD5} & Configs MD5 = {aConfigMD5}  Create Success");
     }
     
    public static bool BuildSyncApplication()
    {
        Debug.Log("Start Build SyncApplication");
        Stopwatch sw = new Stopwatch();
        sw.Start();
        string aBuildPath = $"{m_resolutionPath}/Build/SyncApplication";
        //1、创建文件夹
        if (!Directory.Exists(aBuildPath))
        {
            Directory.CreateDirectory(aBuildPath);
        }
        else
        {
            Directory.Delete(aBuildPath, true);
            Directory.CreateDirectory(aBuildPath);
        }

        string aSourceFileRoot = Application.dataPath + "/FileAssets";
        string aTargetFileRoot = Application.dataPath + "/StreamingAssets/FileAssets";
        if (!Directory.Exists(aTargetFileRoot))
            Directory.CreateDirectory(aTargetFileRoot);

        BuildLogicVersion();

        CopyDirectoryRecurse(aSourceFileRoot, aTargetFileRoot, "");

        //2、打包参数设置
        BuildPlayerOptions opt = new BuildPlayerOptions();
        opt.scenes = new[] { $"{Application.dataPath}/GameMain/Scenes/Test/SyncTestClient.unity" };
        opt.options = m_buildOpt;

        opt.target = m_buildTarget;
        var pattern = "apk";

        if (m_buildTarget == BuildTarget.StandaloneWindows64 || m_buildTarget == BuildTarget.StandaloneWindows)
        {
            pattern = "exe";
        }
        if (m_buildTarget == BuildTarget.iOS)
        {
            pattern = "ipa";
        }
        opt.locationPathName = $"{aBuildPath}/{m_productName}";
        if (m_buildTarget != BuildTarget.iOS)
        {
            opt.locationPathName += $".{pattern}";
        }
        PlayerSettings.productName = m_CompanyName;
        PlayerSettings.bundleVersion = "0.1.1.0";

        //3、开始打包
        BuildReport report = BuildPipeline.BuildPlayer(opt);
        BuildSummary summary = report.summary;
        sw.Stop();
        Debug.Log($"BuildSyncApplication time cost: {sw.Elapsed.TotalSeconds}s");
        Directory.Delete(aTargetFileRoot, true);
        File.Delete(aTargetFileRoot + ".meta");
        AssetDatabase.Refresh();
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("BuildSyncApplicationSuccess");
            return true;
        }
        else
        {
            Debug.Log("BuildSyncApplicationFailed");
            throw new Exception("BuildSyncApplicationFailed");
        }
    }

    public static bool BuildServer()
    {
        Debug.Log("Start Build BuildServer");
        Stopwatch sw = new Stopwatch();
        sw.Start();
        string aBuildDir = $"{m_resolutionPath}/BuildServer";
        //1、创建文件夹
        if (!Directory.Exists(aBuildDir))
        {
            Directory.CreateDirectory(aBuildDir);
        }
        else
        {
            Directory.Delete(aBuildDir, true);
            Directory.CreateDirectory(aBuildDir);
        }
        BuildLogicVersion();
        //2、打包参数设置
        BuildPlayerOptions opt = new BuildPlayerOptions();
        opt.scenes = new[] { $"{Application.dataPath}/GameMain/Scenes/Test/SyncTestServer.unity" };
        opt.options = m_buildOpt;

        opt.target = m_buildTarget;
        opt.subtarget = (int)StandaloneBuildSubtarget.Server;

        opt.locationPathName = $"{aBuildDir}/SyncSever.x86_64";

        PlayerSettings.productName = m_CompanyName;
        PlayerSettings.bundleVersion = versionText;

        //3、开始打包
        BuildReport report = BuildPipeline.BuildPlayer(opt);
        BuildSummary summary = report.summary;
        sw.Stop();
        Debug.Log($"BuildLinuxServer time cost: {sw.Elapsed.TotalSeconds}s");
        if (summary.result == BuildResult.Succeeded)
        {
            string aSourceFileRoot = Application.dataPath + "/FileAssets";
            string aTargetFileRoot = aBuildDir + "/SyncSever_Data/StreamingAssets/FileAssets";
            if (!Directory.Exists(aTargetFileRoot))
                Directory.CreateDirectory(aTargetFileRoot);
            CopyDirectoryRecurse(aSourceFileRoot, aTargetFileRoot, ".meta");
            Debug.Log("BuildLinuxServerSuccess");
            return true;
        }
        else
        {
            Debug.Log("BuildLinuxServerFailed");
            throw new Exception("BuildLinuxServerFailed");
        }
    }
     #endregion


     #region IOS

     [PostProcessBuild(999)]
     public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
#if UNITY_IOS
        DisableBitcodeOnIos(buildTarget, path);
#endif
    }

#if UNITY_IOS
    /// <summary>
    /// Disables bitcode compilation on iOS platform.
    /// </summary>
    // From https://support.unity3d.com/hc/en-us/articles/207942813-How-can-I-disable-Bitcode-support-
    private static void DisableBitcodeOnIos(BuildTarget buildTarget, string path)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        //xcode project plist process
        string plistPath = path + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
       
        PlistElementDict rootDict = plist.root;
        PlistElementDict NSAppTransportSecurity = rootDict.CreateDict("NSAppTransportSecurity");
        NSAppTransportSecurity.SetBoolean("NSAllowsArbitraryLoads",true);
        PlistElementDict NSExceptionDomains = NSAppTransportSecurity.CreateDict("NSExceptionDomains");
        PlistElementDict localhostUrl =  NSExceptionDomains.CreateDict ("localhost");
        localhostUrl.SetBoolean ("NSExceptionAllowsInsecureHTTPLoads",true);
       
        File.WriteAllText(plistPath, plist.WriteToString());

        //xcode project file process
        string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

        Debug.Log($"Post Process Build - START: Disable bitcode on IOS\n Project Path:{projectPath}");

        var pbxProject = new PBXProject();
        pbxProject.ReadFromFile(projectPath);

        string target = pbxProject.GetUnityMainTargetGuid();
        pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

        target = pbxProject.GetUnityFrameworkTargetGuid();
        pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
        pbxProject.SetBuildProperty(target, "CODE_SIGNING_ALLOWED", "NO");
        pbxProject.SetBuildProperty(target, "CODE_SIGN_STYLE", "Manual");
        pbxProject.SetBuildProperty(target, "SUPPORTS_MACCATALYST", "NO");
        if(m_useCcache)
        {
            pbxProject.SetBuildProperty(target, "CC", $"{m_resolutionPath}/BuildScripts/ccache-clang");
            pbxProject.SetBuildProperty(target, "CXX", $"{m_resolutionPath}/BuildScripts/ccache-clang++");
        }

        pbxProject.WriteToFile(projectPath);
        Debug.Log("Post Process Build - SUCCESS: Disable bitcode on IOS\n" + 
                  "Bitcode setting in Xcode project is updated.");
    }
#endif

     #endregion

}

//同步版本信息，给服务器用
public class SyncVersionInfo
{
    public string mSciptsVersion;  
    public string mConfigVersion;
}