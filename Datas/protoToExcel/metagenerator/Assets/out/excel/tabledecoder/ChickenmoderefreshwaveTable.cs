using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ChickenmoderefreshwaveTable : BaseTable
	{
		private Dictionary<uint, ChickenModeRefreshWave> chickenmoderefreshwave_map = new Dictionary<uint, ChickenModeRefreshWave>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ChickenModeRefreshWave_Array chickenmoderefreshwave_array = ChickenModeRefreshWave_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in chickenmoderefreshwave_array.Items)
				{
					chickenmoderefreshwave_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ChickenModeRefreshWave_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ChickenModeRefreshWave> GetTable()
		{
			return chickenmoderefreshwave_map;
		}

		public ChickenModeRefreshWave GetRecorder(uint key)
		{
			if (!chickenmoderefreshwave_map.ContainsKey(key))
			{
				return null;
			}
			return chickenmoderefreshwave_map[key];
		}

	}
}