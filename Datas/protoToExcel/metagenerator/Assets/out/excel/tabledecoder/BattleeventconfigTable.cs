using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BattleeventconfigTable : BaseTable
	{
		private Dictionary<uint, BattleEventConfig> battleeventconfig_map = new Dictionary<uint, BattleEventConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BattleEventConfig_Array battleeventconfig_array = BattleEventConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in battleeventconfig_array.Items)
				{
					battleeventconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BattleEventConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BattleEventConfig> GetTable()
		{
			return battleeventconfig_map;
		}

		public BattleEventConfig GetRecorder(uint key)
		{
			if (!battleeventconfig_map.ContainsKey(key))
			{
				return null;
			}
			return battleeventconfig_map[key];
		}

	}
}