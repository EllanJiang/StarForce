using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BigheadconfigTable : BaseTable
	{
		private Dictionary<uint, BigHeadConfig> bigheadconfig_map = new Dictionary<uint, BigHeadConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BigHeadConfig_Array bigheadconfig_array = BigHeadConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in bigheadconfig_array.Items)
				{
					bigheadconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BigHeadConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BigHeadConfig> GetTable()
		{
			return bigheadconfig_map;
		}

		public BigHeadConfig GetRecorder(uint key)
		{
			if (!bigheadconfig_map.ContainsKey(key))
			{
				return null;
			}
			return bigheadconfig_map[key];
		}

	}
}