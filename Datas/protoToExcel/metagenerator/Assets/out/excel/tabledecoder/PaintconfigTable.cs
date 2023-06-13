using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class PaintconfigTable : BaseTable
	{
		private Dictionary<uint, PaintConfig> paintconfig_map = new Dictionary<uint, PaintConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				PaintConfig_Array paintconfig_array = PaintConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in paintconfig_array.Items)
				{
					paintconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "PaintConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, PaintConfig> GetTable()
		{
			return paintconfig_map;
		}

		public PaintConfig GetRecorder(uint key)
		{
			if (!paintconfig_map.ContainsKey(key))
			{
				return null;
			}
			return paintconfig_map[key];
		}

	}
}