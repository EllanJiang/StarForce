using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ItemdropconfigTable : BaseTable
	{
		private ItemDropConfig_Array itemdropconfig_array = new ItemDropConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				itemdropconfig_array = ItemDropConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "ItemDropConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public ItemDropConfig_Array GetTable()
		{
			return itemdropconfig_array;
		}

		public ItemDropConfig GetRecorder(int key)
		{
			if (key >= itemdropconfig_array.Items.Count)
			{
				return null;
			}
			return itemdropconfig_array.Items[key];
		}

	}
}