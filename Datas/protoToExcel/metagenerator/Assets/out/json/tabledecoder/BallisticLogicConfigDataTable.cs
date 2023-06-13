using System;

namespace D11.Pjson
{
	public class BallisticLogicConfigDataTable : BaseTable
	{
		private BallisticLogicConfigData ballisticlogicconfigdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ballisticlogicconfigdata = BallisticLogicConfigData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BallisticLogicConfigData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BallisticLogicConfigData GetTable()
		{
			return ballisticlogicconfigdata;
		}
	}
}