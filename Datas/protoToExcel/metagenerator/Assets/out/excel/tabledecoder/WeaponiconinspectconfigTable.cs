using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class WeaponiconinspectconfigTable : BaseTable
	{
		private Dictionary<uint, WeaponIconInspectConfig> weaponiconinspectconfig_map = new Dictionary<uint, WeaponIconInspectConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				WeaponIconInspectConfig_Array weaponiconinspectconfig_array = WeaponIconInspectConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in weaponiconinspectconfig_array.Items)
				{
					weaponiconinspectconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponIconInspectConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, WeaponIconInspectConfig> GetTable()
		{
			return weaponiconinspectconfig_map;
		}

		public WeaponIconInspectConfig GetRecorder(uint key)
		{
			if (!weaponiconinspectconfig_map.ContainsKey(key))
			{
				return null;
			}
			return weaponiconinspectconfig_map[key];
		}

	}
}