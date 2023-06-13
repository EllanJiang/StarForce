using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class WeapondetailfieldconfigTable : BaseTable
	{
		private Dictionary<string, WeaponDetailFieldConfig> weapondetailfieldconfig_map = new Dictionary<string, WeaponDetailFieldConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				WeaponDetailFieldConfig_Array weapondetailfieldconfig_array = WeaponDetailFieldConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in weapondetailfieldconfig_array.Items)
				{
					weapondetailfieldconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponDetailFieldConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<string, WeaponDetailFieldConfig> GetTable()
		{
			return weapondetailfieldconfig_map;
		}

		public WeaponDetailFieldConfig GetRecorder(string key)
		{
			if (!weapondetailfieldconfig_map.ContainsKey(key))
			{
				return null;
			}
			return weapondetailfieldconfig_map[key];
		}

	}
}