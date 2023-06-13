using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GamemodelifeconfigTable : BaseTable
	{
		private Dictionary<uint, GameModeLifeConfig> gamemodelifeconfig_map = new Dictionary<uint, GameModeLifeConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				GameModeLifeConfig_Array gamemodelifeconfig_array = GameModeLifeConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in gamemodelifeconfig_array.Items)
				{
					gamemodelifeconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "GameModeLifeConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, GameModeLifeConfig> GetTable()
		{
			return gamemodelifeconfig_map;
		}

		public GameModeLifeConfig GetRecorder(uint key)
		{
			if (!gamemodelifeconfig_map.ContainsKey(key))
			{
				return null;
			}
			return gamemodelifeconfig_map[key];
		}

	}
}