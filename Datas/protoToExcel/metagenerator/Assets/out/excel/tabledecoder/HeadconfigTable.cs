using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class HeadconfigTable : BaseTable
	{
		private Dictionary<uint, HeadConfig> headconfig_map = new Dictionary<uint, HeadConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				HeadConfig_Array headconfig_array = HeadConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in headconfig_array.Items)
				{
					headconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "HeadConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, HeadConfig> GetTable()
		{
			return headconfig_map;
		}

		public HeadConfig GetRecorder(uint key)
		{
			if (!headconfig_map.ContainsKey(key))
			{
				return null;
			}
			return headconfig_map[key];
		}

	}
}