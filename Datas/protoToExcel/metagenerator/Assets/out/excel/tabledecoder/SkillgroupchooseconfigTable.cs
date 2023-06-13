using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class SkillgroupchooseconfigTable : BaseTable
	{
		private Dictionary<uint, SkillGroupChooseConfig> skillgroupchooseconfig_map = new Dictionary<uint, SkillGroupChooseConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SkillGroupChooseConfig_Array skillgroupchooseconfig_array = SkillGroupChooseConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in skillgroupchooseconfig_array.Items)
				{
					skillgroupchooseconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SkillGroupChooseConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, SkillGroupChooseConfig> GetTable()
		{
			return skillgroupchooseconfig_map;
		}

		public SkillGroupChooseConfig GetRecorder(uint key)
		{
			if (!skillgroupchooseconfig_map.ContainsKey(key))
			{
				return null;
			}
			return skillgroupchooseconfig_map[key];
		}

	}
}