using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RolevoxdataTable : BaseTable
	{
		private Dictionary<string, RoleVoxData> rolevoxdata_map = new Dictionary<string, RoleVoxData>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				RoleVoxData_Array rolevoxdata_array = RoleVoxData_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in rolevoxdata_array.Items)
				{
					rolevoxdata_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "RoleVoxData_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<string, RoleVoxData> GetTable()
		{
			return rolevoxdata_map;
		}

		public RoleVoxData GetRecorder(string key)
		{
			if (!rolevoxdata_map.ContainsKey(key))
			{
				return null;
			}
			return rolevoxdata_map[key];
		}

	}
}