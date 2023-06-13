using System;

namespace D11.Pjson
{
	public class WeaponPersistenceConfigInstantDataTable : BaseTable
	{
		private WeaponPersistenceConfigInstantData weaponpersistenceconfiginstantdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponpersistenceconfiginstantdata = WeaponPersistenceConfigInstantData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponPersistenceConfigInstantData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponPersistenceConfigInstantData GetTable()
		{
			return weaponpersistenceconfiginstantdata;
		}
	}
}