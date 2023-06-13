using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BritemslotconfigTable : BaseTable
	{
		private Dictionary<uint, BRItemSlotConfig> britemslotconfig_map = new Dictionary<uint, BRItemSlotConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BRItemSlotConfig_Array britemslotconfig_array = BRItemSlotConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in britemslotconfig_array.Items)
				{
					britemslotconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BRItemSlotConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BRItemSlotConfig> GetTable()
		{
			return britemslotconfig_map;
		}

		public BRItemSlotConfig GetRecorder(uint key)
		{
			if (!britemslotconfig_map.ContainsKey(key))
			{
				return null;
			}
			return britemslotconfig_map[key];
		}

	}
}