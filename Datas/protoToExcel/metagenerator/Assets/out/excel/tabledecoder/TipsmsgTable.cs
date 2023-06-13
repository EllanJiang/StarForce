using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class TipsmsgTable : BaseTable
	{
		private TipsMsg_Array tipsmsg_array = new TipsMsg_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				tipsmsg_array = TipsMsg_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "TipsMsg_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public TipsMsg_Array GetTable()
		{
			return tipsmsg_array;
		}

		public TipsMsg GetRecorder(int key)
		{
			if (key >= tipsmsg_array.Items.Count)
			{
				return null;
			}
			return tipsmsg_array.Items[key];
		}

	}
}