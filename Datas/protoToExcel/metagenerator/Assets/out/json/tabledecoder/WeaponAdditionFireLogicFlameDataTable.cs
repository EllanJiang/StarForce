using System;

namespace D11.Pjson
{
	public class WeaponAdditionFireLogicFlameDataTable : BaseTable
	{
		private WeaponAdditionFireLogicFlameData weaponadditionfirelogicflamedata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponadditionfirelogicflamedata = WeaponAdditionFireLogicFlameData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponAdditionFireLogicFlameData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponAdditionFireLogicFlameData GetTable()
		{
			return weaponadditionfirelogicflamedata;
		}
	}
}