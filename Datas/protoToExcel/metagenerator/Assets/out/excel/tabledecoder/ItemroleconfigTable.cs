using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ItemroleconfigTable : BaseTable
	{
		private Dictionary<uint, ItemRoleConfig> itemroleconfig_map = new Dictionary<uint, ItemRoleConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ItemRoleConfig_Array itemroleconfig_array = ItemRoleConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in itemroleconfig_array.Items)
				{
					itemroleconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ItemRoleConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ItemRoleConfig> GetTable()
		{
			return itemroleconfig_map;
		}

		public ItemRoleConfig GetRecorder(uint key)
		{
			if (!itemroleconfig_map.ContainsKey(key))
			{
				return null;
			}
			return itemroleconfig_map[key];
		}

	}
}