using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class EquipmentdropgroupconfigTable : BaseTable
	{
		private EquipmentDropGroupConfig_Array equipmentdropgroupconfig_array = new EquipmentDropGroupConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				equipmentdropgroupconfig_array = EquipmentDropGroupConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "EquipmentDropGroupConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public EquipmentDropGroupConfig_Array GetTable()
		{
			return equipmentdropgroupconfig_array;
		}

		public EquipmentDropGroupConfig GetRecorder(int key)
		{
			if (key >= equipmentdropgroupconfig_array.Items.Count)
			{
				return null;
			}
			return equipmentdropgroupconfig_array.Items[key];
		}

	}
}