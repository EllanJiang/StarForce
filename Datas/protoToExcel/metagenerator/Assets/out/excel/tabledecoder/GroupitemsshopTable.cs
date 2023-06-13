using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GroupitemsshopTable : BaseTable
	{
		private Dictionary<uint, GroupItemsShop> groupitemsshop_map = new Dictionary<uint, GroupItemsShop>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				GroupItemsShop_Array groupitemsshop_array = GroupItemsShop_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in groupitemsshop_array.Items)
				{
					groupitemsshop_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "GroupItemsShop_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, GroupItemsShop> GetTable()
		{
			return groupitemsshop_map;
		}

		public GroupItemsShop GetRecorder(uint key)
		{
			if (!groupitemsshop_map.ContainsKey(key))
			{
				return null;
			}
			return groupitemsshop_map[key];
		}

	}
}