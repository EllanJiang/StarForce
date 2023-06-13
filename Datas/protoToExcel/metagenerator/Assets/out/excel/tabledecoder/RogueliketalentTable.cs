using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RogueliketalentTable : BaseTable
	{
		private Dictionary<uint, RogueLikeTalent> rogueliketalent_map = new Dictionary<uint, RogueLikeTalent>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				RogueLikeTalent_Array rogueliketalent_array = RogueLikeTalent_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in rogueliketalent_array.Items)
				{
					rogueliketalent_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "RogueLikeTalent_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, RogueLikeTalent> GetTable()
		{
			return rogueliketalent_map;
		}

		public RogueLikeTalent GetRecorder(uint key)
		{
			if (!rogueliketalent_map.ContainsKey(key))
			{
				return null;
			}
			return rogueliketalent_map[key];
		}

	}
}