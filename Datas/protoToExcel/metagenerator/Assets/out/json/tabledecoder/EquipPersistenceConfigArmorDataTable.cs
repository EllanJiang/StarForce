using System;

namespace D11.Pjson
{
	public class EquipPersistenceConfigArmorDataTable : BaseTable
	{
		private EquipPersistenceConfigArmorData equippersistenceconfigarmordata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				equippersistenceconfigarmordata = EquipPersistenceConfigArmorData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "EquipPersistenceConfigArmorData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public EquipPersistenceConfigArmorData GetTable()
		{
			return equippersistenceconfigarmordata;
		}
	}
}