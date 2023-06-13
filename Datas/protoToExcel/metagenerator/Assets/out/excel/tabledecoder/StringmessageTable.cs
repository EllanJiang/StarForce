using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class StringmessageTable : BaseTable
	{
		private Dictionary<string, StringMessage> stringmessage_map = new Dictionary<string, StringMessage>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				StringMessage_Array stringmessage_array = StringMessage_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in stringmessage_array.Items)
				{
					stringmessage_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "StringMessage_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<string, StringMessage> GetTable()
		{
			return stringmessage_map;
		}

		public StringMessage GetRecorder(string key)
		{
			if (!stringmessage_map.ContainsKey(key))
			{
				return null;
			}
			return stringmessage_map[key];
		}

	}
}