using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BotweaponconfigTable : BaseTable
	{
		private Dictionary<uint, BotWeaponConfig> botweaponconfig_map = new Dictionary<uint, BotWeaponConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BotWeaponConfig_Array botweaponconfig_array = BotWeaponConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in botweaponconfig_array.Items)
				{
					botweaponconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BotWeaponConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BotWeaponConfig> GetTable()
		{
			return botweaponconfig_map;
		}

		public BotWeaponConfig GetRecorder(uint key)
		{
			if (!botweaponconfig_map.ContainsKey(key))
			{
				return null;
			}
			return botweaponconfig_map[key];
		}

	}
}