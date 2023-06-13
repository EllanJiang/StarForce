using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ScenepathconfigTable : BaseTable
	{
		private Dictionary<uint, ScenePathConfig> scenepathconfig_map = new Dictionary<uint, ScenePathConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ScenePathConfig_Array scenepathconfig_array = ScenePathConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in scenepathconfig_array.Items)
				{
					scenepathconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ScenePathConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ScenePathConfig> GetTable()
		{
			return scenepathconfig_map;
		}

		public ScenePathConfig GetRecorder(uint key)
		{
			if (!scenepathconfig_map.ContainsKey(key))
			{
				return null;
			}
			return scenepathconfig_map[key];
		}

	}
}