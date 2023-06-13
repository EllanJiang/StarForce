using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ShopshowitemsTable : BaseTable
	{
		private Dictionary<uint, ShopShowItems> shopshowitems_map = new Dictionary<uint, ShopShowItems>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ShopShowItems_Array shopshowitems_array = ShopShowItems_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in shopshowitems_array.Items)
				{
					shopshowitems_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ShopShowItems_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ShopShowItems> GetTable()
		{
			return shopshowitems_map;
		}

		public ShopShowItems GetRecorder(uint key)
		{
			if (!shopshowitems_map.ContainsKey(key))
			{
				return null;
			}
			return shopshowitems_map[key];
		}

	}
}