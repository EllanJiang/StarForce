using System;

namespace D11.Pjson
{
	public class EquipPersistenceConfigDefuseTimeDataTable : BaseTable
	{
		private EquipPersistenceConfigDefuseTimeData equippersistenceconfigdefusetimedata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				equippersistenceconfigdefusetimedata = EquipPersistenceConfigDefuseTimeData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "EquipPersistenceConfigDefuseTimeData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public EquipPersistenceConfigDefuseTimeData GetTable()
		{
			return equippersistenceconfigdefusetimedata;
		}
	}
}