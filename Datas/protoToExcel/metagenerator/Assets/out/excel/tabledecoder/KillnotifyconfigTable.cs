using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class KillnotifyconfigTable : BaseTable
	{
		private Dictionary<uint, KillNotifyConfig> killnotifyconfig_map = new Dictionary<uint, KillNotifyConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				KillNotifyConfig_Array killnotifyconfig_array = KillNotifyConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in killnotifyconfig_array.Items)
				{
					killnotifyconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "KillNotifyConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, KillNotifyConfig> GetTable()
		{
			return killnotifyconfig_map;
		}

		public KillNotifyConfig GetRecorder(uint key)
		{
			if (!killnotifyconfig_map.ContainsKey(key))
			{
				return null;
			}
			return killnotifyconfig_map[key];
		}

	}
}