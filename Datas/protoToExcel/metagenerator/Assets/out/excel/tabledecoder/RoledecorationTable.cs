using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RoledecorationTable : BaseTable
	{
		private Dictionary<uint, RoleDecoration> roledecoration_map = new Dictionary<uint, RoleDecoration>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				RoleDecoration_Array roledecoration_array = RoleDecoration_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in roledecoration_array.Items)
				{
					roledecoration_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "RoleDecoration_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, RoleDecoration> GetTable()
		{
			return roledecoration_map;
		}

		public RoleDecoration GetRecorder(uint key)
		{
			if (!roledecoration_map.ContainsKey(key))
			{
				return null;
			}
			return roledecoration_map[key];
		}

	}
}