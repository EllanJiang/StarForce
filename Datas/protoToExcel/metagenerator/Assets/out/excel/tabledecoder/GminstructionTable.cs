using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class GminstructionTable : BaseTable
	{
		private Dictionary<uint, GMInstruction> gminstruction_map = new Dictionary<uint, GMInstruction>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				GMInstruction_Array gminstruction_array = GMInstruction_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in gminstruction_array.Items)
				{
					gminstruction_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "GMInstruction_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, GMInstruction> GetTable()
		{
			return gminstruction_map;
		}

		public GMInstruction GetRecorder(uint key)
		{
			if (!gminstruction_map.ContainsKey(key))
			{
				return null;
			}
			return gminstruction_map[key];
		}

	}
}