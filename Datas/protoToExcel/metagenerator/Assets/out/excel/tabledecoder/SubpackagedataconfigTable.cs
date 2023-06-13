using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class SubpackagedataconfigTable : BaseTable
	{
		private Dictionary<int, SubpackageDataConfig> subpackagedataconfig_map = new Dictionary<int, SubpackageDataConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SubpackageDataConfig_Array subpackagedataconfig_array = SubpackageDataConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in subpackagedataconfig_array.Items)
				{
					subpackagedataconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SubpackageDataConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<int, SubpackageDataConfig> GetTable()
		{
			return subpackagedataconfig_map;
		}

		public SubpackageDataConfig GetRecorder(int key)
		{
			if (!subpackagedataconfig_map.ContainsKey(key))
			{
				return null;
			}
			return subpackagedataconfig_map[key];
		}

	}
}