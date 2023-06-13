using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RoleenterscenevoiceTable : BaseTable
	{
		private Dictionary<uint, RoleEnterSceneVoice> roleenterscenevoice_map = new Dictionary<uint, RoleEnterSceneVoice>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				RoleEnterSceneVoice_Array roleenterscenevoice_array = RoleEnterSceneVoice_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in roleenterscenevoice_array.Items)
				{
					roleenterscenevoice_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "RoleEnterSceneVoice_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, RoleEnterSceneVoice> GetTable()
		{
			return roleenterscenevoice_map;
		}

		public RoleEnterSceneVoice GetRecorder(uint key)
		{
			if (!roleenterscenevoice_map.ContainsKey(key))
			{
				return null;
			}
			return roleenterscenevoice_map[key];
		}

	}
}