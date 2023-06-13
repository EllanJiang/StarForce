using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BritemconfigTable : BaseTable
	{
		private Dictionary<uint, BRItemConfig> britemconfig_map = new Dictionary<uint, BRItemConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BRItemConfig_Array britemconfig_array = BRItemConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in britemconfig_array.Items)
				{
					britemconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BRItemConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BRItemConfig> GetTable()
		{
			return britemconfig_map;
		}

		public BRItemConfig GetRecorder(uint key)
		{
			if (!britemconfig_map.ContainsKey(key))
			{
				return null;
			}
			return britemconfig_map[key];
		}

	}
}