using System;

namespace D11.Pjson
{
	public class WeaponPersistenceConfigProjectileDataTable : BaseTable
	{
		private WeaponPersistenceConfigProjectileData weaponpersistenceconfigprojectiledata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponpersistenceconfigprojectiledata = WeaponPersistenceConfigProjectileData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponPersistenceConfigProjectileData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponPersistenceConfigProjectileData GetTable()
		{
			return weaponpersistenceconfigprojectiledata;
		}
	}
}