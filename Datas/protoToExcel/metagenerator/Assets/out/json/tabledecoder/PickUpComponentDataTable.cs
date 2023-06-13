using System;

namespace D11.Pjson
{
	public class PickUpComponentDataTable : BaseTable
	{
		private PickUpComponentData pickupcomponentdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				pickupcomponentdata = PickUpComponentData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "PickUpComponentData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public PickUpComponentData GetTable()
		{
			return pickupcomponentdata;
		}
	}
}