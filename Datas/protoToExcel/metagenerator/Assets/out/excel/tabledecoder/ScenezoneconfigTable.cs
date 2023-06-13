using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ScenezoneconfigTable : BaseTable
	{
		private Dictionary<string, SceneZoneConfig> scenezoneconfig_map = new Dictionary<string, SceneZoneConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SceneZoneConfig_Array scenezoneconfig_array = SceneZoneConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in scenezoneconfig_array.Items)
				{
					scenezoneconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SceneZoneConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<string, SceneZoneConfig> GetTable()
		{
			return scenezoneconfig_map;
		}

		public SceneZoneConfig GetRecorder(string key)
		{
			if (!scenezoneconfig_map.ContainsKey(key))
			{
				return null;
			}
			return scenezoneconfig_map[key];
		}

	}
}