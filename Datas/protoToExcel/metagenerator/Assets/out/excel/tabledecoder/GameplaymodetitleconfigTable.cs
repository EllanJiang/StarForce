using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GameplaymodetitleconfigTable : BaseTable
	{
		private Dictionary<uint, GamePlayModeTitleConfig> gameplaymodetitleconfig_map = new Dictionary<uint, GamePlayModeTitleConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				GamePlayModeTitleConfig_Array gameplaymodetitleconfig_array = GamePlayModeTitleConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in gameplaymodetitleconfig_array.Items)
				{
					gameplaymodetitleconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "GamePlayModeTitleConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, GamePlayModeTitleConfig> GetTable()
		{
			return gameplaymodetitleconfig_map;
		}

		public GamePlayModeTitleConfig GetRecorder(uint key)
		{
			if (!gameplaymodetitleconfig_map.ContainsKey(key))
			{
				return null;
			}
			return gameplaymodetitleconfig_map[key];
		}

	}
}