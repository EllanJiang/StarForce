using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class MonsterskillconfigTable : BaseTable
	{
		private Dictionary<uint, MonsterSkillConfig> monsterskillconfig_map = new Dictionary<uint, MonsterSkillConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				MonsterSkillConfig_Array monsterskillconfig_array = MonsterSkillConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in monsterskillconfig_array.Items)
				{
					monsterskillconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "MonsterSkillConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, MonsterSkillConfig> GetTable()
		{
			return monsterskillconfig_map;
		}

		public MonsterSkillConfig GetRecorder(uint key)
		{
			if (!monsterskillconfig_map.ContainsKey(key))
			{
				return null;
			}
			return monsterskillconfig_map[key];
		}

	}
}