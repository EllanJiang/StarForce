using System;

namespace D11.Pjson
{
	public class WeaponFireLogicSniperInstantConfigDataTable : BaseTable
	{
		private WeaponFireLogicSniperInstantConfigData weaponfirelogicsniperinstantconfigdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponfirelogicsniperinstantconfigdata = WeaponFireLogicSniperInstantConfigData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponFireLogicSniperInstantConfigData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponFireLogicSniperInstantConfigData GetTable()
		{
			return weaponfirelogicsniperinstantconfigdata;
		}
	}
}