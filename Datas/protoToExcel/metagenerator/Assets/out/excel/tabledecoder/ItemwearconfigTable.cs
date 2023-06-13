using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ItemwearconfigTable : BaseTable
	{
		private Dictionary<uint, ItemWearConfig> itemwearconfig_map = new Dictionary<uint, ItemWearConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ItemWearConfig_Array itemwearconfig_array = ItemWearConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in itemwearconfig_array.Items)
				{
					itemwearconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ItemWearConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ItemWearConfig> GetTable()
		{
			return itemwearconfig_map;
		}

		public ItemWearConfig GetRecorder(uint key)
		{
			if (!itemwearconfig_map.ContainsKey(key))
			{
				return null;
			}
			return itemwearconfig_map[key];
		}

	}
}