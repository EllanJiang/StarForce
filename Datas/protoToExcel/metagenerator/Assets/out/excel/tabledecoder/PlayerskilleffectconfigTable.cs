using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class PlayerskilleffectconfigTable : BaseTable
	{
		private Dictionary<uint, PlayerSkillEffectConfig> playerskilleffectconfig_map = new Dictionary<uint, PlayerSkillEffectConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				PlayerSkillEffectConfig_Array playerskilleffectconfig_array = PlayerSkillEffectConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in playerskilleffectconfig_array.Items)
				{
					playerskilleffectconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "PlayerSkillEffectConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, PlayerSkillEffectConfig> GetTable()
		{
			return playerskilleffectconfig_map;
		}

		public PlayerSkillEffectConfig GetRecorder(uint key)
		{
			if (!playerskilleffectconfig_map.ContainsKey(key))
			{
				return null;
			}
			return playerskilleffectconfig_map[key];
		}

	}
}