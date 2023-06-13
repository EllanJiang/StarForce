using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class LastkillvfxconfigTable : BaseTable
	{
		private Dictionary<uint, LastKillVFXConfig> lastkillvfxconfig_map = new Dictionary<uint, LastKillVFXConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				LastKillVFXConfig_Array lastkillvfxconfig_array = LastKillVFXConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in lastkillvfxconfig_array.Items)
				{
					lastkillvfxconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "LastKillVFXConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, LastKillVFXConfig> GetTable()
		{
			return lastkillvfxconfig_map;
		}

		public LastKillVFXConfig GetRecorder(uint key)
		{
			if (!lastkillvfxconfig_map.ContainsKey(key))
			{
				return null;
			}
			return lastkillvfxconfig_map[key];
		}

	}
}