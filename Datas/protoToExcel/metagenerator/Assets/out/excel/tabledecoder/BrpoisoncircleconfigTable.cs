using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrpoisoncircleconfigTable : BaseTable
	{
		private BRPoisonCircleConfig_Array brpoisoncircleconfig_array = new BRPoisonCircleConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				brpoisoncircleconfig_array = BRPoisonCircleConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BRPoisonCircleConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BRPoisonCircleConfig_Array GetTable()
		{
			return brpoisoncircleconfig_array;
		}

		public BRPoisonCircleConfig GetRecorder(int key)
		{
			if (key >= brpoisoncircleconfig_array.Items.Count)
			{
				return null;
			}
			return brpoisoncircleconfig_array.Items[key];
		}

	}
}