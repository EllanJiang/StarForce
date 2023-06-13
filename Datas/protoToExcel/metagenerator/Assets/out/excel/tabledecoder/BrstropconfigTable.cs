using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrstropconfigTable : BaseTable
	{
		private BRStropConfig_Array brstropconfig_array = new BRStropConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				brstropconfig_array = BRStropConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BRStropConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BRStropConfig_Array GetTable()
		{
			return brstropconfig_array;
		}

		public BRStropConfig GetRecorder(int key)
		{
			if (key >= brstropconfig_array.Items.Count)
			{
				return null;
			}
			return brstropconfig_array.Items[key];
		}

	}
}