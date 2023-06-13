using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class SkingameplayconfigTable : BaseTable
	{
		private Dictionary<uint, SkinGamePlayConfig> skingameplayconfig_map = new Dictionary<uint, SkinGamePlayConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SkinGamePlayConfig_Array skingameplayconfig_array = SkinGamePlayConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in skingameplayconfig_array.Items)
				{
					skingameplayconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SkinGamePlayConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, SkinGamePlayConfig> GetTable()
		{
			return skingameplayconfig_map;
		}

		public SkinGamePlayConfig GetRecorder(uint key)
		{
			if (!skingameplayconfig_map.ContainsKey(key))
			{
				return null;
			}
			return skingameplayconfig_map[key];
		}

	}
}