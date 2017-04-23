using GameFramework.DataTable;
using System.Collections.Generic;

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

        private void AvoidJIT()
        {
            new Dictionary<int, DRThruster>();
        }
    }
}
