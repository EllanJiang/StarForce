using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class SkinabilitysuitmodeconfigTable : BaseTable
	{
		private Dictionary<uint, SkinAbilitySuitModeConfig> skinabilitysuitmodeconfig_map = new Dictionary<uint, SkinAbilitySuitModeConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SkinAbilitySuitModeConfig_Array skinabilitysuitmodeconfig_array = SkinAbilitySuitModeConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in skinabilitysuitmodeconfig_array.Items)
				{
					skinabilitysuitmodeconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SkinAbilitySuitModeConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, SkinAbilitySuitModeConfig> GetTable()
		{
			return skinabilitysuitmodeconfig_map;
		}

		public SkinAbilitySuitModeConfig GetRecorder(uint key)
		{
			if (!skinabilitysuitmodeconfig_map.ContainsKey(key))
			{
				return null;
			}
			return skinabilitysuitmodeconfig_map[key];
		}

	}
}