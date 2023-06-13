using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class LikegroupconfigTable : BaseTable
	{
		private Dictionary<uint, LikeGroupConfig> likegroupconfig_map = new Dictionary<uint, LikeGroupConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				LikeGroupConfig_Array likegroupconfig_array = LikeGroupConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in likegroupconfig_array.Items)
				{
					likegroupconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "LikeGroupConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, LikeGroupConfig> GetTable()
		{
			return likegroupconfig_map;
		}

		public LikeGroupConfig GetRecorder(uint key)
		{
			if (!likegroupconfig_map.ContainsKey(key))
			{
				return null;
			}
			return likegroupconfig_map[key];
		}

	}
}