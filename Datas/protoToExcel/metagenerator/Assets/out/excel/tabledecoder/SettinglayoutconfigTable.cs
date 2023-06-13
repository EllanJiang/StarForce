using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class SettinglayoutconfigTable : BaseTable
	{
		private Dictionary<uint, SettingLayoutConfig> settinglayoutconfig_map = new Dictionary<uint, SettingLayoutConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SettingLayoutConfig_Array settinglayoutconfig_array = SettingLayoutConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in settinglayoutconfig_array.Items)
				{
					settinglayoutconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SettingLayoutConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, SettingLayoutConfig> GetTable()
		{
			return settinglayoutconfig_map;
		}

		public SettingLayoutConfig GetRecorder(uint key)
		{
			if (!settinglayoutconfig_map.ContainsKey(key))
			{
				return null;
			}
			return settinglayoutconfig_map[key];
		}

	}
}