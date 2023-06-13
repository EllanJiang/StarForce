using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class StrengthenconfigTable : BaseTable
	{
		private Dictionary<uint, StrengthenConfig> strengthenconfig_map = new Dictionary<uint, StrengthenConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				StrengthenConfig_Array strengthenconfig_array = StrengthenConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in strengthenconfig_array.Items)
				{
					strengthenconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "StrengthenConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, StrengthenConfig> GetTable()
		{
			return strengthenconfig_map;
		}

		public StrengthenConfig GetRecorder(uint key)
		{
			if (!strengthenconfig_map.ContainsKey(key))
			{
				return null;
			}
			return strengthenconfig_map[key];
		}

	}
}