using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GunfunctionqualitygropconfigTable : BaseTable
	{
		private GunFunctionQualityGropConfig_Array gunfunctionqualitygropconfig_array = new GunFunctionQualityGropConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				gunfunctionqualitygropconfig_array = GunFunctionQualityGropConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "GunFunctionQualityGropConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public GunFunctionQualityGropConfig_Array GetTable()
		{
			return gunfunctionqualitygropconfig_array;
		}

		public GunFunctionQualityGropConfig GetRecorder(int key)
		{
			if (key >= gunfunctionqualitygropconfig_array.Items.Count)
			{
				return null;
			}
			return gunfunctionqualitygropconfig_array.Items[key];
		}

	}
}