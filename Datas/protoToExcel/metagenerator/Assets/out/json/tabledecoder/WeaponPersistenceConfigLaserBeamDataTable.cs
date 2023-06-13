using System;

namespace D11.Pjson
{
	public class WeaponPersistenceConfigLaserBeamDataTable : BaseTable
	{
		private WeaponPersistenceConfigLaserBeamData weaponpersistenceconfiglaserbeamdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponpersistenceconfiglaserbeamdata = WeaponPersistenceConfigLaserBeamData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponPersistenceConfigLaserBeamData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponPersistenceConfigLaserBeamData GetTable()
		{
			return weaponpersistenceconfiglaserbeamdata;
		}
	}
}