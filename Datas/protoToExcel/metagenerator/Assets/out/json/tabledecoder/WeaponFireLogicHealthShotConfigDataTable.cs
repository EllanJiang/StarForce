using System;

namespace D11.Pjson
{
	public class WeaponFireLogicHealthShotConfigDataTable : BaseTable
	{
		private WeaponFireLogicHealthShotConfigData weaponfirelogichealthshotconfigdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponfirelogichealthshotconfigdata = WeaponFireLogicHealthShotConfigData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponFireLogicHealthShotConfigData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponFireLogicHealthShotConfigData GetTable()
		{
			return weaponfirelogichealthshotconfigdata;
		}
	}
}