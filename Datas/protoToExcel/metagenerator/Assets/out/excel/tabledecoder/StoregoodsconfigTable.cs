using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class StoregoodsconfigTable : BaseTable
	{
		private Dictionary<uint, StoreGoodsConfig> storegoodsconfig_map = new Dictionary<uint, StoreGoodsConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				StoreGoodsConfig_Array storegoodsconfig_array = StoreGoodsConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in storegoodsconfig_array.Items)
				{
					storegoodsconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "StoreGoodsConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, StoreGoodsConfig> GetTable()
		{
			return storegoodsconfig_map;
		}

		public StoreGoodsConfig GetRecorder(uint key)
		{
			if (!storegoodsconfig_map.ContainsKey(key))
			{
				return null;
			}
			return storegoodsconfig_map[key];
		}

	}
}