using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class DecoyweapondatatableTable : BaseTable
	{
		private Dictionary<uint, DecoyWeaponDataTable> decoyweapondatatable_map = new Dictionary<uint, DecoyWeaponDataTable>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				DecoyWeaponDataTable_Array decoyweapondatatable_array = DecoyWeaponDataTable_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in decoyweapondatatable_array.Items)
				{
					decoyweapondatatable_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "DecoyWeaponDataTable_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, DecoyWeaponDataTable> GetTable()
		{
			return decoyweapondatatable_map;
		}

		public DecoyWeaponDataTable GetRecorder(uint key)
		{
			if (!decoyweapondatatable_map.ContainsKey(key))
			{
				return null;
			}
			return decoyweapondatatable_map[key];
		}

	}
}