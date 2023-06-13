using System;

namespace D11.Pjson
{
	public class WeaponFireLogicC4BombConfigDataTable : BaseTable
	{
		private WeaponFireLogicC4BombConfigData weaponfirelogicc4bombconfigdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				weaponfirelogicc4bombconfigdata = WeaponFireLogicC4BombConfigData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponFireLogicC4BombConfigData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public WeaponFireLogicC4BombConfigData GetTable()
		{
			return weaponfirelogicc4bombconfigdata;
		}
	}
}