using System;

namespace D11.Pjson
{
	public class GameModeConfigKeepWatchTable : BaseTable
	{
		private GameModeConfigKeepWatch gamemodeconfigkeepwatch;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				gamemodeconfigkeepwatch = GameModeConfigKeepWatch.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "GameModeConfigKeepWatch.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public GameModeConfigKeepWatch GetTable()
		{
			return gamemodeconfigkeepwatch;
		}
	}
}