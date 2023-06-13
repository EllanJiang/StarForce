using System;

namespace D11.Pjson
{
	public class EquipPersistenceConfigComponentDataTable : BaseTable
	{
		private EquipPersistenceConfigComponentData equippersistenceconfigcomponentdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				equippersistenceconfigcomponentdata = EquipPersistenceConfigComponentData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "EquipPersistenceConfigComponentData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public EquipPersistenceConfigComponentData GetTable()
		{
			return equippersistenceconfigcomponentdata;
		}
	}
}