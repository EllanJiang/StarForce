using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BattlemoneymatcheventconfigTable : BaseTable
	{
		private Dictionary<uint, BattleMoneyMatchEventConfig> battlemoneymatcheventconfig_map = new Dictionary<uint, BattleMoneyMatchEventConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BattleMoneyMatchEventConfig_Array battlemoneymatcheventconfig_array = BattleMoneyMatchEventConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in battlemoneymatcheventconfig_array.Items)
				{
					battlemoneymatcheventconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BattleMoneyMatchEventConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BattleMoneyMatchEventConfig> GetTable()
		{
			return battlemoneymatcheventconfig_map;
		}

		public BattleMoneyMatchEventConfig GetRecorder(uint key)
		{
			if (!battlemoneymatcheventconfig_map.ContainsKey(key))
			{
				return null;
			}
			return battlemoneymatcheventconfig_map[key];
		}

	}
}