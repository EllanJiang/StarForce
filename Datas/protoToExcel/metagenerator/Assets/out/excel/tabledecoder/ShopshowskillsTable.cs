using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ShopshowskillsTable : BaseTable
	{
		private Dictionary<uint, ShopShowSkills> shopshowskills_map = new Dictionary<uint, ShopShowSkills>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ShopShowSkills_Array shopshowskills_array = ShopShowSkills_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in shopshowskills_array.Items)
				{
					shopshowskills_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ShopShowSkills_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ShopShowSkills> GetTable()
		{
			return shopshowskills_map;
		}

		public ShopShowSkills GetRecorder(uint key)
		{
			if (!shopshowskills_map.ContainsKey(key))
			{
				return null;
			}
			return shopshowskills_map[key];
		}

	}
}