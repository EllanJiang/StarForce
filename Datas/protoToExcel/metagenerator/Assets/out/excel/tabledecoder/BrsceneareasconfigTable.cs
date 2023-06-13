using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrsceneareasconfigTable : BaseTable
	{
		private Dictionary<uint, BRSceneAreasConfig> brsceneareasconfig_map = new Dictionary<uint, BRSceneAreasConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				BRSceneAreasConfig_Array brsceneareasconfig_array = BRSceneAreasConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in brsceneareasconfig_array.Items)
				{
					brsceneareasconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "BRSceneAreasConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, BRSceneAreasConfig> GetTable()
		{
			return brsceneareasconfig_map;
		}

		public BRSceneAreasConfig GetRecorder(uint key)
		{
			if (!brsceneareasconfig_map.ContainsKey(key))
			{
				return null;
			}
			return brsceneareasconfig_map[key];
		}

	}
}