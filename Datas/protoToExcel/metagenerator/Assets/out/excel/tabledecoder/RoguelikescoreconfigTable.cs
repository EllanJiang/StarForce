using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RoguelikescoreconfigTable : BaseTable
	{
		private Dictionary<uint, RogueLikeScoreConfig> roguelikescoreconfig_map = new Dictionary<uint, RogueLikeScoreConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				RogueLikeScoreConfig_Array roguelikescoreconfig_array = RogueLikeScoreConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in roguelikescoreconfig_array.Items)
				{
					roguelikescoreconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "RogueLikeScoreConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, RogueLikeScoreConfig> GetTable()
		{
			return roguelikescoreconfig_map;
		}

		public RogueLikeScoreConfig GetRecorder(uint key)
		{
			if (!roguelikescoreconfig_map.ContainsKey(key))
			{
				return null;
			}
			return roguelikescoreconfig_map[key];
		}

	}
}