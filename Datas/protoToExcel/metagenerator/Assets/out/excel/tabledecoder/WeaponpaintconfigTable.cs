using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class WeaponpaintconfigTable : BaseTable
	{
		private Dictionary<uint, WeaponPaintConfig> weaponpaintconfig_map = new Dictionary<uint, WeaponPaintConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				WeaponPaintConfig_Array weaponpaintconfig_array = WeaponPaintConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in weaponpaintconfig_array.Items)
				{
					weaponpaintconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponPaintConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, WeaponPaintConfig> GetTable()
		{
			return weaponpaintconfig_map;
		}

		public WeaponPaintConfig GetRecorder(uint key)
		{
			if (!weaponpaintconfig_map.ContainsKey(key))
			{
				return null;
			}
			return weaponpaintconfig_map[key];
		}

	}
}