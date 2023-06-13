using System;

namespace D11.Pjson
{
	public class WeaponFireLogicTaserConfigDataTable : BaseTable
	{
		private WeaponFireLogicTaserConfigData weaponfirelogictaserconfigdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponfirelogictaserconfigdata = WeaponFireLogicTaserConfigData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponFireLogicTaserConfigData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponFireLogicTaserConfigData GetTable()
		{
			return weaponfirelogictaserconfigdata;
		}
	}
}