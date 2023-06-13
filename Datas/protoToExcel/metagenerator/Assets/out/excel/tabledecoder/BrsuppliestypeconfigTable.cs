using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrsuppliestypeconfigTable : BaseTable
	{
		private BRSuppliesTypeConfig_Array brsuppliestypeconfig_array = new BRSuppliesTypeConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				brsuppliestypeconfig_array = BRSuppliesTypeConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BRSuppliesTypeConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BRSuppliesTypeConfig_Array GetTable()
		{
			return brsuppliestypeconfig_array;
		}

		public BRSuppliesTypeConfig GetRecorder(int key)
		{
			if (key >= brsuppliestypeconfig_array.Items.Count)
			{
				return null;
			}
			return brsuppliestypeconfig_array.Items[key];
		}

	}
}