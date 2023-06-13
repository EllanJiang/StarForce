using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class DeathmatchsprintconfigTable : BaseTable
	{
		private Dictionary<uint, DeathMatchSprintConfig> deathmatchsprintconfig_map = new Dictionary<uint, DeathMatchSprintConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				DeathMatchSprintConfig_Array deathmatchsprintconfig_array = DeathMatchSprintConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in deathmatchsprintconfig_array.Items)
				{
					deathmatchsprintconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "DeathMatchSprintConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, DeathMatchSprintConfig> GetTable()
		{
			return deathmatchsprintconfig_map;
		}

		public DeathMatchSprintConfig GetRecorder(uint key)
		{
			if (!deathmatchsprintconfig_map.ContainsKey(key))
			{
				return null;
			}
			return deathmatchsprintconfig_map[key];
		}

	}
}