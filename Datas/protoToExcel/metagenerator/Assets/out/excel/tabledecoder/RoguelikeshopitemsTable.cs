using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RoguelikeshopitemsTable : BaseTable
	{
		private Dictionary<uint, RogueLikeShopItems> roguelikeshopitems_map = new Dictionary<uint, RogueLikeShopItems>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				RogueLikeShopItems_Array roguelikeshopitems_array = RogueLikeShopItems_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in roguelikeshopitems_array.Items)
				{
					roguelikeshopitems_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "RogueLikeShopItems_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, RogueLikeShopItems> GetTable()
		{
			return roguelikeshopitems_map;
		}

		public RogueLikeShopItems GetRecorder(uint key)
		{
			if (!roguelikeshopitems_map.ContainsKey(key))
			{
				return null;
			}
			return roguelikeshopitems_map[key];
		}

	}
}