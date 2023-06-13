using System;

namespace D11.Pjson
{
	public class BaseBallisticConfigDataTable : BaseTable
	{
		private BaseBallisticConfigData baseballisticconfigdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				baseballisticconfigdata = BaseBallisticConfigData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BaseBallisticConfigData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BaseBallisticConfigData GetTable()
		{
			return baseballisticconfigdata;
		}
	}
}