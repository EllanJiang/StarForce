using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RoguelikeshopitemsgroupTable : BaseTable
	{
		private RogueLikeShopItemsGroup_Array roguelikeshopitemsgroup_array = new RogueLikeShopItemsGroup_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				roguelikeshopitemsgroup_array = RogueLikeShopItemsGroup_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "RogueLikeShopItemsGroup_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public RogueLikeShopItemsGroup_Array GetTable()
		{
			return roguelikeshopitemsgroup_array;
		}

		public RogueLikeShopItemsGroup GetRecorder(int key)
		{
			if (key >= roguelikeshopitemsgroup_array.Items.Count)
			{
				return null;
			}
			return roguelikeshopitemsgroup_array.Items[key];
		}

	}
}