using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class NoviceguiderefineconfigTable : BaseTable
	{
		private Dictionary<uint, NoviceGuideRefineConfig> noviceguiderefineconfig_map = new Dictionary<uint, NoviceGuideRefineConfig>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				NoviceGuideRefineConfig_Array noviceguiderefineconfig_array = NoviceGuideRefineConfig_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in noviceguiderefineconfig_array.Items)
				{
					noviceguiderefineconfig_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "NoviceGuideRefineConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, NoviceGuideRefineConfig> GetTable()
		{
			return noviceguiderefineconfig_map;
		}

		public NoviceGuideRefineConfig GetRecorder(uint key)
		{
			if (!noviceguiderefineconfig_map.ContainsKey(key))
			{
				return null;
			}
			return noviceguiderefineconfig_map[key];
		}

	}
}