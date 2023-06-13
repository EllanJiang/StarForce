using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ItemcommentconfigTable : BaseTable
	{
		private Dictionary<uint, ItemCommentConfig> itemcommentconfig_map = new Dictionary<uint, ItemCommentConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ItemCommentConfig_Array itemcommentconfig_array = ItemCommentConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in itemcommentconfig_array.Items)
				{
					itemcommentconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ItemCommentConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ItemCommentConfig> GetTable()
		{
			return itemcommentconfig_map;
		}

		public ItemCommentConfig GetRecorder(uint key)
		{
			if (!itemcommentconfig_map.ContainsKey(key))
			{
				return null;
			}
			return itemcommentconfig_map[key];
		}

	}
}