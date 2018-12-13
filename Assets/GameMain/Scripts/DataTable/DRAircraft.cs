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
    /// 战机表。
    /// </summary>
    public class DRAircraft : DataRowBase
    {
        private const int MaxWeaponCount = 3; // 最大武器数量
        private const int MaxArmorCount = 3; // 最大装甲数量

        private int m_Id = 0;
        private int[] m_WeaponIds = new int[MaxWeaponCount];
        private int[] m_ArmorIds = new int[MaxArmorCount];

        /// <summary>
        /// 战机编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 推进器编号。
        /// </summary>
        public int ThrusterId
        {
            get;
            private set;
        }

        /// <summary>
        /// 死亡特效编号。
        /// </summary>
        public int DeadEffectId
        {
            get;
            private set;
        }

        /// <summary>
        /// 死亡声音编号。
        /// </summary>
        public int DeadSoundId
        {
            get;
            private set;
        }

        public int GetWeaponIds(int index)
        {
            return index < m_WeaponIds.Length ? m_WeaponIds[index] : 0;
        }

        public int GetArmorIds(int index)
        {
            return index < m_ArmorIds.Length ? m_ArmorIds[index] : 0;
        }

        public override bool ParseDataRow(GameFrameworkSegment<string> dataRowSegment)
        {
            string[] text = DataTableExtension.SplitDataRow(dataRowSegment);
            int index = 0;
            index++;
            m_Id = int.Parse(text[index++]);
            index++;
            ThrusterId = int.Parse(text[index++]);
            for (int i = 0; i < MaxWeaponCount; i++)
            {
                m_WeaponIds[i] = int.Parse(text[index++]);
            }
            for (int i = 0; i < MaxArmorCount; i++)
            {
                m_ArmorIds[i] = int.Parse(text[index++]);
            }
            DeadEffectId = int.Parse(text[index++]);
            DeadSoundId = int.Parse(text[index++]);

            return true;
        }
    }
}
