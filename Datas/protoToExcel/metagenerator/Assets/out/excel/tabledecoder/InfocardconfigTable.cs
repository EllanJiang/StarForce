using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class InfocardconfigTable : BaseTable
	{
		private Dictionary<uint, InfoCardConfig> infocardconfig_map = new Dictionary<uint, InfoCardConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				InfoCardConfig_Array infocardconfig_array = InfoCardConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in infocardconfig_array.Items)
				{
					infocardconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "InfoCardConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, InfoCardConfig> GetTable()
		{
			return infocardconfig_map;
		}

		public InfoCardConfig GetRecorder(uint key)
		{
			if (!infocardconfig_map.ContainsKey(key))
			{
				return null;
			}
			return infocardconfig_map[key];
		}

	}
}