using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class MusicboxmodelconfigTable : BaseTable
	{
		private Dictionary<uint, MusicBoxModelConfig> musicboxmodelconfig_map = new Dictionary<uint, MusicBoxModelConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				MusicBoxModelConfig_Array musicboxmodelconfig_array = MusicBoxModelConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in musicboxmodelconfig_array.Items)
				{
					musicboxmodelconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "MusicBoxModelConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, MusicBoxModelConfig> GetTable()
		{
			return musicboxmodelconfig_map;
		}

		public MusicBoxModelConfig GetRecorder(uint key)
		{
			if (!musicboxmodelconfig_map.ContainsKey(key))
			{
				return null;
			}
			return musicboxmodelconfig_map[key];
		}

	}
}