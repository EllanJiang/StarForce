using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class PerformancesetTable : BaseTable
	{
		private Dictionary<uint, PerformanceSet> performanceset_map = new Dictionary<uint, PerformanceSet>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				PerformanceSet_Array performanceset_array = PerformanceSet_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in performanceset_array.Items)
				{
					performanceset_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "PerformanceSet_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, PerformanceSet> GetTable()
		{
			return performanceset_map;
		}

		public PerformanceSet GetRecorder(uint key)
		{
			if (!performanceset_map.ContainsKey(key))
			{
				return null;
			}
			return performanceset_map[key];
		}

	}
}