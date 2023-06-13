using System;

namespace D11.Pjson
{
	public class WeaponPersistenceConfigInstantSYDataTable : BaseTable
	{
		private WeaponPersistenceConfigInstantSYData weaponpersistenceconfiginstantsydata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponpersistenceconfiginstantsydata = WeaponPersistenceConfigInstantSYData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponPersistenceConfigInstantSYData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponPersistenceConfigInstantSYData GetTable()
		{
			return weaponpersistenceconfiginstantsydata;
		}
	}
}