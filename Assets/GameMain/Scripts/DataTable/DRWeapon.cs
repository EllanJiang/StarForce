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
    /// 武器表。
    /// </summary>
    public class DRWeapon : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 武器编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 攻击力。
        /// </summary>
        public int Attack
        {
            get;
            private set;
        }

        /// <summary>
        /// 攻击间隔。
        /// </summary>
        public float AttackInterval
        {
            get;
            private set;
        }

        /// <summary>
        /// 子弹编号。
        /// </summary>
        public int BulletId
        {
            get;
            private set;
        }

        /// <summary>
        /// 子弹速度。
        /// </summary>
        public float BulletSpeed
        {
            get;
            private set;
        }

        /// <summary>
        /// 子弹声音编号。
        /// </summary>
        public int BulletSoundId
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
            Attack = int.Parse(text[index++]);
            AttackInterval = float.Parse(text[index++]);
            BulletId = int.Parse(text[index++]);
            BulletSpeed = float.Parse(text[index++]);
            BulletSoundId = int.Parse(text[index++]);

            return true;
        }
    }
}
