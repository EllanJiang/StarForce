using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BattlemoneyrewardTable : BaseTable
	{
		private Dictionary<uint, BattleMoneyReward> battlemoneyreward_map = new Dictionary<uint, BattleMoneyReward>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BattleMoneyReward_Array battlemoneyreward_array = BattleMoneyReward_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in battlemoneyreward_array.Items)
				{
					battlemoneyreward_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BattleMoneyReward_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BattleMoneyReward> GetTable()
		{
			return battlemoneyreward_map;
		}

		public BattleMoneyReward GetRecorder(uint key)
		{
			if (!battlemoneyreward_map.ContainsKey(key))
			{
				return null;
			}
			return battlemoneyreward_map[key];
		}

	}
}