using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StarForce.Editor
{
    public class DeviceModelConfigEditorWindow : EditorWindow
    {
        private DeviceModelConfig m_Config = null;

        [MenuItem("Star Force/Device Model Config Editor")]
        public static void EditDeviceModelConfig()
        {
            const string DeviceModelConfigFullName = "Assets/GameMain/Configs/DeviceModelConfig.asset";
            OpenWindow(AssetDatabase.LoadAssetAtPath<DeviceModelConfig>(DeviceModelConfigFullName));
        }

        public static void OpenWindow(DeviceModelConfig deviceModelConfig)
        {
            if (deviceModelConfig == null)
            {
                return;
            }

            DeviceModelConfigEditorWindow window = GetWindow<DeviceModelConfigEditorWindow>(true, "Device Model Config Editor");
            window.m_Config = deviceModelConfig;
            window.minSize = new Vector2(460f, 400f);
        }

        private void OnGUI()
        {
            if (m_Config == null)
            {
                return;
            }

            OnDeviceModelGUI();
        }

        private Vector2 m_DeviceModelTablePosition = Vector2.zero;
        private FieldInfo m_DeviceNameCellField = typeof(DeviceModel).GetField("m_DeviceName", BindingFlags.NonPublic | BindingFlags.Instance);
        private FieldInfo m_ModelNameCellField = typeof(DeviceModel).GetField("m_ModelName", BindingFlags.NonPublic | BindingFlags.Instance);
        private FieldInfo m_QualityLevelCellField = typeof(DeviceModel).GetField("m_QualityLevel", BindingFlags.NonPublic | BindingFlags.Instance);

        private void OnDeviceModelGUI()
        {
            DeviceModel[] deviceModels = m_Config.GetDeviceModels();

            DrawHeader();

            m_DeviceModelTablePosition = EditorGUILayout.BeginScrollView(m_DeviceModelTablePosition, GUILayout.Width(this.position.width));

            int deleteIndex = -1;

            for (int i = 0; i < deviceModels.Length; i++)
            {
                if (DrawItem(deviceModels[i]))
                {
                    deleteIndex = i;
                }
            }

            if (deleteIndex >= 0)
            {
                m_Config.RemoveDeviceModelAt(deleteIndex);
            }

            if (GUILayout.Button("+", GUILayout.Width(20f)))
            {
                m_Config.NewDeviceModel();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Empty, GUILayout.Width(20f));
            EditorGUILayout.LabelField("Device Name", GUILayout.Width(200f));
            EditorGUILayout.LabelField("Model Name", GUILayout.Width(100f));
            EditorGUILayout.LabelField("Quality Level", GUILayout.Width(100f));
            EditorGUILayout.EndHorizontal();
        }

        private bool DrawItem(DeviceModel row)
        {
            EditorGUILayout.BeginHorizontal();
            bool deleteMe = GUILayout.Button("-", GUILayout.Width(20f), GUILayout.Height(EditorGUIUtility.singleLineHeight));
            DrawTextItem(row, m_DeviceNameCellField, 200f);
            DrawTextItem(row, m_ModelNameCellField, 100f);
            DrawEnumItem(row, m_QualityLevelCellField, 100f);
            EditorGUILayout.EndHorizontal();

            return deleteMe;
        }

        private void DrawTextItem(object obj, FieldInfo field, float width = 300)
        {
            string oldValue = (string)field.GetValue(obj);
            string value = EditorGUILayout.TextField(oldValue, GUILayout.Width(width));
            if (value != oldValue)
            {
                EditorUtility.SetDirty(m_Config);
            }

            field.SetValue(obj, value);
        }

        private void DrawEnumItem(object obj, FieldInfo field, float width = 300)
        {
            Enum oldValue = (Enum)field.GetValue(obj);
            Enum value = EditorGUILayout.EnumPopup(oldValue, GUILayout.Width(width));
            if (value != oldValue)
            {
                EditorUtility.SetDirty(m_Config);
            }

            field.SetValue(obj, value);
        }
    }
}
