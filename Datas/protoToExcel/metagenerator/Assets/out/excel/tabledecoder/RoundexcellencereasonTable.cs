using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RoundexcellencereasonTable : BaseTable
	{
		private RoundExcellenceReason_Array roundexcellencereason_array = new RoundExcellenceReason_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				roundexcellencereason_array = RoundExcellenceReason_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "RoundExcellenceReason_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public RoundExcellenceReason_Array GetTable()
		{
			return roundexcellencereason_array;
		}

		public RoundExcellenceReason GetRecorder(int key)
		{
			if (key >= roundexcellencereason_array.Items.Count)
			{
				return null;
			}
			return roundexcellencereason_array.Items[key];
		}

	}
}