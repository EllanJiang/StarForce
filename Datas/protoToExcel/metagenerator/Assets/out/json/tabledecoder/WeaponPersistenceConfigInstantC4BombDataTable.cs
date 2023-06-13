using System;

namespace D11.Pjson
{
	public class WeaponPersistenceConfigInstantC4BombDataTable : BaseTable
	{
		private WeaponPersistenceConfigInstantC4BombData weaponpersistenceconfiginstantc4bombdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponpersistenceconfiginstantc4bombdata = WeaponPersistenceConfigInstantC4BombData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponPersistenceConfigInstantC4BombData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponPersistenceConfigInstantC4BombData GetTable()
		{
			return weaponpersistenceconfiginstantc4bombdata;
		}
	}
}