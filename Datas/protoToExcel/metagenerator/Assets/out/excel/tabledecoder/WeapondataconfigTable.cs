using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class WeapondataconfigTable : BaseTable
	{
		private Dictionary<uint, WeaponDataConfig> weapondataconfig_map = new Dictionary<uint, WeaponDataConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				WeaponDataConfig_Array weapondataconfig_array = WeaponDataConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in weapondataconfig_array.Items)
				{
					weapondataconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponDataConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, WeaponDataConfig> GetTable()
		{
			return weapondataconfig_map;
		}

		public WeaponDataConfig GetRecorder(uint key)
		{
			if (!weapondataconfig_map.ContainsKey(key))
			{
				return null;
			}
			return weapondataconfig_map[key];
		}

	}
}