using System;

namespace D11.Pjson
{
	public class WeaponHitSceneDecalComponentDataTable : BaseTable
	{
		private WeaponHitSceneDecalComponentData weaponhitscenedecalcomponentdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponhitscenedecalcomponentdata = WeaponHitSceneDecalComponentData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponHitSceneDecalComponentData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponHitSceneDecalComponentData GetTable()
		{
			return weaponhitscenedecalcomponentdata;
		}
	}
}