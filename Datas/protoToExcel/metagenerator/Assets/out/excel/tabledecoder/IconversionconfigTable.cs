using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class IconversionconfigTable : BaseTable
	{
		private Dictionary<uint, IconVersionConfig> iconversionconfig_map = new Dictionary<uint, IconVersionConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				IconVersionConfig_Array iconversionconfig_array = IconVersionConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in iconversionconfig_array.Items)
				{
					iconversionconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "IconVersionConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, IconVersionConfig> GetTable()
		{
			return iconversionconfig_map;
		}

		public IconVersionConfig GetRecorder(uint key)
		{
			if (!iconversionconfig_map.ContainsKey(key))
			{
				return null;
			}
			return iconversionconfig_map[key];
		}

	}
}