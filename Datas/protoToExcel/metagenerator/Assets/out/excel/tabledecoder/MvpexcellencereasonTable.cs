using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class MvpexcellencereasonTable : BaseTable
	{
		private MVPExcellenceReason_Array mvpexcellencereason_array = new MVPExcellenceReason_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				mvpexcellencereason_array = MVPExcellenceReason_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "MVPExcellenceReason_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public MVPExcellenceReason_Array GetTable()
		{
			return mvpexcellencereason_array;
		}

		public MVPExcellenceReason GetRecorder(int key)
		{
			if (key >= mvpexcellencereason_array.Items.Count)
			{
				return null;
			}
			return mvpexcellencereason_array.Items[key];
		}

	}
}