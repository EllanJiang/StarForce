using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class WeapondescribeattributeTable : BaseTable
	{
		private Dictionary<uint, WeaponDescribeAttribute> weapondescribeattribute_map = new Dictionary<uint, WeaponDescribeAttribute>();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				WeaponDescribeAttribute_Array weapondescribeattribute_array = WeaponDescribeAttribute_Array.Parser.ParseFrom(buffer, offset, length);
				foreach (var item in weapondescribeattribute_array.Items)
				{
					weapondescribeattribute_map.Add(item.Id, item);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "WeaponDescribeAttribute_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public Dictionary<uint, WeaponDescribeAttribute> GetTable()
		{
			return weapondescribeattribute_map;
		}

		public WeaponDescribeAttribute GetRecorder(uint key)
		{
			if (!weapondescribeattribute_map.ContainsKey(key))
			{
				return null;
			}
			return weapondescribeattribute_map[key];
		}

	}
}