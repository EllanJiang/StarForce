using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class HeadframeconfigTable : BaseTable
	{
		private Dictionary<uint, HeadFrameConfig> headframeconfig_map = new Dictionary<uint, HeadFrameConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				HeadFrameConfig_Array headframeconfig_array = HeadFrameConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in headframeconfig_array.Items)
				{
					headframeconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "HeadFrameConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, HeadFrameConfig> GetTable()
		{
			return headframeconfig_map;
		}

		public HeadFrameConfig GetRecorder(uint key)
		{
			if (!headframeconfig_map.ContainsKey(key))
			{
				return null;
			}
			return headframeconfig_map[key];
		}

	}
}