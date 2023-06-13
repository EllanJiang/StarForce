using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrscenetilinginfoTable : BaseTable
	{
		private BRSceneTilingInfo_Array brscenetilinginfo_array = new BRSceneTilingInfo_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				brscenetilinginfo_array = BRSceneTilingInfo_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BRSceneTilingInfo_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BRSceneTilingInfo_Array GetTable()
		{
			return brscenetilinginfo_array;
		}

		public BRSceneTilingInfo GetRecorder(int key)
		{
			if (key >= brscenetilinginfo_array.Items.Count)
			{
				return null;
			}
			return brscenetilinginfo_array.Items[key];
		}

	}
}