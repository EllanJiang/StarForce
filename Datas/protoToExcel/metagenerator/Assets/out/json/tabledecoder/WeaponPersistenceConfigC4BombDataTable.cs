using System;

namespace D11.Pjson
{
	public class WeaponPersistenceConfigC4BombDataTable : BaseTable
	{
		private WeaponPersistenceConfigC4BombData weaponpersistenceconfigc4bombdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponpersistenceconfigc4bombdata = WeaponPersistenceConfigC4BombData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponPersistenceConfigC4BombData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponPersistenceConfigC4BombData GetTable()
		{
			return weaponpersistenceconfigc4bombdata;
		}
	}
}