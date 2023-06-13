using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ZombiemonsterdataTable : BaseTable
	{
		private Dictionary<uint, ZombieMonsterData> zombiemonsterdata_map = new Dictionary<uint, ZombieMonsterData>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ZombieMonsterData_Array zombiemonsterdata_array = ZombieMonsterData_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in zombiemonsterdata_array.Items)
				{
					zombiemonsterdata_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ZombieMonsterData_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ZombieMonsterData> GetTable()
		{
			return zombiemonsterdata_map;
		}

		public ZombieMonsterData GetRecorder(uint key)
		{
			if (!zombiemonsterdata_map.ContainsKey(key))
			{
				return null;
			}
			return zombiemonsterdata_map[key];
		}

	}
}