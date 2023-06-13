using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GameresultbuffconfigTable : BaseTable
	{
		private Dictionary<uint, GameResultBuffConfig> gameresultbuffconfig_map = new Dictionary<uint, GameResultBuffConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				GameResultBuffConfig_Array gameresultbuffconfig_array = GameResultBuffConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in gameresultbuffconfig_array.Items)
				{
					gameresultbuffconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "GameResultBuffConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, GameResultBuffConfig> GetTable()
		{
			return gameresultbuffconfig_map;
		}

		public GameResultBuffConfig GetRecorder(uint key)
		{
			if (!gameresultbuffconfig_map.ContainsKey(key))
			{
				return null;
			}
			return gameresultbuffconfig_map[key];
		}

	}
}