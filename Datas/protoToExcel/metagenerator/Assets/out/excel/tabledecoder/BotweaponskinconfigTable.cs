using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BotweaponskinconfigTable : BaseTable
	{
		private Dictionary<uint, BotWeaponSkinConfig> botweaponskinconfig_map = new Dictionary<uint, BotWeaponSkinConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BotWeaponSkinConfig_Array botweaponskinconfig_array = BotWeaponSkinConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in botweaponskinconfig_array.Items)
				{
					botweaponskinconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BotWeaponSkinConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BotWeaponSkinConfig> GetTable()
		{
			return botweaponskinconfig_map;
		}

		public BotWeaponSkinConfig GetRecorder(uint key)
		{
			if (!botweaponskinconfig_map.ContainsKey(key))
			{
				return null;
			}
			return botweaponskinconfig_map[key];
		}

	}
}