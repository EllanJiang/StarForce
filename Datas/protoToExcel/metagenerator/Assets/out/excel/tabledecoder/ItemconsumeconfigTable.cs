using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ItemconsumeconfigTable : BaseTable
	{
		private Dictionary<uint, ItemConsumeConfig> itemconsumeconfig_map = new Dictionary<uint, ItemConsumeConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ItemConsumeConfig_Array itemconsumeconfig_array = ItemConsumeConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in itemconsumeconfig_array.Items)
				{
					itemconsumeconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ItemConsumeConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ItemConsumeConfig> GetTable()
		{
			return itemconsumeconfig_map;
		}

		public ItemConsumeConfig GetRecorder(uint key)
		{
			if (!itemconsumeconfig_map.ContainsKey(key))
			{
				return null;
			}
			return itemconsumeconfig_map[key];
		}

	}
}