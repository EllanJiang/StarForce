using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class PreloadgeneraltopoolTable : BaseTable
	{
		private Dictionary<uint, PreLoadGeneralToPool> preloadgeneraltopool_map = new Dictionary<uint, PreLoadGeneralToPool>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				PreLoadGeneralToPool_Array preloadgeneraltopool_array = PreLoadGeneralToPool_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in preloadgeneraltopool_array.Items)
				{
					preloadgeneraltopool_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "PreLoadGeneralToPool_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, PreLoadGeneralToPool> GetTable()
		{
			return preloadgeneraltopool_map;
		}

		public PreLoadGeneralToPool GetRecorder(uint key)
		{
			if (!preloadgeneraltopool_map.ContainsKey(key))
			{
				return null;
			}
			return preloadgeneraltopool_map[key];
		}

	}
}