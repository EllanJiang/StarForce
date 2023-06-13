using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GamemapconfigTable : BaseTable
	{
		private Dictionary<uint, GameMapConfig> gamemapconfig_map = new Dictionary<uint, GameMapConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				GameMapConfig_Array gamemapconfig_array = GameMapConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in gamemapconfig_array.Items)
				{
					gamemapconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "GameMapConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, GameMapConfig> GetTable()
		{
			return gamemapconfig_map;
		}

		public GameMapConfig GetRecorder(uint key)
		{
			if (!gamemapconfig_map.ContainsKey(key))
			{
				return null;
			}
			return gamemapconfig_map[key];
		}

	}
}