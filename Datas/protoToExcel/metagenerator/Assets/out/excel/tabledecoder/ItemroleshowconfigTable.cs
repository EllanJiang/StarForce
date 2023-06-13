using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ItemroleshowconfigTable : BaseTable
	{
		private Dictionary<uint, ItemRoleShowConfig> itemroleshowconfig_map = new Dictionary<uint, ItemRoleShowConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ItemRoleShowConfig_Array itemroleshowconfig_array = ItemRoleShowConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in itemroleshowconfig_array.Items)
				{
					itemroleshowconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ItemRoleShowConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ItemRoleShowConfig> GetTable()
		{
			return itemroleshowconfig_map;
		}

		public ItemRoleShowConfig GetRecorder(uint key)
		{
			if (!itemroleshowconfig_map.ContainsKey(key))
			{
				return null;
			}
			return itemroleshowconfig_map[key];
		}

	}
}