using System;

namespace D11.Pjson
{
	public class WeaponPersistenceConfigComponentDataTable : BaseTable
	{
		private WeaponPersistenceConfigComponentData weaponpersistenceconfigcomponentdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponpersistenceconfigcomponentdata = WeaponPersistenceConfigComponentData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponPersistenceConfigComponentData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponPersistenceConfigComponentData GetTable()
		{
			return weaponpersistenceconfigcomponentdata;
		}
	}
}