using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class WeapondescribeattributequalityconfigTable : BaseTable
	{
		private Dictionary<uint, WeaponDescribeAttributeQualityConfig> weapondescribeattributequalityconfig_map = new Dictionary<uint, WeaponDescribeAttributeQualityConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				WeaponDescribeAttributeQualityConfig_Array weapondescribeattributequalityconfig_array = WeaponDescribeAttributeQualityConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in weapondescribeattributequalityconfig_array.Items)
				{
					weapondescribeattributequalityconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponDescribeAttributeQualityConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, WeaponDescribeAttributeQualityConfig> GetTable()
		{
			return weapondescribeattributequalityconfig_map;
		}

		public WeaponDescribeAttributeQualityConfig GetRecorder(uint key)
		{
			if (!weapondescribeattributequalityconfig_map.ContainsKey(key))
			{
				return null;
			}
			return weapondescribeattributequalityconfig_map[key];
		}

	}
}