using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrweapondataconfigTable : BaseTable
	{
		private Dictionary<uint, BRWeaponDataConfig> brweapondataconfig_map = new Dictionary<uint, BRWeaponDataConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BRWeaponDataConfig_Array brweapondataconfig_array = BRWeaponDataConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in brweapondataconfig_array.Items)
				{
					brweapondataconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BRWeaponDataConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BRWeaponDataConfig> GetTable()
		{
			return brweapondataconfig_map;
		}

		public BRWeaponDataConfig GetRecorder(uint key)
		{
			if (!brweapondataconfig_map.ContainsKey(key))
			{
				return null;
			}
			return brweapondataconfig_map[key];
		}

	}
}