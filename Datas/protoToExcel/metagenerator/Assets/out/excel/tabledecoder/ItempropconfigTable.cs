using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ItempropconfigTable : BaseTable
	{
		private Dictionary<uint, ItemPropConfig> itempropconfig_map = new Dictionary<uint, ItemPropConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ItemPropConfig_Array itempropconfig_array = ItemPropConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in itempropconfig_array.Items)
				{
					itempropconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ItemPropConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ItemPropConfig> GetTable()
		{
			return itempropconfig_map;
		}

		public ItemPropConfig GetRecorder(uint key)
		{
			if (!itempropconfig_map.ContainsKey(key))
			{
				return null;
			}
			return itempropconfig_map[key];
		}

	}
}