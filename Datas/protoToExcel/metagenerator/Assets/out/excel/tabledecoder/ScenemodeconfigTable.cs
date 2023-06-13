using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ScenemodeconfigTable : BaseTable
	{
		private Dictionary<uint, SceneModeConfig> scenemodeconfig_map = new Dictionary<uint, SceneModeConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				SceneModeConfig_Array scenemodeconfig_array = SceneModeConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in scenemodeconfig_array.Items)
				{
					scenemodeconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "SceneModeConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, SceneModeConfig> GetTable()
		{
			return scenemodeconfig_map;
		}

		public SceneModeConfig GetRecorder(uint key)
		{
			if (!scenemodeconfig_map.ContainsKey(key))
			{
				return null;
			}
			return scenemodeconfig_map[key];
		}

	}
}