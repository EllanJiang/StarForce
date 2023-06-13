using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class DecalpaintconfigTable : BaseTable
	{
		private Dictionary<uint, DecalPaintConfig> decalpaintconfig_map = new Dictionary<uint, DecalPaintConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				DecalPaintConfig_Array decalpaintconfig_array = DecalPaintConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in decalpaintconfig_array.Items)
				{
					decalpaintconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "DecalPaintConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, DecalPaintConfig> GetTable()
		{
			return decalpaintconfig_map;
		}

		public DecalPaintConfig GetRecorder(uint key)
		{
			if (!decalpaintconfig_map.ContainsKey(key))
			{
				return null;
			}
			return decalpaintconfig_map[key];
		}

	}
}