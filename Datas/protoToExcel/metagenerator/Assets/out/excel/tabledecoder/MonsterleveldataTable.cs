using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class MonsterleveldataTable : BaseTable
	{
		private Dictionary<uint, MonsterLevelData> monsterleveldata_map = new Dictionary<uint, MonsterLevelData>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				MonsterLevelData_Array monsterleveldata_array = MonsterLevelData_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in monsterleveldata_array.Items)
				{
					monsterleveldata_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "MonsterLevelData_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, MonsterLevelData> GetTable()
		{
			return monsterleveldata_map;
		}

		public MonsterLevelData GetRecorder(uint key)
		{
			if (!monsterleveldata_map.ContainsKey(key))
			{
				return null;
			}
			return monsterleveldata_map[key];
		}

	}
}