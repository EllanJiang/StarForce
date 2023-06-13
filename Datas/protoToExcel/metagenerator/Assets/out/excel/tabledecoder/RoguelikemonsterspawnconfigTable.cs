using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RoguelikemonsterspawnconfigTable : BaseTable
	{
		private Dictionary<uint, RogueLikeMonsterSpawnConfig> roguelikemonsterspawnconfig_map = new Dictionary<uint, RogueLikeMonsterSpawnConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				RogueLikeMonsterSpawnConfig_Array roguelikemonsterspawnconfig_array = RogueLikeMonsterSpawnConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in roguelikemonsterspawnconfig_array.Items)
				{
					roguelikemonsterspawnconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "RogueLikeMonsterSpawnConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, RogueLikeMonsterSpawnConfig> GetTable()
		{
			return roguelikemonsterspawnconfig_map;
		}

		public RogueLikeMonsterSpawnConfig GetRecorder(uint key)
		{
			if (!roguelikemonsterspawnconfig_map.ContainsKey(key))
			{
				return null;
			}
			return roguelikemonsterspawnconfig_map[key];
		}

	}
}