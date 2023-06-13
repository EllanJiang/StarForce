using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class RoguelikerandomstageconfigTable : BaseTable
	{
		private RogueLikeRandomStageConfig_Array roguelikerandomstageconfig_array = new RogueLikeRandomStageConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				roguelikerandomstageconfig_array = RogueLikeRandomStageConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "RogueLikeRandomStageConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public RogueLikeRandomStageConfig_Array GetTable()
		{
			return roguelikerandomstageconfig_array;
		}

		public RogueLikeRandomStageConfig GetRecorder(int key)
		{
			if (key >= roguelikerandomstageconfig_array.Items.Count)
			{
				return null;
			}
			return roguelikerandomstageconfig_array.Items[key];
		}

	}
}