using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BotconstconfigTable : BaseTable
	{
		private Dictionary<uint, BotConstConfig> botconstconfig_map = new Dictionary<uint, BotConstConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BotConstConfig_Array botconstconfig_array = BotConstConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in botconstconfig_array.Items)
				{
					botconstconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BotConstConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BotConstConfig> GetTable()
		{
			return botconstconfig_map;
		}

		public BotConstConfig GetRecorder(uint key)
		{
			if (!botconstconfig_map.ContainsKey(key))
			{
				return null;
			}
			return botconstconfig_map[key];
		}

	}
}