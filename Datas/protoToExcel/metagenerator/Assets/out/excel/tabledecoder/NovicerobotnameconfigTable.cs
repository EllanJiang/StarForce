using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class NovicerobotnameconfigTable : BaseTable
	{
		private NoviceRobotNameConfig_Array novicerobotnameconfig_array = new NoviceRobotNameConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				novicerobotnameconfig_array = NoviceRobotNameConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "NoviceRobotNameConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public NoviceRobotNameConfig_Array GetTable()
		{
			return novicerobotnameconfig_array;
		}

		public NoviceRobotNameConfig GetRecorder(int key)
		{
			if (key >= novicerobotnameconfig_array.Items.Count)
			{
				return null;
			}
			return novicerobotnameconfig_array.Items[key];
		}

	}
}