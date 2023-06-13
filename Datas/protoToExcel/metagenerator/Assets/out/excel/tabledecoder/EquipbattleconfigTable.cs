using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class EquipbattleconfigTable : BaseTable
	{
		private Dictionary<uint, EquipBattleConfig> equipbattleconfig_map = new Dictionary<uint, EquipBattleConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				EquipBattleConfig_Array equipbattleconfig_array = EquipBattleConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in equipbattleconfig_array.Items)
				{
					equipbattleconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "EquipBattleConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, EquipBattleConfig> GetTable()
		{
			return equipbattleconfig_map;
		}

		public EquipBattleConfig GetRecorder(uint key)
		{
			if (!equipbattleconfig_map.ContainsKey(key))
			{
				return null;
			}
			return equipbattleconfig_map[key];
		}

	}
}