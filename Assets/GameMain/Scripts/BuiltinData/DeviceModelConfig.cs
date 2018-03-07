using System.Collections.Generic;
using UnityEngine;

namespace StarForce
{
    public class DeviceModelConfig : ScriptableObject
    {
        [SerializeField]
        private List<DeviceModel> m_DeviceModels = null;

        public DeviceModel[] GetDeviceModels()
        {
            return m_DeviceModels.ToArray();
        }

        public void NewDeviceModel()
        {
            m_DeviceModels.Add(new DeviceModel());
        }

        public void RemoveDeviceModelAt(int index)
        {
            m_DeviceModels.RemoveAt(index);
        }

        public QualityLevelType GetDefaultQualityLevel()
        {
            string modelName = SystemInfo.deviceModel;
            for (int i = 0; i < m_DeviceModels.Count; i++)
            {
                if (m_DeviceModels[i].ModelName == modelName)
                {
                    return m_DeviceModels[i].QualityLevel;
                }
            }

            return QualityLevelType.Fastest;
        }
    }
}
