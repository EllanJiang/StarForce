using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class PlayerexcellencereasonTable : BaseTable
	{
		private PlayerExcellenceReason_Array playerexcellencereason_array = new PlayerExcellenceReason_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				playerexcellencereason_array = PlayerExcellenceReason_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "PlayerExcellenceReason_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public PlayerExcellenceReason_Array GetTable()
		{
			return playerexcellencereason_array;
		}

		public PlayerExcellenceReason GetRecorder(int key)
		{
			if (key >= playerexcellencereason_array.Items.Count)
			{
				return null;
			}
			return playerexcellencereason_array.Items[key];
		}

	}
}