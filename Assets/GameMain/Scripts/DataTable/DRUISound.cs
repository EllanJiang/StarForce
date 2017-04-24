using GameFramework.DataTable;
using System.Collections.Generic;

namespace StarForce
{
    /// <summary>
    /// 界面声音配置表。
    /// </summary>
    public class DRUISound : IDataRow
    {
        /// <summary>
        /// 界面声音编号。
        /// </summary>
        public int Id
        {
            get;
            protected set;
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

        public void ParseDataRow(string dataRowText)
        {
            string[] text = DataTableExtension.SplitDataRow(dataRowText);
            int index = 0;
            index++;
            Id = int.Parse(text[index++]);
            index++;
            AssetName = text[index++];
            Priority = int.Parse(text[index++]);
            Volume = float.Parse(text[index++]);
        }

        private void AvoidJIT()
        {
            new Dictionary<int, DRUISound>();
        }
    }
}
