using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class NoviceguideweapondesconfigTable : BaseTable
	{
		private Dictionary<uint, NoviceGuideWeaponDesConfig> noviceguideweapondesconfig_map = new Dictionary<uint, NoviceGuideWeaponDesConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				NoviceGuideWeaponDesConfig_Array noviceguideweapondesconfig_array = NoviceGuideWeaponDesConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in noviceguideweapondesconfig_array.Items)
				{
					noviceguideweapondesconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "NoviceGuideWeaponDesConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, NoviceGuideWeaponDesConfig> GetTable()
		{
			return noviceguideweapondesconfig_map;
		}

		public NoviceGuideWeaponDesConfig GetRecorder(uint key)
		{
			if (!noviceguideweapondesconfig_map.ContainsKey(key))
			{
				return null;
			}
			return noviceguideweapondesconfig_map[key];
		}

	}
}