using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class PlayerskillconfigTable : BaseTable
	{
		private Dictionary<uint, PlayerSkillConfig> playerskillconfig_map = new Dictionary<uint, PlayerSkillConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				PlayerSkillConfig_Array playerskillconfig_array = PlayerSkillConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in playerskillconfig_array.Items)
				{
					playerskillconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "PlayerSkillConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, PlayerSkillConfig> GetTable()
		{
			return playerskillconfig_map;
		}

		public PlayerSkillConfig GetRecorder(uint key)
		{
			if (!playerskillconfig_map.ContainsKey(key))
			{
				return null;
			}
			return playerskillconfig_map[key];
		}

	}
}