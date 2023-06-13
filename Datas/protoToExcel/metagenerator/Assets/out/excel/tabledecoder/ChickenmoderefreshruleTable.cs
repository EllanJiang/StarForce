using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ChickenmoderefreshruleTable : BaseTable
	{
		private Dictionary<uint, ChickenModeRefreshRule> chickenmoderefreshrule_map = new Dictionary<uint, ChickenModeRefreshRule>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ChickenModeRefreshRule_Array chickenmoderefreshrule_array = ChickenModeRefreshRule_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in chickenmoderefreshrule_array.Items)
				{
					chickenmoderefreshrule_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ChickenModeRefreshRule_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ChickenModeRefreshRule> GetTable()
		{
			return chickenmoderefreshrule_map;
		}

		public ChickenModeRefreshRule GetRecorder(uint key)
		{
			if (!chickenmoderefreshrule_map.ContainsKey(key))
			{
				return null;
			}
			return chickenmoderefreshrule_map[key];
		}

	}
}