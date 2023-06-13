using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class PlayerhangupconfigTable : BaseTable
	{
		private Dictionary<uint, PlayerHangUpConfig> playerhangupconfig_map = new Dictionary<uint, PlayerHangUpConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				PlayerHangUpConfig_Array playerhangupconfig_array = PlayerHangUpConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in playerhangupconfig_array.Items)
				{
					playerhangupconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "PlayerHangUpConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, PlayerHangUpConfig> GetTable()
		{
			return playerhangupconfig_map;
		}

		public PlayerHangUpConfig GetRecorder(uint key)
		{
			if (!playerhangupconfig_map.ContainsKey(key))
			{
				return null;
			}
			return playerhangupconfig_map[key];
		}

	}
}