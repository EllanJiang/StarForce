using System;

namespace StarForce.Editor.DataTableTools
{
    public sealed partial class DataTableGenerator
    {
        private sealed class Data
        {
            public DateTime CreateTime
            {
                get;
                set;
            }

            public string NameSpace
            {
                get;
                set;
            }

            public string ClassName
            {
                get;
                set;
            }
        }
    }
}
