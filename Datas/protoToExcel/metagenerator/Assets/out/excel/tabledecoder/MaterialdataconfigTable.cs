using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class MaterialdataconfigTable : BaseTable
	{
		private Dictionary<uint, MaterialDataConfig> materialdataconfig_map = new Dictionary<uint, MaterialDataConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				MaterialDataConfig_Array materialdataconfig_array = MaterialDataConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in materialdataconfig_array.Items)
				{
					materialdataconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "MaterialDataConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, MaterialDataConfig> GetTable()
		{
			return materialdataconfig_map;
		}

		public MaterialDataConfig GetRecorder(uint key)
		{
			if (!materialdataconfig_map.ContainsKey(key))
			{
				return null;
			}
			return materialdataconfig_map[key];
		}

	}
}