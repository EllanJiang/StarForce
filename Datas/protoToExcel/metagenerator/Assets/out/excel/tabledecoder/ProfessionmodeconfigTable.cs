using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ProfessionmodeconfigTable : BaseTable
	{
		private Dictionary<uint, ProfessionModeConfig> professionmodeconfig_map = new Dictionary<uint, ProfessionModeConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ProfessionModeConfig_Array professionmodeconfig_array = ProfessionModeConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in professionmodeconfig_array.Items)
				{
					professionmodeconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ProfessionModeConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ProfessionModeConfig> GetTable()
		{
			return professionmodeconfig_map;
		}

		public ProfessionModeConfig GetRecorder(uint key)
		{
			if (!professionmodeconfig_map.ContainsKey(key))
			{
				return null;
			}
			return professionmodeconfig_map[key];
		}

	}
}