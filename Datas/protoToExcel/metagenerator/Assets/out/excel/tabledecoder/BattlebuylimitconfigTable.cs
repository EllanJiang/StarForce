using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BattlebuylimitconfigTable : BaseTable
	{
		private Dictionary<uint, BattleBuyLimitConfig> battlebuylimitconfig_map = new Dictionary<uint, BattleBuyLimitConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BattleBuyLimitConfig_Array battlebuylimitconfig_array = BattleBuyLimitConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in battlebuylimitconfig_array.Items)
				{
					battlebuylimitconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BattleBuyLimitConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BattleBuyLimitConfig> GetTable()
		{
			return battlebuylimitconfig_map;
		}

		public BattleBuyLimitConfig GetRecorder(uint key)
		{
			if (!battlebuylimitconfig_map.ContainsKey(key))
			{
				return null;
			}
			return battlebuylimitconfig_map[key];
		}

	}
}