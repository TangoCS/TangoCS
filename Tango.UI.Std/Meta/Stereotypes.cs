using System;
using System.Collections.Generic;
using System.Text;
using Tango.Meta.Database;

namespace Tango.Meta
{
    public class SHistKey : MetaStereotype//, IOnTableGenerateLogic
    {
        public List<string> ColumnNames = new List<string>();
        public SHistKey(string keycolumn)
        {
            ColumnNames.Add(keycolumn);
            Name = "SHistKey";
        }
        //public void Generate(Table table)
        //{
        //    if (!Name.IsEmpty())
        //        table.AssignStereotype(this);
        //}
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
