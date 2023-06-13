using System;

namespace D11.Pjson
{
	public class WeaponAdditionFireLogicSlowAreaDataTable : BaseTable
	{
		private WeaponAdditionFireLogicSlowAreaData weaponadditionfirelogicslowareadata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponadditionfirelogicslowareadata = WeaponAdditionFireLogicSlowAreaData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponAdditionFireLogicSlowAreaData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponAdditionFireLogicSlowAreaData GetTable()
		{
			return weaponadditionfirelogicslowareadata;
		}
	}
}