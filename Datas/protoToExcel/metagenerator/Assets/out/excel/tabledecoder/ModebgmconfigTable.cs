using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ModebgmconfigTable : BaseTable
	{
		private Dictionary<uint, ModeBGMConfig> modebgmconfig_map = new Dictionary<uint, ModeBGMConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ModeBGMConfig_Array modebgmconfig_array = ModeBGMConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in modebgmconfig_array.Items)
				{
					modebgmconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ModeBGMConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ModeBGMConfig> GetTable()
		{
			return modebgmconfig_map;
		}

		public ModeBGMConfig GetRecorder(uint key)
		{
			if (!modebgmconfig_map.ContainsKey(key))
			{
				return null;
			}
			return modebgmconfig_map[key];
		}

	}
}