using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class TacticalvoiceTable : BaseTable
	{
		private Dictionary<uint, TacticalVoice> tacticalvoice_map = new Dictionary<uint, TacticalVoice>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				TacticalVoice_Array tacticalvoice_array = TacticalVoice_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in tacticalvoice_array.Items)
				{
					tacticalvoice_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "TacticalVoice_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, TacticalVoice> GetTable()
		{
			return tacticalvoice_map;
		}

		public TacticalVoice GetRecorder(uint key)
		{
			if (!tacticalvoice_map.ContainsKey(key))
			{
				return null;
			}
			return tacticalvoice_map[key];
		}

	}
}