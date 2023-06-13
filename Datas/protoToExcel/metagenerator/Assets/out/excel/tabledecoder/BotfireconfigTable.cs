using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BotfireconfigTable : BaseTable
	{
		private Dictionary<uint, BotFireConfig> botfireconfig_map = new Dictionary<uint, BotFireConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BotFireConfig_Array botfireconfig_array = BotFireConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in botfireconfig_array.Items)
				{
					botfireconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BotFireConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BotFireConfig> GetTable()
		{
			return botfireconfig_map;
		}

		public BotFireConfig GetRecorder(uint key)
		{
			if (!botfireconfig_map.ContainsKey(key))
			{
				return null;
			}
			return botfireconfig_map[key];
		}

	}
}