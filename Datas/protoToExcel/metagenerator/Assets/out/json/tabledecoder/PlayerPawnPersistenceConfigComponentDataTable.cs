using System;

namespace D11.Pjson
{
	public class PlayerPawnPersistenceConfigComponentDataTable : BaseTable
	{
		private PlayerPawnPersistenceConfigComponentData playerpawnpersistenceconfigcomponentdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				playerpawnpersistenceconfigcomponentdata = PlayerPawnPersistenceConfigComponentData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "PlayerPawnPersistenceConfigComponentData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public PlayerPawnPersistenceConfigComponentData GetTable()
		{
			return playerpawnpersistenceconfigcomponentdata;
		}
	}
}