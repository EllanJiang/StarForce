using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrsuppliesresourcepointconfigTable : BaseTable
	{
		private BRSuppliesResourcePointConfig_Array brsuppliesresourcepointconfig_array = new BRSuppliesResourcePointConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				brsuppliesresourcepointconfig_array = BRSuppliesResourcePointConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BRSuppliesResourcePointConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BRSuppliesResourcePointConfig_Array GetTable()
		{
			return brsuppliesresourcepointconfig_array;
		}

		public BRSuppliesResourcePointConfig GetRecorder(int key)
		{
			if (key >= brsuppliesresourcepointconfig_array.Items.Count)
			{
				return null;
			}
			return brsuppliesresourcepointconfig_array.Items[key];
		}

	}
}