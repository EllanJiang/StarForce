using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class CampexcellencereasonTable : BaseTable
	{
		private CampExcellenceReason_Array campexcellencereason_array = new CampExcellenceReason_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				campexcellencereason_array = CampExcellenceReason_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "CampExcellenceReason_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public CampExcellenceReason_Array GetTable()
		{
			return campexcellencereason_array;
		}

		public CampExcellenceReason GetRecorder(int key)
		{
			if (key >= campexcellencereason_array.Items.Count)
			{
				return null;
			}
			return campexcellencereason_array.Items[key];
		}

	}
}