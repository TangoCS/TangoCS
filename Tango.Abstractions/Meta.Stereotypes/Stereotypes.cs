using System;
using System.Collections.Generic;
using System.Text;
using Tango.Meta.Database;

namespace Tango.Meta
{
    public class SHistKey : MetaStereotype
    {
        public List<string> ColumnNames = new List<string>();
        public SHistKey(string keycolumn)
        {
            ColumnNames.Add(keycolumn);
            Name = "SHistKey";
        }
		public SHistKey(List<string> keycolumns)
		{
			ColumnNames.AddRange(keycolumns);
			Name = "SHistKey";
		}
	}

    public class SRegistry : MetaStereotype
    {
        public string RegistryTypeName { get; }
        public SRegistry(string registryTypeName)
        {
            RegistryTypeName = registryTypeName;
        }
    }
}
