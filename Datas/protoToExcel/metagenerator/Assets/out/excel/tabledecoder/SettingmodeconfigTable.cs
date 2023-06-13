using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class SettingmodeconfigTable : BaseTable
	{
		private Dictionary<uint, SettingModeConfig> settingmodeconfig_map = new Dictionary<uint, SettingModeConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SettingModeConfig_Array settingmodeconfig_array = SettingModeConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in settingmodeconfig_array.Items)
				{
					settingmodeconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SettingModeConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, SettingModeConfig> GetTable()
		{
			return settingmodeconfig_map;
		}

		public SettingModeConfig GetRecorder(uint key)
		{
			if (!settingmodeconfig_map.ContainsKey(key))
			{
				return null;
			}
			return settingmodeconfig_map[key];
		}

	}
}