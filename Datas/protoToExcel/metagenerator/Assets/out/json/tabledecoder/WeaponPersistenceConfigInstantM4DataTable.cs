using System;

namespace D11.Pjson
{
	public class WeaponPersistenceConfigInstantM4DataTable : BaseTable
	{
		private WeaponPersistenceConfigInstantM4Data weaponpersistenceconfiginstantm4data;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponpersistenceconfiginstantm4data = WeaponPersistenceConfigInstantM4Data.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponPersistenceConfigInstantM4Data.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponPersistenceConfigInstantM4Data GetTable()
		{
			return weaponpersistenceconfiginstantm4data;
		}
	}
}