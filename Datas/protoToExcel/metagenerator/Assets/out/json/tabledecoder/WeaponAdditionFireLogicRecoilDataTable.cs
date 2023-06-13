using System;

namespace D11.Pjson
{
	public class WeaponAdditionFireLogicRecoilDataTable : BaseTable
	{
		private WeaponAdditionFireLogicRecoilData weaponadditionfirelogicrecoildata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponadditionfirelogicrecoildata = WeaponAdditionFireLogicRecoilData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponAdditionFireLogicRecoilData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponAdditionFireLogicRecoilData GetTable()
		{
			return weaponadditionfirelogicrecoildata;
		}
	}
}