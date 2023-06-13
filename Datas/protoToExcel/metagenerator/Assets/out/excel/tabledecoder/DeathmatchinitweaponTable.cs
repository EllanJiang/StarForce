using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class DeathmatchinitweaponTable : BaseTable
	{
		private DeathMatchInitWeapon_Array deathmatchinitweapon_array = new DeathMatchInitWeapon_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				deathmatchinitweapon_array = DeathMatchInitWeapon_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "DeathMatchInitWeapon_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public DeathMatchInitWeapon_Array GetTable()
		{
			return deathmatchinitweapon_array;
		}

		public DeathMatchInitWeapon GetRecorder(int key)
		{
			if (key >= deathmatchinitweapon_array.Items.Count)
			{
				return null;
			}
			return deathmatchinitweapon_array.Items[key];
		}

	}
}