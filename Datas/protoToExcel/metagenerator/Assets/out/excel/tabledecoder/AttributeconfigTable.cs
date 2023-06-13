using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class AttributeconfigTable : BaseTable
	{
		private Dictionary<uint, AttributeConfig> attributeconfig_map = new Dictionary<uint, AttributeConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				AttributeConfig_Array attributeconfig_array = AttributeConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in attributeconfig_array.Items)
				{
					attributeconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "AttributeConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, AttributeConfig> GetTable()
		{
			return attributeconfig_map;
		}

		public AttributeConfig GetRecorder(uint key)
		{
			if (!attributeconfig_map.ContainsKey(key))
			{
				return null;
			}
			return attributeconfig_map[key];
		}

	}
}