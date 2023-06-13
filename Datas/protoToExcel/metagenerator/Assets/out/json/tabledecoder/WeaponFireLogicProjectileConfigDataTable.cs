using System;

namespace D11.Pjson
{
	public class WeaponFireLogicProjectileConfigDataTable : BaseTable
	{
		private WeaponFireLogicProjectileConfigData weaponfirelogicprojectileconfigdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponfirelogicprojectileconfigdata = WeaponFireLogicProjectileConfigData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponFireLogicProjectileConfigData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponFireLogicProjectileConfigData GetTable()
		{
			return weaponfirelogicprojectileconfigdata;
		}
	}
}