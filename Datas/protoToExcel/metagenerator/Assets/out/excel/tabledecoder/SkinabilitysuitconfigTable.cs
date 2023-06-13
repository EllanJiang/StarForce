using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class SkinabilitysuitconfigTable : BaseTable
	{
		private Dictionary<uint, SkinAbilitySuitConfig> skinabilitysuitconfig_map = new Dictionary<uint, SkinAbilitySuitConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SkinAbilitySuitConfig_Array skinabilitysuitconfig_array = SkinAbilitySuitConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in skinabilitysuitconfig_array.Items)
				{
					skinabilitysuitconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SkinAbilitySuitConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, SkinAbilitySuitConfig> GetTable()
		{
			return skinabilitysuitconfig_map;
		}

		public SkinAbilitySuitConfig GetRecorder(uint key)
		{
			if (!skinabilitysuitconfig_map.ContainsKey(key))
			{
				return null;
			}
			return skinabilitysuitconfig_map[key];
		}

	}
}