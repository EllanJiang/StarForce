using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ScreeninnerwidthconfigTable : BaseTable
	{
		private Dictionary<uint, ScreenInnerWidthConfig> screeninnerwidthconfig_map = new Dictionary<uint, ScreenInnerWidthConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ScreenInnerWidthConfig_Array screeninnerwidthconfig_array = ScreenInnerWidthConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in screeninnerwidthconfig_array.Items)
				{
					screeninnerwidthconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ScreenInnerWidthConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ScreenInnerWidthConfig> GetTable()
		{
			return screeninnerwidthconfig_map;
		}

		public ScreenInnerWidthConfig GetRecorder(uint key)
		{
			if (!screeninnerwidthconfig_map.ContainsKey(key))
			{
				return null;
			}
			return screeninnerwidthconfig_map[key];
		}

	}
}