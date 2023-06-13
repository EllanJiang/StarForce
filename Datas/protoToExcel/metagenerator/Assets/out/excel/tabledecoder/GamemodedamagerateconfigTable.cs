using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GamemodedamagerateconfigTable : BaseTable
	{
		private Dictionary<string, GameModeDamageRateConfig> gamemodedamagerateconfig_map = new Dictionary<string, GameModeDamageRateConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				GameModeDamageRateConfig_Array gamemodedamagerateconfig_array = GameModeDamageRateConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in gamemodedamagerateconfig_array.Items)
				{
					gamemodedamagerateconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "GameModeDamageRateConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<string, GameModeDamageRateConfig> GetTable()
		{
			return gamemodedamagerateconfig_map;
		}

		public GameModeDamageRateConfig GetRecorder(string key)
		{
			if (!gamemodedamagerateconfig_map.ContainsKey(key))
			{
				return null;
			}
			return gamemodedamagerateconfig_map[key];
		}

	}
}