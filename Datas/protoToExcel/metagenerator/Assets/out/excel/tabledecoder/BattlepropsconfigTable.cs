using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BattlepropsconfigTable : BaseTable
	{
		private Dictionary<uint, BattlePropsConfig> battlepropsconfig_map = new Dictionary<uint, BattlePropsConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BattlePropsConfig_Array battlepropsconfig_array = BattlePropsConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in battlepropsconfig_array.Items)
				{
					battlepropsconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BattlePropsConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BattlePropsConfig> GetTable()
		{
			return battlepropsconfig_map;
		}

		public BattlePropsConfig GetRecorder(uint key)
		{
			if (!battlepropsconfig_map.ContainsKey(key))
			{
				return null;
			}
			return battlepropsconfig_map[key];
		}

	}
}