using System;

namespace D11.Pjson
{
	public class WeaponFireLogicHpShieldConfigDataTable : BaseTable
	{
		private WeaponFireLogicHpShieldConfigData weaponfirelogichpshieldconfigdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponfirelogichpshieldconfigdata = WeaponFireLogicHpShieldConfigData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponFireLogicHpShieldConfigData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponFireLogicHpShieldConfigData GetTable()
		{
			return weaponfirelogichpshieldconfigdata;
		}
	}
}