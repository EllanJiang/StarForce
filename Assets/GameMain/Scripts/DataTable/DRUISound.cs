//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using UnityGameFramework.Runtime;

namespace StarForce
{
    /// <summary>
    /// 界面声音配置表。
    /// </summary>
    public class DRUISound : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 界面声音编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 资源名称。
        /// </summary>
        public string AssetName
        {
            get;
            private set;
        }

        /// <summary>
        /// 优先级。
        /// </summary>
        public int Priority
        {
            get;
            private set;
        }

        /// <summary>
        /// 音量。
        /// </summary>
        public float Volume
        {
            get;
            private set;
        }

        public override bool ParseDataRow(GameFrameworkSegment<string> dataRowSegment)
        {
            string[] text = DataTableExtension.SplitDataRow(dataRowSegment);
            int index = 0;
            index++;
            m_Id = int.Parse(text[index++]);
            index++;
            AssetName = text[index++];
            Priority = int.Parse(text[index++]);
            Volume = float.Parse(text[index++]);

            return true;
        }
    }
}
