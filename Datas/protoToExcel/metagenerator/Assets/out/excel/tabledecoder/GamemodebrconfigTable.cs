using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GamemodebrconfigTable : BaseTable
	{
		private Dictionary<uint, GameModeBRConfig> gamemodebrconfig_map = new Dictionary<uint, GameModeBRConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				GameModeBRConfig_Array gamemodebrconfig_array = GameModeBRConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in gamemodebrconfig_array.Items)
				{
					gamemodebrconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "GameModeBRConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, GameModeBRConfig> GetTable()
		{
			return gamemodebrconfig_map;
		}

		public GameModeBRConfig GetRecorder(uint key)
		{
			if (!gamemodebrconfig_map.ContainsKey(key))
			{
				return null;
			}
			return gamemodebrconfig_map[key];
		}

	}
}