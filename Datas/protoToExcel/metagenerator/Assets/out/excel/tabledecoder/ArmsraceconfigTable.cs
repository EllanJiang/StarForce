using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ArmsraceconfigTable : BaseTable
	{
		private Dictionary<uint, ArmsRaceConfig> armsraceconfig_map = new Dictionary<uint, ArmsRaceConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ArmsRaceConfig_Array armsraceconfig_array = ArmsRaceConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in armsraceconfig_array.Items)
				{
					armsraceconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ArmsRaceConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ArmsRaceConfig> GetTable()
		{
			return armsraceconfig_map;
		}

		public ArmsRaceConfig GetRecorder(uint key)
		{
			if (!armsraceconfig_map.ContainsKey(key))
			{
				return null;
			}
			return armsraceconfig_map[key];
		}

	}
}