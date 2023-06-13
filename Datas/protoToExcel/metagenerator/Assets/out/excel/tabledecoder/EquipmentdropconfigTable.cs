using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class EquipmentdropconfigTable : BaseTable
	{
		private EquipmentDropConfig_Array equipmentdropconfig_array = new EquipmentDropConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				equipmentdropconfig_array = EquipmentDropConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "EquipmentDropConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public EquipmentDropConfig_Array GetTable()
		{
			return equipmentdropconfig_array;
		}

		public EquipmentDropConfig GetRecorder(int key)
		{
			if (key >= equipmentdropconfig_array.Items.Count)
			{
				return null;
			}
			return equipmentdropconfig_array.Items[key];
		}

	}
}