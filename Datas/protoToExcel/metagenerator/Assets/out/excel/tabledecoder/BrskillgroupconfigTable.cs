using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrskillgroupconfigTable : BaseTable
	{
		private BRSkillGroupConfig_Array brskillgroupconfig_array = new BRSkillGroupConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				brskillgroupconfig_array = BRSkillGroupConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BRSkillGroupConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BRSkillGroupConfig_Array GetTable()
		{
			return brskillgroupconfig_array;
		}

		public BRSkillGroupConfig GetRecorder(int key)
		{
			if (key >= brskillgroupconfig_array.Items.Count)
			{
				return null;
			}
			return brskillgroupconfig_array.Items[key];
		}

	}
}