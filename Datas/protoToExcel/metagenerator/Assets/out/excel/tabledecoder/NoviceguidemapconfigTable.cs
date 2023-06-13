using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class NoviceguidemapconfigTable : BaseTable
	{
		private Dictionary<uint, NoviceGuideMapConfig> noviceguidemapconfig_map = new Dictionary<uint, NoviceGuideMapConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				NoviceGuideMapConfig_Array noviceguidemapconfig_array = NoviceGuideMapConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in noviceguidemapconfig_array.Items)
				{
					noviceguidemapconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "NoviceGuideMapConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, NoviceGuideMapConfig> GetTable()
		{
			return noviceguidemapconfig_map;
		}

		public NoviceGuideMapConfig GetRecorder(uint key)
		{
			if (!noviceguidemapconfig_map.ContainsKey(key))
			{
				return null;
			}
			return noviceguidemapconfig_map[key];
		}

	}
}