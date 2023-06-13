using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class SettingrangefloatfieldsconfigTable : BaseTable
	{
		private Dictionary<uint, SettingRangeFloatFieldsConfig> settingrangefloatfieldsconfig_map = new Dictionary<uint, SettingRangeFloatFieldsConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SettingRangeFloatFieldsConfig_Array settingrangefloatfieldsconfig_array = SettingRangeFloatFieldsConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in settingrangefloatfieldsconfig_array.Items)
				{
					settingrangefloatfieldsconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SettingRangeFloatFieldsConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, SettingRangeFloatFieldsConfig> GetTable()
		{
			return settingrangefloatfieldsconfig_map;
		}

		public SettingRangeFloatFieldsConfig GetRecorder(uint key)
		{
			if (!settingrangefloatfieldsconfig_map.ContainsKey(key))
			{
				return null;
			}
			return settingrangefloatfieldsconfig_map[key];
		}

	}
}