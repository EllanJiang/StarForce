/*
* 文件名：TablesManager
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/15 15:46:21
* 修改记录：
*/

using System;
using Bright.Serialization;

namespace cfg.luban
{
    public class TablesManager
    {
        private static cfg.Tables _tables;

        public static cfg.Tables Tables
        {
            get
            {
                LoadTables();
                return _tables;
            }
        }

        private static void LoadTables()
        {
            if (_tables == null)
            {
                _tables = new Tables(TableLoader);
            }
        }

        private static Func<string, ByteBuf> _tableLoader;
        public static Func<string, ByteBuf> TableLoader
        {
            get
            {
#if UNITY_EDITOR
                // todo 运行时需要重新设置loader
                _tableLoader = (file) =>
                {
                    var fullPath = $"Assets/GameMain/DataTables/LubanByteDatas/{file}.bytes";
                    return new ByteBuf(System.IO.File.ReadAllBytes(fullPath));
                };
#endif
                return _tableLoader;
            }

            set
            {
                _tableLoader = value;
            }
        }

    }
}