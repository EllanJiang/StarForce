using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrsuppliesconfigTable : BaseTable
	{
		private BRSuppliesConfig_Array brsuppliesconfig_array = new BRSuppliesConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				brsuppliesconfig_array = BRSuppliesConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BRSuppliesConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BRSuppliesConfig_Array GetTable()
		{
			return brsuppliesconfig_array;
		}

		public BRSuppliesConfig GetRecorder(int key)
		{
			if (key >= brsuppliesconfig_array.Items.Count)
			{
				return null;
			}
			return brsuppliesconfig_array.Items[key];
		}

	}
}