/*
* 文件名：FixBurstCompiler
* 文件描述：
* 作者：aronliang
* 创建时间：2023/05/24 17:23:59
* 修改记录：
*/

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

/// <summary>
/// Burst编译找不到 android_ndk_root的bug修改
/// </summary>
public class FixBurstCompiler : IPreprocessBuildWithReport
{
    public int callbackOrder { get; }
    public void OnPreprocessBuild(BuildReport report)
    {
#if UNITY_ANDROID
        EditorPrefs.SetString("AndroidNdkRoot",UnityEditor.Android.AndroidExternalToolsSettings.ndkRootPath);
#endif
    }
}
#endif