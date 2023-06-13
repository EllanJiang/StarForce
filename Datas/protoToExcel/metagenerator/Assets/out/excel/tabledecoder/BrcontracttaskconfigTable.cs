using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrcontracttaskconfigTable : BaseTable
	{
		private Dictionary<uint, BRContractTaskConfig> brcontracttaskconfig_map = new Dictionary<uint, BRContractTaskConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BRContractTaskConfig_Array brcontracttaskconfig_array = BRContractTaskConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in brcontracttaskconfig_array.Items)
				{
					brcontracttaskconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BRContractTaskConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BRContractTaskConfig> GetTable()
		{
			return brcontracttaskconfig_map;
		}

		public BRContractTaskConfig GetRecorder(uint key)
		{
			if (!brcontracttaskconfig_map.ContainsKey(key))
			{
				return null;
			}
			return brcontracttaskconfig_map[key];
		}

	}
}