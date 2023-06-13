using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class SubpackagemodedependconfigTable : BaseTable
	{
		private Dictionary<uint, SubpackageModeDependConfig> subpackagemodedependconfig_map = new Dictionary<uint, SubpackageModeDependConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SubpackageModeDependConfig_Array subpackagemodedependconfig_array = SubpackageModeDependConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in subpackagemodedependconfig_array.Items)
				{
					subpackagemodedependconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SubpackageModeDependConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, SubpackageModeDependConfig> GetTable()
		{
			return subpackagemodedependconfig_map;
		}

		public SubpackageModeDependConfig GetRecorder(uint key)
		{
			if (!subpackagemodedependconfig_map.ContainsKey(key))
			{
				return null;
			}
			return subpackagemodedependconfig_map[key];
		}

	}
}