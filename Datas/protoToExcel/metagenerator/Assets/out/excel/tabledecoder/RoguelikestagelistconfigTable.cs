using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RoguelikestagelistconfigTable : BaseTable
	{
		private Dictionary<uint, RogueLikeStageListConfig> roguelikestagelistconfig_map = new Dictionary<uint, RogueLikeStageListConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				RogueLikeStageListConfig_Array roguelikestagelistconfig_array = RogueLikeStageListConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in roguelikestagelistconfig_array.Items)
				{
					roguelikestagelistconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "RogueLikeStageListConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, RogueLikeStageListConfig> GetTable()
		{
			return roguelikestagelistconfig_map;
		}

		public RogueLikeStageListConfig GetRecorder(uint key)
		{
			if (!roguelikestagelistconfig_map.ContainsKey(key))
			{
				return null;
			}
			return roguelikestagelistconfig_map[key];
		}

	}
}