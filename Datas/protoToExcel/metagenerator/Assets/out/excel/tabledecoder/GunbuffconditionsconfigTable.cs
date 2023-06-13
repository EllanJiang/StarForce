using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GunbuffconditionsconfigTable : BaseTable
	{
		private Dictionary<uint, GunBuffConditionsConfig> gunbuffconditionsconfig_map = new Dictionary<uint, GunBuffConditionsConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				GunBuffConditionsConfig_Array gunbuffconditionsconfig_array = GunBuffConditionsConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in gunbuffconditionsconfig_array.Items)
				{
					gunbuffconditionsconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "GunBuffConditionsConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, GunBuffConditionsConfig> GetTable()
		{
			return gunbuffconditionsconfig_map;
		}

		public GunBuffConditionsConfig GetRecorder(uint key)
		{
			if (!gunbuffconditionsconfig_map.ContainsKey(key))
			{
				return null;
			}
			return gunbuffconditionsconfig_map[key];
		}

	}
}