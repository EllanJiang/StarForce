using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class EntityproxymodeconfigTable : BaseTable
	{
		private Dictionary<ulong, EntityProxyModeConfig> entityproxymodeconfig_map = new Dictionary<ulong, EntityProxyModeConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				EntityProxyModeConfig_Array entityproxymodeconfig_array = EntityProxyModeConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in entityproxymodeconfig_array.Items)
				{
					entityproxymodeconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "EntityProxyModeConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<ulong, EntityProxyModeConfig> GetTable()
		{
			return entityproxymodeconfig_map;
		}

		public EntityProxyModeConfig GetRecorder(ulong key)
		{
			if (!entityproxymodeconfig_map.ContainsKey(key))
			{
				return null;
			}
			return entityproxymodeconfig_map[key];
		}

	}
}