using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class DeathmatchrandweaponTable : BaseTable
	{
		private DeathMatchRandWeapon_Array deathmatchrandweapon_array = new DeathMatchRandWeapon_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				deathmatchrandweapon_array = DeathMatchRandWeapon_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "DeathMatchRandWeapon_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public DeathMatchRandWeapon_Array GetTable()
		{
			return deathmatchrandweapon_array;
		}

		public DeathMatchRandWeapon GetRecorder(int key)
		{
			if (key >= deathmatchrandweapon_array.Items.Count)
			{
				return null;
			}
			return deathmatchrandweapon_array.Items[key];
		}

	}
}