using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrareaconfigTable : BaseTable
	{
		private Dictionary<uint, BRAreaConfig> brareaconfig_map = new Dictionary<uint, BRAreaConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BRAreaConfig_Array brareaconfig_array = BRAreaConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in brareaconfig_array.Items)
				{
					brareaconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BRAreaConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BRAreaConfig> GetTable()
		{
			return brareaconfig_map;
		}

		public BRAreaConfig GetRecorder(uint key)
		{
			if (!brareaconfig_map.ContainsKey(key))
			{
				return null;
			}
			return brareaconfig_map[key];
		}

	}
}