using UnityEditor;
using UnityEngine;

namespace StarForce.Editor
{
    public static class ProjectSaver
    {
        /// <summary>
        /// 存储可序列化的资源。
        /// </summary>
        /// <remarks>等同于执行 Unity 菜单 File/Save Project。</remarks>
        [MenuItem("Star Force/Save Assets &s")]
        public static void SaveAssets()
        {
#if UNITY_5_5_OR_NEWER
            AssetDatabase.SaveAssets();
#else
            EditorApplication.SaveAssets(); 
#endif
            Debug.Log("You have saved the serializable assets in the project.");
        }
    }
}
