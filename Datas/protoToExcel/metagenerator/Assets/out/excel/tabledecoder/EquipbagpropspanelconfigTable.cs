using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class EquipbagpropspanelconfigTable : BaseTable
	{
		private Dictionary<uint, EquipBagPropsPanelConfig> equipbagpropspanelconfig_map = new Dictionary<uint, EquipBagPropsPanelConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				EquipBagPropsPanelConfig_Array equipbagpropspanelconfig_array = EquipBagPropsPanelConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in equipbagpropspanelconfig_array.Items)
				{
					equipbagpropspanelconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "EquipBagPropsPanelConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, EquipBagPropsPanelConfig> GetTable()
		{
			return equipbagpropspanelconfig_map;
		}

		public EquipBagPropsPanelConfig GetRecorder(uint key)
		{
			if (!equipbagpropspanelconfig_map.ContainsKey(key))
			{
				return null;
			}
			return equipbagpropspanelconfig_map[key];
		}

	}
}