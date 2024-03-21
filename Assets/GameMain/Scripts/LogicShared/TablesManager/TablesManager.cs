/*
* 文件名：TablesManager
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/15 15:46:21
* 修改记录：
*/

using System;
using Bright.Serialization;
using LogicShared.FileLoader;

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
                if (_tableLoader == null)
                {
                    _tableLoader = (file) =>
                    {
                        var filePath = $"DataTables/LubanByteDatas/{file}.bytes";
                        return new ByteBuf(FileLoaderUtil.ReadStreamingFile(filePath));
                    };
                }
                return _tableLoader;
            }

            set
            {
                _tableLoader = value;
            }
        }

    }
}