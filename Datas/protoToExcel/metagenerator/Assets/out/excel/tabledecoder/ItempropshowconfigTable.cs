using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ItempropshowconfigTable : BaseTable
	{
		private Dictionary<uint, ItemPropShowConfig> itempropshowconfig_map = new Dictionary<uint, ItemPropShowConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ItemPropShowConfig_Array itempropshowconfig_array = ItemPropShowConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in itempropshowconfig_array.Items)
				{
					itempropshowconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ItemPropShowConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ItemPropShowConfig> GetTable()
		{
			return itempropshowconfig_map;
		}

		public ItemPropShowConfig GetRecorder(uint key)
		{
			if (!itempropshowconfig_map.ContainsKey(key))
			{
				return null;
			}
			return itempropshowconfig_map[key];
		}

	}
}