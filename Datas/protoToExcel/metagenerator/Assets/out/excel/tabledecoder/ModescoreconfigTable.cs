using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ModescoreconfigTable : BaseTable
	{
		private Dictionary<uint, ModeScoreConfig> modescoreconfig_map = new Dictionary<uint, ModeScoreConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ModeScoreConfig_Array modescoreconfig_array = ModeScoreConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in modescoreconfig_array.Items)
				{
					modescoreconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ModeScoreConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ModeScoreConfig> GetTable()
		{
			return modescoreconfig_map;
		}

		public ModeScoreConfig GetRecorder(uint key)
		{
			if (!modescoreconfig_map.ContainsKey(key))
			{
				return null;
			}
			return modescoreconfig_map[key];
		}

	}
}