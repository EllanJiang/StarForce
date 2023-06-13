using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class AchievementtypeTable : BaseTable
	{
		private Dictionary<uint, AchievementType> achievementtype_map = new Dictionary<uint, AchievementType>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				AchievementType_Array achievementtype_array = AchievementType_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in achievementtype_array.Items)
				{
					achievementtype_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "AchievementType_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, AchievementType> GetTable()
		{
			return achievementtype_map;
		}

		public AchievementType GetRecorder(uint key)
		{
			if (!achievementtype_map.ContainsKey(key))
			{
				return null;
			}
			return achievementtype_map[key];
		}

	}
}