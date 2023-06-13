using System;

namespace D11.Pjson
{
	public class WeaponFireLogicBuffShotConfigDataTable : BaseTable
	{
		private WeaponFireLogicBuffShotConfigData weaponfirelogicbuffshotconfigdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponfirelogicbuffshotconfigdata = WeaponFireLogicBuffShotConfigData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponFireLogicBuffShotConfigData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponFireLogicBuffShotConfigData GetTable()
		{
			return weaponfirelogicbuffshotconfigdata;
		}
	}
}