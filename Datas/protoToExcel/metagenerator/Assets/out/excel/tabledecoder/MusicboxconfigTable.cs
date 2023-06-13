using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class MusicboxconfigTable : BaseTable
	{
		private Dictionary<ulong, MusicBoxConfig> musicboxconfig_map = new Dictionary<ulong, MusicBoxConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				MusicBoxConfig_Array musicboxconfig_array = MusicBoxConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in musicboxconfig_array.Items)
				{
					musicboxconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "MusicBoxConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<ulong, MusicBoxConfig> GetTable()
		{
			return musicboxconfig_map;
		}

		public MusicBoxConfig GetRecorder(ulong key)
		{
			if (!musicboxconfig_map.ContainsKey(key))
			{
				return null;
			}
			return musicboxconfig_map[key];
		}

	}
}