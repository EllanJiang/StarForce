using System;

namespace D11.Pjson
{
	public class TrackballPersistenceConfigComponentDataTable : BaseTable
	{
		private TrackballPersistenceConfigComponentData trackballpersistenceconfigcomponentdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				trackballpersistenceconfigcomponentdata = TrackballPersistenceConfigComponentData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "TrackballPersistenceConfigComponentData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public TrackballPersistenceConfigComponentData GetTable()
		{
			return trackballpersistenceconfigcomponentdata;
		}
	}
}