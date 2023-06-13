using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class PainttypeconfigTable : BaseTable
	{
		private Dictionary<uint, PaintTypeConfig> painttypeconfig_map = new Dictionary<uint, PaintTypeConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				PaintTypeConfig_Array painttypeconfig_array = PaintTypeConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in painttypeconfig_array.Items)
				{
					painttypeconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "PaintTypeConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, PaintTypeConfig> GetTable()
		{
			return painttypeconfig_map;
		}

		public PaintTypeConfig GetRecorder(uint key)
		{
			if (!painttypeconfig_map.ContainsKey(key))
			{
				return null;
			}
			return painttypeconfig_map[key];
		}

	}
}