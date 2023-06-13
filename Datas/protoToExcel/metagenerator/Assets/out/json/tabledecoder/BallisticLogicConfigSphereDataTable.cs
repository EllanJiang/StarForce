using System;

namespace D11.Pjson
{
	public class BallisticLogicConfigSphereDataTable : BaseTable
	{
		private BallisticLogicConfigSphereData ballisticlogicconfigspheredata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ballisticlogicconfigspheredata = BallisticLogicConfigSphereData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BallisticLogicConfigSphereData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BallisticLogicConfigSphereData GetTable()
		{
			return ballisticlogicconfigspheredata;
		}
	}
}