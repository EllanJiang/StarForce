using System;

namespace D11.Pjson
{
	public class BallisticLogicConfigHingeDataTable : BaseTable
	{
		private BallisticLogicConfigHingeData ballisticlogicconfighingedata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ballisticlogicconfighingedata = BallisticLogicConfigHingeData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BallisticLogicConfigHingeData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BallisticLogicConfigHingeData GetTable()
		{
			return ballisticlogicconfighingedata;
		}
	}
}