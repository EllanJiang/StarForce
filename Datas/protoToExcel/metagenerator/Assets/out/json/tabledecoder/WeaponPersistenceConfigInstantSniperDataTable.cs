using System;

namespace D11.Pjson
{
	public class WeaponPersistenceConfigInstantSniperDataTable : BaseTable
	{
		private WeaponPersistenceConfigInstantSniperData weaponpersistenceconfiginstantsniperdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponpersistenceconfiginstantsniperdata = WeaponPersistenceConfigInstantSniperData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponPersistenceConfigInstantSniperData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponPersistenceConfigInstantSniperData GetTable()
		{
			return weaponpersistenceconfiginstantsniperdata;
		}
	}
}