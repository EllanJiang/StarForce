using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class Buffdatav2configTable : BaseTable
	{
		private Dictionary<uint, BuffDataV2Config> buffdatav2config_map = new Dictionary<uint, BuffDataV2Config>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BuffDataV2Config_Array buffdatav2config_array = BuffDataV2Config_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in buffdatav2config_array.Items)
				{
					buffdatav2config_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BuffDataV2Config_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BuffDataV2Config> GetTable()
		{
			return buffdatav2config_map;
		}

		public BuffDataV2Config GetRecorder(uint key)
		{
			if (!buffdatav2config_map.ContainsKey(key))
			{
				return null;
			}
			return buffdatav2config_map[key];
		}

	}
}