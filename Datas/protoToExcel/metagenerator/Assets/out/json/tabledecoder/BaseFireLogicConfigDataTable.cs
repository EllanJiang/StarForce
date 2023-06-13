using System;

namespace D11.Pjson
{
	public class BaseFireLogicConfigDataTable : BaseTable
	{
		private BaseFireLogicConfigData basefirelogicconfigdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				basefirelogicconfigdata = BaseFireLogicConfigData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BaseFireLogicConfigData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BaseFireLogicConfigData GetTable()
		{
			return basefirelogicconfigdata;
		}
	}
}