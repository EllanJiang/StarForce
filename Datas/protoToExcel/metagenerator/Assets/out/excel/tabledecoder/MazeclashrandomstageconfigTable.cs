using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class MazeclashrandomstageconfigTable : BaseTable
	{
		private MazeClashRandomStageConfig_Array mazeclashrandomstageconfig_array = new MazeClashRandomStageConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				mazeclashrandomstageconfig_array = MazeClashRandomStageConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "MazeClashRandomStageConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public MazeClashRandomStageConfig_Array GetTable()
		{
			return mazeclashrandomstageconfig_array;
		}

		public MazeClashRandomStageConfig GetRecorder(int key)
		{
			if (key >= mazeclashrandomstageconfig_array.Items.Count)
			{
				return null;
			}
			return mazeclashrandomstageconfig_array.Items[key];
		}

	}
}