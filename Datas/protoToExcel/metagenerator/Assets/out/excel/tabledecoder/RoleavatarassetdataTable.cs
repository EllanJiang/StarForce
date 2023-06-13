using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RoleavatarassetdataTable : BaseTable
	{
		private Dictionary<uint, RoleAvatarAssetData> roleavatarassetdata_map = new Dictionary<uint, RoleAvatarAssetData>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				RoleAvatarAssetData_Array roleavatarassetdata_array = RoleAvatarAssetData_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in roleavatarassetdata_array.Items)
				{
					roleavatarassetdata_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "RoleAvatarAssetData_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, RoleAvatarAssetData> GetTable()
		{
			return roleavatarassetdata_map;
		}

		public RoleAvatarAssetData GetRecorder(uint key)
		{
			if (!roleavatarassetdata_map.ContainsKey(key))
			{
				return null;
			}
			return roleavatarassetdata_map[key];
		}

	}
}