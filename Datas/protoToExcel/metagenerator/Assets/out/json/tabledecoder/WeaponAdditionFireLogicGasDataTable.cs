using System;

namespace D11.Pjson
{
	public class WeaponAdditionFireLogicGasDataTable : BaseTable
	{
		private WeaponAdditionFireLogicGasData weaponadditionfirelogicgasdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponadditionfirelogicgasdata = WeaponAdditionFireLogicGasData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponAdditionFireLogicGasData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponAdditionFireLogicGasData GetTable()
		{
			return weaponadditionfirelogicgasdata;
		}
	}
}