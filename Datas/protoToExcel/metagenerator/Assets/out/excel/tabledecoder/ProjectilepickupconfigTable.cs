using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class ProjectilepickupconfigTable : BaseTable
	{
		private Dictionary<uint, ProjectilePickUpConfig> projectilepickupconfig_map = new Dictionary<uint, ProjectilePickUpConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				ProjectilePickUpConfig_Array projectilepickupconfig_array = ProjectilePickUpConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in projectilepickupconfig_array.Items)
				{
					projectilepickupconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "ProjectilePickUpConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, ProjectilePickUpConfig> GetTable()
		{
			return projectilepickupconfig_map;
		}

		public ProjectilePickUpConfig GetRecorder(uint key)
		{
			if (!projectilepickupconfig_map.ContainsKey(key))
			{
				return null;
			}
			return projectilepickupconfig_map[key];
		}

	}
}