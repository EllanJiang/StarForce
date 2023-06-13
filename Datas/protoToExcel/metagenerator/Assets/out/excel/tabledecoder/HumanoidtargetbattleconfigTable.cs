using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class HumanoidtargetbattleconfigTable : BaseTable
	{
		private Dictionary<uint, HumanoidTargetBattleConfig> humanoidtargetbattleconfig_map = new Dictionary<uint, HumanoidTargetBattleConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				HumanoidTargetBattleConfig_Array humanoidtargetbattleconfig_array = HumanoidTargetBattleConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in humanoidtargetbattleconfig_array.Items)
				{
					humanoidtargetbattleconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "HumanoidTargetBattleConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, HumanoidTargetBattleConfig> GetTable()
		{
			return humanoidtargetbattleconfig_map;
		}

		public HumanoidTargetBattleConfig GetRecorder(uint key)
		{
			if (!humanoidtargetbattleconfig_map.ContainsKey(key))
			{
				return null;
			}
			return humanoidtargetbattleconfig_map[key];
		}

	}
}