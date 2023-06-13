using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class NameidconfigTable : BaseTable
	{
		private Dictionary<uint, NameIdConfig> nameidconfig_map = new Dictionary<uint, NameIdConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				NameIdConfig_Array nameidconfig_array = NameIdConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in nameidconfig_array.Items)
				{
					nameidconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "NameIdConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, NameIdConfig> GetTable()
		{
			return nameidconfig_map;
		}

		public NameIdConfig GetRecorder(uint key)
		{
			if (!nameidconfig_map.ContainsKey(key))
			{
				return null;
			}
			return nameidconfig_map[key];
		}

	}
}