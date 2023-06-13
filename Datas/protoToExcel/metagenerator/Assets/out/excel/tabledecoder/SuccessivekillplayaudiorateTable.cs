using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class SuccessivekillplayaudiorateTable : BaseTable
	{
		private Dictionary<int, SuccessiveKillPlayAudioRate> successivekillplayaudiorate_map = new Dictionary<int, SuccessiveKillPlayAudioRate>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SuccessiveKillPlayAudioRate_Array successivekillplayaudiorate_array = SuccessiveKillPlayAudioRate_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in successivekillplayaudiorate_array.Items)
				{
					successivekillplayaudiorate_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SuccessiveKillPlayAudioRate_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<int, SuccessiveKillPlayAudioRate> GetTable()
		{
			return successivekillplayaudiorate_map;
		}

		public SuccessiveKillPlayAudioRate GetRecorder(int key)
		{
			if (!successivekillplayaudiorate_map.ContainsKey(key))
			{
				return null;
			}
			return successivekillplayaudiorate_map[key];
		}

	}
}