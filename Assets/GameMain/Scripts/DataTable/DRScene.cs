//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework.DataTable;
using System.Collections.Generic;

namespace StarForce
{
    /// <summary>
    /// 场景配置表。
    /// </summary>
    public class DRScene : IDataRow
    {
        /// <summary>
        /// 场景编号。
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
        /// 背景音乐编号。
        /// </summary>
        public int BackgroundMusicId
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
            BackgroundMusicId = int.Parse(text[index++]);
        }
    }
}
