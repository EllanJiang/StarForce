using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BreathrestorehpconfigTable : BaseTable
	{
		private Dictionary<uint, BreathRestoreHPConfig> breathrestorehpconfig_map = new Dictionary<uint, BreathRestoreHPConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BreathRestoreHPConfig_Array breathrestorehpconfig_array = BreathRestoreHPConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in breathrestorehpconfig_array.Items)
				{
					breathrestorehpconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BreathRestoreHPConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BreathRestoreHPConfig> GetTable()
		{
			return breathrestorehpconfig_map;
		}

		public BreathRestoreHPConfig GetRecorder(uint key)
		{
			if (!breathrestorehpconfig_map.ContainsKey(key))
			{
				return null;
			}
			return breathrestorehpconfig_map[key];
		}

	}
}