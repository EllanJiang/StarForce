using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GroupdropitemconfigTable : BaseTable
	{
		private GroupDropItemConfig_Array groupdropitemconfig_array = new GroupDropItemConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				groupdropitemconfig_array = GroupDropItemConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "GroupDropItemConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public GroupDropItemConfig_Array GetTable()
		{
			return groupdropitemconfig_array;
		}

		public GroupDropItemConfig GetRecorder(int key)
		{
			if (key >= groupdropitemconfig_array.Items.Count)
			{
				return null;
			}
			return groupdropitemconfig_array.Items[key];
		}

	}
}