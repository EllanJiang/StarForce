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
    /// 推进器表。
    /// </summary>
    public class DRThruster : IDataRow
    {
        /// <summary>
        /// 推进器编号。
        /// </summary>
        public int Id
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

        public void ParseDataRow(string dataRowText)
        {
            string[] text = DataTableExtension.SplitDataRow(dataRowText);
            int index = 0;
            index++;
            Id = int.Parse(text[index++]);
            index++;
            Speed = float.Parse(text[index++]);
        }
    }
}
