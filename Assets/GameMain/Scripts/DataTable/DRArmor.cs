using GameFramework.DataTable;
using System.Collections.Generic;

namespace StarForce
{
    /// <summary>
    /// 装甲表。
    /// </summary>
    public class DRArmor : IDataRow
    {
        /// <summary>
        /// 装甲编号。
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
        /// 防御力。
        /// </summary>
        public int Defense
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
            Defense = int.Parse(text[index++]);
        }

        private void AvoidJIT()
        {
            new Dictionary<int, DRArmor>();
        }
    }
}
