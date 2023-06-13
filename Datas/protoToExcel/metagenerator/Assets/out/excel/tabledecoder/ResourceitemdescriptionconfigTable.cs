using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ResourceitemdescriptionconfigTable : BaseTable
	{
		private Dictionary<uint, ResourceItemDescriptionConfig> resourceitemdescriptionconfig_map = new Dictionary<uint, ResourceItemDescriptionConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ResourceItemDescriptionConfig_Array resourceitemdescriptionconfig_array = ResourceItemDescriptionConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in resourceitemdescriptionconfig_array.Items)
				{
					resourceitemdescriptionconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ResourceItemDescriptionConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ResourceItemDescriptionConfig> GetTable()
		{
			return resourceitemdescriptionconfig_map;
		}

		public ResourceItemDescriptionConfig GetRecorder(uint key)
		{
			if (!resourceitemdescriptionconfig_map.ContainsKey(key))
			{
				return null;
			}
			return resourceitemdescriptionconfig_map[key];
		}

	}
}