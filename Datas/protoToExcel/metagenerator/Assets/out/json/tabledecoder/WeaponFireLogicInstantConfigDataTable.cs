using System;

namespace D11.Pjson
{
	public class WeaponFireLogicInstantConfigDataTable : BaseTable
	{
		private WeaponFireLogicInstantConfigData weaponfirelogicinstantconfigdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponfirelogicinstantconfigdata = WeaponFireLogicInstantConfigData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponFireLogicInstantConfigData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponFireLogicInstantConfigData GetTable()
		{
			return weaponfirelogicinstantconfigdata;
		}
	}
}