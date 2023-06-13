using System;
using System.Collections.Generic;
namespace Configs
{
	public class AbilityuseconfigTable : BaseTable
	{
		private Dictionary<uint, AbilityUseConfig> abilityuseconfig_map = new Dictionary<uint, AbilityUseConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				AbilityUseConfig_Array abilityuseconfig_array = AbilityUseConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in abilityuseconfig_array.Items)
				{
					abilityuseconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "AbilityUseConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, AbilityUseConfig> GetTable()
		{
			return abilityuseconfig_map;
		}

		public AbilityUseConfig GetRecorder(uint key)
		{
			if (!abilityuseconfig_map.ContainsKey(key))
			{
				return null;
			}
			return abilityuseconfig_map[key];
		}

	}
}