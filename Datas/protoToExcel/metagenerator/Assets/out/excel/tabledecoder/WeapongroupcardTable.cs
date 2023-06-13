using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class WeapongroupcardTable : BaseTable
	{
		private Dictionary<uint, WeaponGroupCard> weapongroupcard_map = new Dictionary<uint, WeaponGroupCard>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				WeaponGroupCard_Array weapongroupcard_array = WeaponGroupCard_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in weapongroupcard_array.Items)
				{
					weapongroupcard_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponGroupCard_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, WeaponGroupCard> GetTable()
		{
			return weapongroupcard_map;
		}

		public WeaponGroupCard GetRecorder(uint key)
		{
			if (!weapongroupcard_map.ContainsKey(key))
			{
				return null;
			}
			return weapongroupcard_map[key];
		}

	}
}