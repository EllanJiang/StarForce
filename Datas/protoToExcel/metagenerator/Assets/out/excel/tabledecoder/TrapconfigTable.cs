using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class TrapconfigTable : BaseTable
	{
		private Dictionary<uint, TrapConfig> trapconfig_map = new Dictionary<uint, TrapConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				TrapConfig_Array trapconfig_array = TrapConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in trapconfig_array.Items)
				{
					trapconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "TrapConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, TrapConfig> GetTable()
		{
			return trapconfig_map;
		}

		public TrapConfig GetRecorder(uint key)
		{
			if (!trapconfig_map.ContainsKey(key))
			{
				return null;
			}
			return trapconfig_map[key];
		}

	}
}