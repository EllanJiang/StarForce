using System;

namespace D11.Pjson
{
	public class WeaponPersistenceConfigMeleeDataTable : BaseTable
	{
		private WeaponPersistenceConfigMeleeData weaponpersistenceconfigmeleedata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponpersistenceconfigmeleedata = WeaponPersistenceConfigMeleeData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponPersistenceConfigMeleeData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponPersistenceConfigMeleeData GetTable()
		{
			return weaponpersistenceconfigmeleedata;
		}
	}
}