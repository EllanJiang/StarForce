using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class KillnotifyitemconfigTable : BaseTable
	{
		private Dictionary<uint, KillNotifyItemConfig> killnotifyitemconfig_map = new Dictionary<uint, KillNotifyItemConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				KillNotifyItemConfig_Array killnotifyitemconfig_array = KillNotifyItemConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in killnotifyitemconfig_array.Items)
				{
					killnotifyitemconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "KillNotifyItemConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, KillNotifyItemConfig> GetTable()
		{
			return killnotifyitemconfig_map;
		}

		public KillNotifyItemConfig GetRecorder(uint key)
		{
			if (!killnotifyitemconfig_map.ContainsKey(key))
			{
				return null;
			}
			return killnotifyitemconfig_map[key];
		}

	}
}