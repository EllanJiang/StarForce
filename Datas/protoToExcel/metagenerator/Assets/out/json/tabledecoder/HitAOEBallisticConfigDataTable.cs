using System;

namespace D11.Pjson
{
	public class HitAOEBallisticConfigDataTable : BaseTable
	{
		private HitAOEBallisticConfigData hitaoeballisticconfigdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				hitaoeballisticconfigdata = HitAOEBallisticConfigData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "HitAOEBallisticConfigData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public HitAOEBallisticConfigData GetTable()
		{
			return hitaoeballisticconfigdata;
		}
	}
}