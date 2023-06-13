using System;

namespace D11.Pjson
{
	public class WeaponFireLogicMeleeConfigDataTable : BaseTable
	{
		private WeaponFireLogicMeleeConfigData weaponfirelogicmeleeconfigdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponfirelogicmeleeconfigdata = WeaponFireLogicMeleeConfigData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponFireLogicMeleeConfigData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponFireLogicMeleeConfigData GetTable()
		{
			return weaponfirelogicmeleeconfigdata;
		}
	}
}