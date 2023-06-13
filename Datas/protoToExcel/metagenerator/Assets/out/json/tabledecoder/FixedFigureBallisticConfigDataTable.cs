using System;

namespace D11.Pjson
{
	public class FixedFigureBallisticConfigDataTable : BaseTable
	{
		private FixedFigureBallisticConfigData fixedfigureballisticconfigdata;

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				fixedfigureballisticconfigdata = FixedFigureBallisticConfigData.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "FixedFigureBallisticConfigData.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public FixedFigureBallisticConfigData GetTable()
		{
			return fixedfigureballisticconfigdata;
		}
	}
}