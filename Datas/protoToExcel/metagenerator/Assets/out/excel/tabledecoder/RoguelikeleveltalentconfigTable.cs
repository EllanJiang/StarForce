using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RoguelikeleveltalentconfigTable : BaseTable
	{
		private Dictionary<uint, RogueLikeLevelTalentConfig> roguelikeleveltalentconfig_map = new Dictionary<uint, RogueLikeLevelTalentConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				RogueLikeLevelTalentConfig_Array roguelikeleveltalentconfig_array = RogueLikeLevelTalentConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in roguelikeleveltalentconfig_array.Items)
				{
					roguelikeleveltalentconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "RogueLikeLevelTalentConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, RogueLikeLevelTalentConfig> GetTable()
		{
			return roguelikeleveltalentconfig_map;
		}

		public RogueLikeLevelTalentConfig GetRecorder(uint key)
		{
			if (!roguelikeleveltalentconfig_map.ContainsKey(key))
			{
				return null;
			}
			return roguelikeleveltalentconfig_map[key];
		}

	}
}