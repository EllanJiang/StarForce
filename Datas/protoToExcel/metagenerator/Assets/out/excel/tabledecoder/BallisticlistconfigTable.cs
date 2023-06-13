using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BallisticlistconfigTable : BaseTable
	{
		private Dictionary<uint, BallisticListConfig> ballisticlistconfig_map = new Dictionary<uint, BallisticListConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BallisticListConfig_Array ballisticlistconfig_array = BallisticListConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in ballisticlistconfig_array.Items)
				{
					ballisticlistconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BallisticListConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BallisticListConfig> GetTable()
		{
			return ballisticlistconfig_map;
		}

		public BallisticListConfig GetRecorder(uint key)
		{
			if (!ballisticlistconfig_map.ContainsKey(key))
			{
				return null;
			}
			return ballisticlistconfig_map[key];
		}

	}
}