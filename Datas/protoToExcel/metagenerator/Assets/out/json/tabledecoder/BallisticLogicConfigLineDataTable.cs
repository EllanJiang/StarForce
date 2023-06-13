using System;

namespace D11.Pjson
{
	public class BallisticLogicConfigLineDataTable : BaseTable
	{
		private BallisticLogicConfigLineData ballisticlogicconfiglinedata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ballisticlogicconfiglinedata = BallisticLogicConfigLineData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BallisticLogicConfigLineData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BallisticLogicConfigLineData GetTable()
		{
			return ballisticlogicconfiglinedata;
		}
	}
}