using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class WeaponpaintkitconfigTable : BaseTable
	{
		private Dictionary<uint, WeaponPaintKitConfig> weaponpaintkitconfig_map = new Dictionary<uint, WeaponPaintKitConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				WeaponPaintKitConfig_Array weaponpaintkitconfig_array = WeaponPaintKitConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in weaponpaintkitconfig_array.Items)
				{
					weaponpaintkitconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponPaintKitConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, WeaponPaintKitConfig> GetTable()
		{
			return weaponpaintkitconfig_map;
		}

		public WeaponPaintKitConfig GetRecorder(uint key)
		{
			if (!weaponpaintkitconfig_map.ContainsKey(key))
			{
				return null;
			}
			return weaponpaintkitconfig_map[key];
		}

	}
}