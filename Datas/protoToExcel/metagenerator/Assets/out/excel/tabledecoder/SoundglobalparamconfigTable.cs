using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class SoundglobalparamconfigTable : BaseTable
	{
		private SoundGlobalParamConfig_Array soundglobalparamconfig_array = new SoundGlobalParamConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				soundglobalparamconfig_array = SoundGlobalParamConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "SoundGlobalParamConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public SoundGlobalParamConfig_Array GetTable()
		{
			return soundglobalparamconfig_array;
		}

		public SoundGlobalParamConfig GetRecorder(int key)
		{
			if (key >= soundglobalparamconfig_array.Items.Count)
			{
				return null;
			}
			return soundglobalparamconfig_array.Items[key];
		}

	}
}