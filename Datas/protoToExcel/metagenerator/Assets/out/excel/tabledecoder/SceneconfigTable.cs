using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class SceneconfigTable : BaseTable
	{
		private Dictionary<uint, SceneConfig> sceneconfig_map = new Dictionary<uint, SceneConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SceneConfig_Array sceneconfig_array = SceneConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in sceneconfig_array.Items)
				{
					sceneconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SceneConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, SceneConfig> GetTable()
		{
			return sceneconfig_map;
		}

		public SceneConfig GetRecorder(uint key)
		{
			if (!sceneconfig_map.ContainsKey(key))
			{
				return null;
			}
			return sceneconfig_map[key];
		}

	}
}