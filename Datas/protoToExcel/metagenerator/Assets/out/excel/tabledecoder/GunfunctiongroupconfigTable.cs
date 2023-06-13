using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GunfunctiongroupconfigTable : BaseTable
	{
		private GunFunctionGroupConfig_Array gunfunctiongroupconfig_array = new GunFunctionGroupConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				gunfunctiongroupconfig_array = GunFunctionGroupConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "GunFunctionGroupConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public GunFunctionGroupConfig_Array GetTable()
		{
			return gunfunctiongroupconfig_array;
		}

		public GunFunctionGroupConfig GetRecorder(int key)
		{
			if (key >= gunfunctiongroupconfig_array.Items.Count)
			{
				return null;
			}
			return gunfunctiongroupconfig_array.Items[key];
		}

	}
}