using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class SkinabilitysystemconfigTable : BaseTable
	{
		private Dictionary<uint, SkinAbilitySystemConfig> skinabilitysystemconfig_map = new Dictionary<uint, SkinAbilitySystemConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SkinAbilitySystemConfig_Array skinabilitysystemconfig_array = SkinAbilitySystemConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in skinabilitysystemconfig_array.Items)
				{
					skinabilitysystemconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SkinAbilitySystemConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, SkinAbilitySystemConfig> GetTable()
		{
			return skinabilitysystemconfig_map;
		}

		public SkinAbilitySystemConfig GetRecorder(uint key)
		{
			if (!skinabilitysystemconfig_map.ContainsKey(key))
			{
				return null;
			}
			return skinabilitysystemconfig_map[key];
		}

	}
}