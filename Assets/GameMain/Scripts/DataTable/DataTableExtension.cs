using GameFramework;
using System;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public static class DataTableExtension
    {
        private const string DataTableClassPrefixName = "StarForce.DR";
        private static readonly string[] ColumnSplit = new string[] { "\t" };

        public static void LoadDataTable(this DataTableComponent dataTableComponent, string dataTableName, object userData = null)
        {
            if (string.IsNullOrEmpty(dataTableName))
            {
                Log.Warning("Data table name is invalid.");
                return;
            }

            string[] splitNames = dataTableName.Split('_');
            if (splitNames.Length > 2)
            {
                Log.Warning("Data table name is invalid.");
                return;
            }

            string dataTableClassName = DataTableClassPrefixName + splitNames[0];

            Type dataTableType = Type.GetType(dataTableClassName);
            if (dataTableType == null)
            {
                Log.Warning("Can not get data table type with class name '{0}'.", dataTableClassName);
                return;
            }

            string dataTableNameInType = splitNames.Length > 1 ? splitNames[1] : null;
            dataTableComponent.LoadDataTable(dataTableName, dataTableType, dataTableNameInType, AssetUtility.GetDataTableAsset(dataTableName), userData);
        }

        public static string[] SplitDataRow(string dataRowText)
        {
            return dataRowText.Split(ColumnSplit, StringSplitOptions.None);
        }
    }
}
