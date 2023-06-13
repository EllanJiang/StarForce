using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class WeapondetailconfigTable : BaseTable
	{
		private Dictionary<uint, WeaponDetailConfig> weapondetailconfig_map = new Dictionary<uint, WeaponDetailConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				WeaponDetailConfig_Array weapondetailconfig_array = WeaponDetailConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in weapondetailconfig_array.Items)
				{
					weapondetailconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponDetailConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, WeaponDetailConfig> GetTable()
		{
			return weapondetailconfig_map;
		}

		public WeaponDetailConfig GetRecorder(uint key)
		{
			if (!weapondetailconfig_map.ContainsKey(key))
			{
				return null;
			}
			return weapondetailconfig_map[key];
		}

	}
}