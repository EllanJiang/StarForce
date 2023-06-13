using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GamemodehudconfigTable : BaseTable
	{
		private Dictionary<uint, GameModeHudConfig> gamemodehudconfig_map = new Dictionary<uint, GameModeHudConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				GameModeHudConfig_Array gamemodehudconfig_array = GameModeHudConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in gamemodehudconfig_array.Items)
				{
					gamemodehudconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "GameModeHudConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, GameModeHudConfig> GetTable()
		{
			return gamemodehudconfig_map;
		}

		public GameModeHudConfig GetRecorder(uint key)
		{
			if (!gamemodehudconfig_map.ContainsKey(key))
			{
				return null;
			}
			return gamemodehudconfig_map[key];
		}

	}
}