using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrshopitemgroupconfigTable : BaseTable
	{
		private BRShopItemGroupConfig_Array brshopitemgroupconfig_array = new BRShopItemGroupConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				brshopitemgroupconfig_array = BRShopItemGroupConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BRShopItemGroupConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BRShopItemGroupConfig_Array GetTable()
		{
			return brshopitemgroupconfig_array;
		}

		public BRShopItemGroupConfig GetRecorder(int key)
		{
			if (key >= brshopitemgroupconfig_array.Items.Count)
			{
				return null;
			}
			return brshopitemgroupconfig_array.Items[key];
		}

	}
}