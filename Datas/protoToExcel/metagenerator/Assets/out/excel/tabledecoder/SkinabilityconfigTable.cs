using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class SkinabilityconfigTable : BaseTable
	{
		private Dictionary<uint, SkinAbilityConfig> skinabilityconfig_map = new Dictionary<uint, SkinAbilityConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SkinAbilityConfig_Array skinabilityconfig_array = SkinAbilityConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in skinabilityconfig_array.Items)
				{
					skinabilityconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SkinAbilityConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, SkinAbilityConfig> GetTable()
		{
			return skinabilityconfig_map;
		}

		public SkinAbilityConfig GetRecorder(uint key)
		{
			if (!skinabilityconfig_map.ContainsKey(key))
			{
				return null;
			}
			return skinabilityconfig_map[key];
		}

	}
}