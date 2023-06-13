using System;

namespace D11.Pjson
{
	public class BossMonsterData_HeavyPunchTable : BaseTable
	{
		private BossMonsterData_HeavyPunch bossmonsterdata_heavypunch;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				bossmonsterdata_heavypunch = BossMonsterData_HeavyPunch.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BossMonsterData_HeavyPunch.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BossMonsterData_HeavyPunch GetTable()
		{
			return bossmonsterdata_heavypunch;
		}
	}
}