using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrshoplotteryconfigTable : BaseTable
	{
		private BRShopLotteryConfig_Array brshoplotteryconfig_array = new BRShopLotteryConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				brshoplotteryconfig_array = BRShopLotteryConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BRShopLotteryConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BRShopLotteryConfig_Array GetTable()
		{
			return brshoplotteryconfig_array;
		}

		public BRShopLotteryConfig GetRecorder(int key)
		{
			if (key >= brshoplotteryconfig_array.Items.Count)
			{
				return null;
			}
			return brshoplotteryconfig_array.Items[key];
		}

	}
}