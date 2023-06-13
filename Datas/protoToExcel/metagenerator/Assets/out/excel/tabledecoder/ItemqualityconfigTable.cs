using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ItemqualityconfigTable : BaseTable
	{
		private Dictionary<uint, ItemQualityConfig> itemqualityconfig_map = new Dictionary<uint, ItemQualityConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ItemQualityConfig_Array itemqualityconfig_array = ItemQualityConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in itemqualityconfig_array.Items)
				{
					itemqualityconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ItemQualityConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ItemQualityConfig> GetTable()
		{
			return itemqualityconfig_map;
		}

		public ItemQualityConfig GetRecorder(uint key)
		{
			if (!itemqualityconfig_map.ContainsKey(key))
			{
				return null;
			}
			return itemqualityconfig_map[key];
		}

	}
}