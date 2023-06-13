using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GamemodeconfigTable : BaseTable
	{
		private Dictionary<uint, GameModeConfig> gamemodeconfig_map = new Dictionary<uint, GameModeConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				GameModeConfig_Array gamemodeconfig_array = GameModeConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in gamemodeconfig_array.Items)
				{
					gamemodeconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "GameModeConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, GameModeConfig> GetTable()
		{
			return gamemodeconfig_map;
		}

		public GameModeConfig GetRecorder(uint key)
		{
			if (!gamemodeconfig_map.ContainsKey(key))
			{
				return null;
			}
			return gamemodeconfig_map[key];
		}

	}
}