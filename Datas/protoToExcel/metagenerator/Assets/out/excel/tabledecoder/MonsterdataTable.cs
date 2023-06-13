using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class MonsterdataTable : BaseTable
	{
		private Dictionary<uint, MonsterData> monsterdata_map = new Dictionary<uint, MonsterData>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				MonsterData_Array monsterdata_array = MonsterData_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in monsterdata_array.Items)
				{
					monsterdata_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "MonsterData_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, MonsterData> GetTable()
		{
			return monsterdata_map;
		}

		public MonsterData GetRecorder(uint key)
		{
			if (!monsterdata_map.ContainsKey(key))
			{
				return null;
			}
			return monsterdata_map[key];
		}

	}
}