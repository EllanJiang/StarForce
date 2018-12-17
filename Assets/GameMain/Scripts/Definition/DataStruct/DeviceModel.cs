//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System;
using UnityEngine;

namespace StarForce
{
    [Serializable]
    public class DeviceModel
    {
        [SerializeField]
        private string m_DeviceName = null;

        [SerializeField]
        private string m_ModelName = null;

        [SerializeField]
        private QualityLevelType m_QualityLevel = QualityLevelType.Fastest;

        public string DeviceName
        {
            get
            {
                return m_DeviceName;
            }
        }

        public string ModelName
        {
            get
            {
                return m_ModelName;
            }
        }

        public QualityLevelType QualityLevel
        {
            get
            {
                return m_QualityLevel;
            }
        }
    }
}
