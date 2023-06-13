using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrshopconfigTable : BaseTable
	{
		private Dictionary<uint, BRShopConfig> brshopconfig_map = new Dictionary<uint, BRShopConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BRShopConfig_Array brshopconfig_array = BRShopConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in brshopconfig_array.Items)
				{
					brshopconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BRShopConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BRShopConfig> GetTable()
		{
			return brshopconfig_map;
		}

		public BRShopConfig GetRecorder(uint key)
		{
			if (!brshopconfig_map.ContainsKey(key))
			{
				return null;
			}
			return brshopconfig_map[key];
		}

	}
}