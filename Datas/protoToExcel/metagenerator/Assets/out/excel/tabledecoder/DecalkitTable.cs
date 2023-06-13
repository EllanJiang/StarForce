using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class DecalkitTable : BaseTable
	{
		private Dictionary<uint, DecalKit> decalkit_map = new Dictionary<uint, DecalKit>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				DecalKit_Array decalkit_array = DecalKit_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in decalkit_array.Items)
				{
					decalkit_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "DecalKit_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, DecalKit> GetTable()
		{
			return decalkit_map;
		}

		public DecalKit GetRecorder(uint key)
		{
			if (!decalkit_map.ContainsKey(key))
			{
				return null;
			}
			return decalkit_map[key];
		}

	}
}