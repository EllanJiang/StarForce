using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RecommendbuyconfigTable : BaseTable
	{
		private Dictionary<uint, RecommendBuyConfig> recommendbuyconfig_map = new Dictionary<uint, RecommendBuyConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				RecommendBuyConfig_Array recommendbuyconfig_array = RecommendBuyConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in recommendbuyconfig_array.Items)
				{
					recommendbuyconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "RecommendBuyConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, RecommendBuyConfig> GetTable()
		{
			return recommendbuyconfig_map;
		}

		public RecommendBuyConfig GetRecorder(uint key)
		{
			if (!recommendbuyconfig_map.ContainsKey(key))
			{
				return null;
			}
			return recommendbuyconfig_map[key];
		}

	}
}