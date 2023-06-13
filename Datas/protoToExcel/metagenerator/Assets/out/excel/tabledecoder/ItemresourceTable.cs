using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ItemresourceTable : BaseTable
	{
		private Dictionary<uint, ItemResource> itemresource_map = new Dictionary<uint, ItemResource>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ItemResource_Array itemresource_array = ItemResource_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in itemresource_array.Items)
				{
					itemresource_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ItemResource_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ItemResource> GetTable()
		{
			return itemresource_map;
		}

		public ItemResource GetRecorder(uint key)
		{
			if (!itemresource_map.ContainsKey(key))
			{
				return null;
			}
			return itemresource_map[key];
		}

	}
}