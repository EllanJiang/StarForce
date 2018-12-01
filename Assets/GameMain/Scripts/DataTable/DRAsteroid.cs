//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework.DataTable;

namespace StarForce
{
    /// <summary>
    /// 小行星表。
    /// </summary>
    public class DRAsteroid : IDataRow
    {
        /// <summary>
        /// 小行星编号。
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// 最大生命。
        /// </summary>
        public int MaxHP
        {
            get;
            private set;
        }

        /// <summary>
        /// 冲击力。
        /// </summary>
        public int Attack
        {
            get;
            private set;
        }

        /// <summary>
        /// 速度。
        /// </summary>
        public float Speed
        {
            get;
            private set;
        }

        /// <summary>
        /// 角速度。
        /// </summary>
        public float AngularSpeed
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

        public void ParseDataRow(string dataRowText)
        {
            string[] text = DataTableExtension.SplitDataRow(dataRowText);
            int index = 0;
            index++;
            Id = int.Parse(text[index++]);
            index++;
            MaxHP = int.Parse(text[index++]);
            Attack = int.Parse(text[index++]);
            Speed = float.Parse(text[index++]);
            AngularSpeed = float.Parse(text[index++]);
            DeadEffectId = int.Parse(text[index++]);
            DeadSoundId = int.Parse(text[index++]);
        }
    }
}
