using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BattleinitweaponconfigTable : BaseTable
	{
		private Dictionary<uint, BattleInitWeaponConfig> battleinitweaponconfig_map = new Dictionary<uint, BattleInitWeaponConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BattleInitWeaponConfig_Array battleinitweaponconfig_array = BattleInitWeaponConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in battleinitweaponconfig_array.Items)
				{
					battleinitweaponconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BattleInitWeaponConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BattleInitWeaponConfig> GetTable()
		{
			return battleinitweaponconfig_map;
		}

		public BattleInitWeaponConfig GetRecorder(uint key)
		{
			if (!battleinitweaponconfig_map.ContainsKey(key))
			{
				return null;
			}
			return battleinitweaponconfig_map[key];
		}

	}
}