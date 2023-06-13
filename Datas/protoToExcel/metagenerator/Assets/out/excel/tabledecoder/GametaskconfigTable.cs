using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GametaskconfigTable : BaseTable
	{
		private Dictionary<uint, GameTaskConfig> gametaskconfig_map = new Dictionary<uint, GameTaskConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				GameTaskConfig_Array gametaskconfig_array = GameTaskConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in gametaskconfig_array.Items)
				{
					gametaskconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "GameTaskConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, GameTaskConfig> GetTable()
		{
			return gametaskconfig_map;
		}

		public GameTaskConfig GetRecorder(uint key)
		{
			if (!gametaskconfig_map.ContainsKey(key))
			{
				return null;
			}
			return gametaskconfig_map[key];
		}

	}
}