using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RoguelikerefreshgunbuffpriceTable : BaseTable
	{
		private RogueLikeRefreshGunBuffPrice_Array roguelikerefreshgunbuffprice_array = new RogueLikeRefreshGunBuffPrice_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				roguelikerefreshgunbuffprice_array = RogueLikeRefreshGunBuffPrice_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "RogueLikeRefreshGunBuffPrice_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public RogueLikeRefreshGunBuffPrice_Array GetTable()
		{
			return roguelikerefreshgunbuffprice_array;
		}

		public RogueLikeRefreshGunBuffPrice GetRecorder(int key)
		{
			if (key >= roguelikerefreshgunbuffprice_array.Items.Count)
			{
				return null;
			}
			return roguelikerefreshgunbuffprice_array.Items[key];
		}

	}
}