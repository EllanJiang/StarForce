using System;

namespace D11.Pjson
{
	public class PickUpConfigComponentDataTable : BaseTable
	{
		private PickUpConfigComponentData pickupconfigcomponentdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				pickupconfigcomponentdata = PickUpConfigComponentData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "PickUpConfigComponentData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public PickUpConfigComponentData GetTable()
		{
			return pickupconfigcomponentdata;
		}
	}
}