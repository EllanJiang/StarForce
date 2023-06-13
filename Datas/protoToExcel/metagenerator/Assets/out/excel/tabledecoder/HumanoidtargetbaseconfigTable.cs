using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class HumanoidtargetbaseconfigTable : BaseTable
	{
		private Dictionary<uint, HumanoidTargetBaseConfig> humanoidtargetbaseconfig_map = new Dictionary<uint, HumanoidTargetBaseConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				HumanoidTargetBaseConfig_Array humanoidtargetbaseconfig_array = HumanoidTargetBaseConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in humanoidtargetbaseconfig_array.Items)
				{
					humanoidtargetbaseconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "HumanoidTargetBaseConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, HumanoidTargetBaseConfig> GetTable()
		{
			return humanoidtargetbaseconfig_map;
		}

		public HumanoidTargetBaseConfig GetRecorder(uint key)
		{
			if (!humanoidtargetbaseconfig_map.ContainsKey(key))
			{
				return null;
			}
			return humanoidtargetbaseconfig_map[key];
		}

	}
}