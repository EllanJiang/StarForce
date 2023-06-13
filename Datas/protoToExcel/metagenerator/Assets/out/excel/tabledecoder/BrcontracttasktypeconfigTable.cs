using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrcontracttasktypeconfigTable : BaseTable
	{
		private Dictionary<uint, BRContractTaskTypeConfig> brcontracttasktypeconfig_map = new Dictionary<uint, BRContractTaskTypeConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BRContractTaskTypeConfig_Array brcontracttasktypeconfig_array = BRContractTaskTypeConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in brcontracttasktypeconfig_array.Items)
				{
					brcontracttasktypeconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BRContractTaskTypeConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BRContractTaskTypeConfig> GetTable()
		{
			return brcontracttasktypeconfig_map;
		}

		public BRContractTaskTypeConfig GetRecorder(uint key)
		{
			if (!brcontracttasktypeconfig_map.ContainsKey(key))
			{
				return null;
			}
			return brcontracttasktypeconfig_map[key];
		}

	}
}