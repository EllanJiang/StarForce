using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ShopitemspriceTable : BaseTable
	{
		private Dictionary<uint, ShopItemsPrice> shopitemsprice_map = new Dictionary<uint, ShopItemsPrice>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ShopItemsPrice_Array shopitemsprice_array = ShopItemsPrice_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in shopitemsprice_array.Items)
				{
					shopitemsprice_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ShopItemsPrice_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ShopItemsPrice> GetTable()
		{
			return shopitemsprice_map;
		}

		public ShopItemsPrice GetRecorder(uint key)
		{
			if (!shopitemsprice_map.ContainsKey(key))
			{
				return null;
			}
			return shopitemsprice_map[key];
		}

	}
}