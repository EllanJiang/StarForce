using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BuffeffectconfigTable : BaseTable
	{
		private Dictionary<uint, BuffEffectConfig> buffeffectconfig_map = new Dictionary<uint, BuffEffectConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BuffEffectConfig_Array buffeffectconfig_array = BuffEffectConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in buffeffectconfig_array.Items)
				{
					buffeffectconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BuffEffectConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BuffEffectConfig> GetTable()
		{
			return buffeffectconfig_map;
		}

		public BuffEffectConfig GetRecorder(uint key)
		{
			if (!buffeffectconfig_map.ContainsKey(key))
			{
				return null;
			}
			return buffeffectconfig_map[key];
		}

	}
}