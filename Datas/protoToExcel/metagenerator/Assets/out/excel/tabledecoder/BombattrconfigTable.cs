using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BombattrconfigTable : BaseTable
	{
		private Dictionary<uint, BombAttrConfig> bombattrconfig_map = new Dictionary<uint, BombAttrConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BombAttrConfig_Array bombattrconfig_array = BombAttrConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in bombattrconfig_array.Items)
				{
					bombattrconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BombAttrConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BombAttrConfig> GetTable()
		{
			return bombattrconfig_map;
		}

		public BombAttrConfig GetRecorder(uint key)
		{
			if (!bombattrconfig_map.ContainsKey(key))
			{
				return null;
			}
			return bombattrconfig_map[key];
		}

	}
}