using System;

namespace D11.Pjson
{
	public class WeaponAdditionFireLogicGrenadeScanDataTable : BaseTable
	{
		private WeaponAdditionFireLogicGrenadeScanData weaponadditionfirelogicgrenadescandata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponadditionfirelogicgrenadescandata = WeaponAdditionFireLogicGrenadeScanData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponAdditionFireLogicGrenadeScanData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponAdditionFireLogicGrenadeScanData GetTable()
		{
			return weaponadditionfirelogicgrenadescandata;
		}
	}
}