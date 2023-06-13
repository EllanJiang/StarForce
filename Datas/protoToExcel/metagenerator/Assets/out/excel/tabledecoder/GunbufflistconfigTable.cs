using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GunbufflistconfigTable : BaseTable
	{
		private Dictionary<uint, GunBuffListConfig> gunbufflistconfig_map = new Dictionary<uint, GunBuffListConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				GunBuffListConfig_Array gunbufflistconfig_array = GunBuffListConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in gunbufflistconfig_array.Items)
				{
					gunbufflistconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "GunBuffListConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, GunBuffListConfig> GetTable()
		{
			return gunbufflistconfig_map;
		}

		public GunBuffListConfig GetRecorder(uint key)
		{
			if (!gunbufflistconfig_map.ContainsKey(key))
			{
				return null;
			}
			return gunbufflistconfig_map[key];
		}

	}
}