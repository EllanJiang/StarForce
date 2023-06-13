using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GamemodebaseconfigTable : BaseTable
	{
		private Dictionary<uint, GameModeBaseConfig> gamemodebaseconfig_map = new Dictionary<uint, GameModeBaseConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				GameModeBaseConfig_Array gamemodebaseconfig_array = GameModeBaseConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in gamemodebaseconfig_array.Items)
				{
					gamemodebaseconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "GameModeBaseConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, GameModeBaseConfig> GetTable()
		{
			return gamemodebaseconfig_map;
		}

		public GameModeBaseConfig GetRecorder(uint key)
		{
			if (!gamemodebaseconfig_map.ContainsKey(key))
			{
				return null;
			}
			return gamemodebaseconfig_map[key];
		}

	}
}