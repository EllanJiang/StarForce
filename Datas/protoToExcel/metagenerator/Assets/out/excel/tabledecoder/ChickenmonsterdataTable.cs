using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ChickenmonsterdataTable : BaseTable
	{
		private Dictionary<uint, ChickenMonsterData> chickenmonsterdata_map = new Dictionary<uint, ChickenMonsterData>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ChickenMonsterData_Array chickenmonsterdata_array = ChickenMonsterData_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in chickenmonsterdata_array.Items)
				{
					chickenmonsterdata_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ChickenMonsterData_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ChickenMonsterData> GetTable()
		{
			return chickenmonsterdata_map;
		}

		public ChickenMonsterData GetRecorder(uint key)
		{
			if (!chickenmonsterdata_map.ContainsKey(key))
			{
				return null;
			}
			return chickenmonsterdata_map[key];
		}

	}
}