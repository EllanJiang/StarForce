using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class WbbrscenetilinginfoTable : BaseTable
	{
		private WbBRSceneTilingInfo_Array wbbrscenetilinginfo_array = new WbBRSceneTilingInfo_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				wbbrscenetilinginfo_array = WbBRSceneTilingInfo_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WbBRSceneTilingInfo_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WbBRSceneTilingInfo_Array GetTable()
		{
			return wbbrscenetilinginfo_array;
		}

		public WbBRSceneTilingInfo GetRecorder(int key)
		{
			if (key >= wbbrscenetilinginfo_array.Items.Count)
			{
				return null;
			}
			return wbbrscenetilinginfo_array.Items[key];
		}

	}
}