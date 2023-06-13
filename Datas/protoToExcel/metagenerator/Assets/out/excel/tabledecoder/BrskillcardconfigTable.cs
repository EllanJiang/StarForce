using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrskillcardconfigTable : BaseTable
	{
		private BRSkillCardConfig_Array brskillcardconfig_array = new BRSkillCardConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				brskillcardconfig_array = BRSkillCardConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BRSkillCardConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BRSkillCardConfig_Array GetTable()
		{
			return brskillcardconfig_array;
		}

		public BRSkillCardConfig GetRecorder(int key)
		{
			if (key >= brskillcardconfig_array.Items.Count)
			{
				return null;
			}
			return brskillcardconfig_array.Items[key];
		}

	}
}