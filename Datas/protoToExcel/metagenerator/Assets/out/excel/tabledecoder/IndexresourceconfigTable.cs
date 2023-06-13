using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class IndexresourceconfigTable : BaseTable
	{
		private Dictionary<uint, IndexResourceConfig> indexresourceconfig_map = new Dictionary<uint, IndexResourceConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				IndexResourceConfig_Array indexresourceconfig_array = IndexResourceConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in indexresourceconfig_array.Items)
				{
					indexresourceconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "IndexResourceConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, IndexResourceConfig> GetTable()
		{
			return indexresourceconfig_map;
		}

		public IndexResourceConfig GetRecorder(uint key)
		{
			if (!indexresourceconfig_map.ContainsKey(key))
			{
				return null;
			}
			return indexresourceconfig_map[key];
		}

	}
}