using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class VoxdataTable : BaseTable
	{
		private Dictionary<uint, VoxData> voxdata_map = new Dictionary<uint, VoxData>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				VoxData_Array voxdata_array = VoxData_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in voxdata_array.Items)
				{
					voxdata_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "VoxData_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, VoxData> GetTable()
		{
			return voxdata_map;
		}

		public VoxData GetRecorder(uint key)
		{
			if (!voxdata_map.ContainsKey(key))
			{
				return null;
			}
			return voxdata_map[key];
		}

	}
}