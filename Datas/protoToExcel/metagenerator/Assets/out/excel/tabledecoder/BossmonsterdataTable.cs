using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BossmonsterdataTable : BaseTable
	{
		private Dictionary<uint, BossMonsterData> bossmonsterdata_map = new Dictionary<uint, BossMonsterData>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BossMonsterData_Array bossmonsterdata_array = BossMonsterData_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in bossmonsterdata_array.Items)
				{
					bossmonsterdata_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BossMonsterData_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BossMonsterData> GetTable()
		{
			return bossmonsterdata_map;
		}

		public BossMonsterData GetRecorder(uint key)
		{
			if (!bossmonsterdata_map.ContainsKey(key))
			{
				return null;
			}
			return bossmonsterdata_map[key];
		}

	}
}