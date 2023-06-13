using System;

namespace D11.Pjson
{
	public class WeaponAdditionFireLogicAPSDataTable : BaseTable
	{
		private WeaponAdditionFireLogicAPSData weaponadditionfirelogicapsdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponadditionfirelogicapsdata = WeaponAdditionFireLogicAPSData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponAdditionFireLogicAPSData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponAdditionFireLogicAPSData GetTable()
		{
			return weaponadditionfirelogicapsdata;
		}
	}
}