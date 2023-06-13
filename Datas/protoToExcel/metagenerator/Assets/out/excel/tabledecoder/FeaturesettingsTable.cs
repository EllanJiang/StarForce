using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class FeaturesettingsTable : BaseTable
	{
		private Dictionary<int, FeatureSettings> featuresettings_map = new Dictionary<int, FeatureSettings>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				FeatureSettings_Array featuresettings_array = FeatureSettings_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in featuresettings_array.Items)
				{
					featuresettings_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "FeatureSettings_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<int, FeatureSettings> GetTable()
		{
			return featuresettings_map;
		}

		public FeatureSettings GetRecorder(int key)
		{
			if (!featuresettings_map.ContainsKey(key))
			{
				return null;
			}
			return featuresettings_map[key];
		}

	}
}