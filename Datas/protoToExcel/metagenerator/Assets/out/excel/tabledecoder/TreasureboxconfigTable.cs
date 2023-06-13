using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class TreasureboxconfigTable : BaseTable
	{
		private Dictionary<uint, TreasureBoxConfig> treasureboxconfig_map = new Dictionary<uint, TreasureBoxConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				TreasureBoxConfig_Array treasureboxconfig_array = TreasureBoxConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in treasureboxconfig_array.Items)
				{
					treasureboxconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "TreasureBoxConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, TreasureBoxConfig> GetTable()
		{
			return treasureboxconfig_map;
		}

		public TreasureBoxConfig GetRecorder(uint key)
		{
			if (!treasureboxconfig_map.ContainsKey(key))
			{
				return null;
			}
			return treasureboxconfig_map[key];
		}

	}
}