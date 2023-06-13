using System;
using System.Collections.Generic;
namespace D11.Pbeans
{
	public class BrscenehierarchyconfigTable : BaseTable
	{
		private BRSceneHierarchyConfig_Array brscenehierarchyconfig_array = new BRSceneHierarchyConfig_Array();

		public override void LoadConfig(byte[] buffer, int offset, int length)
		{
			try
			{
				brscenehierarchyconfig_array = BRSceneHierarchyConfig_Array.Parser.ParseFrom(buffer, offset, length);
			}
			catch (Exception ex)
			{
				string errorMsg = "BRSceneHierarchyConfig_Array.LoadConfig Error\n{0}";
				throw new Exception(string.Format(errorMsg, ex.ToString()));
			}
		}

		public BRSceneHierarchyConfig_Array GetTable()
		{
			return brscenehierarchyconfig_array;
		}

		public BRSceneHierarchyConfig GetRecorder(int key)
		{
			if (key >= brscenehierarchyconfig_array.Items.Count)
			{
				return null;
			}
			return brscenehierarchyconfig_array.Items[key];
		}

	}
}