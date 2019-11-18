using System;
using System.Collections.Generic;
using System.Linq;

namespace Tango.Meta.Database
{
	[Serializable]
	public partial class Schema : MetaNamedElement
	{
		public Dictionary<string, Table> Tables { get; private set; }
		public Dictionary<string, View> Views { get; private set; }
		public Dictionary<string, Procedure> Procedures { get; private set; }
		public Dictionary<string, Function> Functions { get; private set; }
		public Dictionary<string, TableFunction> TableFunctions { get; private set; }

		public Schema()
		{
			Tables = new Dictionary<string, Table>();
			Views = new Dictionary<string, View>();
			Procedures = new Dictionary<string, Procedure>();
			Functions = new Dictionary<string, Function>();
			TableFunctions = new Dictionary<string, TableFunction>();
		}
	}

	[Serializable]
	public partial class Table : MetaNamedElement
	{
		public string Owner { get; set; }
		public string Description { get; set; }
		public bool Identity { get; set; }
		public Dictionary<string, Column> Columns { get; private set; }
		public Dictionary<string, Index> Indexes { get; private set; }
		public Dictionary<string, ForeignKey> ForeignKeys { get; private set; }
		public Dictionary<string, Trigger> Triggers { get; private set; }
		public PrimaryKey PrimaryKey { get; set; }
		public Schema Schema { get; set; }

		public Table()
		{
			Columns = new Dictionary<string, Column>();
			ForeignKeys = new Dictionary<string, ForeignKey>();
			Triggers = new Dictionary<string, Trigger>();
			Indexes = new Dictionary<string, Index>();
			//Identity = true;
		}
	}

	[Serializable]
	public partial class Index : MetaNamedElement
	{
		public string[] Columns { get; set; }
		public Table Table { get; set; }

		public string Cluster { get; set; }
		public bool AllowPageLocks { get; set; }
		public bool AllowRowLocks { get; set; }
		public bool IgnoreDupKey { get; set; }
		public bool IsUnique { get; set; }

	}
	[Serializable]
	public partial class Column : MetaNamedElement
	{
		public IMetaPrimitiveType Type { get; set; }
		public bool Nullable { get; set; }
		public string DefaultValue { get; set; }
		public string ComputedText { get; set; }
		public string Description { get; set; }

		public string ForeignKeyName { get; set; }
		public bool IsPrimaryKey { get; set; }
		public bool Identity { get; set; }
		public Table Table { get; set; }
	}

	[Serializable]
	public partial class ViewColumn : MetaNamedElement
	{
		public IMetaPrimitiveType Type { get; set; }
		public bool Nullable { get; set; }
		public string Description { get; set; }
	}

	[Serializable]
	public partial class PrimaryKey : MetaNamedElement
	{
		public string[] Columns { get; set; }
		public Table Table { get; set; }
	}

	[Serializable]
	public class ForeignKey : MetaNamedElement
	{
		public string RefTable { get; set; }
		public bool IsEnabled { get; set; }
		public string[] Columns { get; set; }
		public string[] RefTableColumns { get; set; }
		public DeleteOption DeleteOption { get; set; }
		public Table Table { get; set; }
	}

	public enum DeleteOption
	{
		Cascade,
		SetNull,
		Restrict
	}

	[Serializable]
	public partial class Procedure : MetaNamedElement
	{
		public string Text { get; set; }
		public Dictionary<string, Parameter> Parameters { get; private set; }

		public Procedure()
		{
			Parameters = new Dictionary<string, Parameter>();
		}
	}

	[Serializable]
	public partial class Function : MetaNamedElement
	{
		public string Text { get; set; }
		public Dictionary<string, Parameter> Parameters { get; private set; }

		public Function()
		{
			Parameters = new Dictionary<string, Parameter>();
		}
	}

	[Serializable]
	public partial class TableFunction : MetaNamedElement
	{
		public string Text { get; set; }
		public Dictionary<string, Parameter> Parameters { get; private set; }
		public Dictionary<string, ViewColumn> Columns { get; private set; }
		public string ReturnType { get; set; }

		public TableFunction()
		{
			Parameters = new Dictionary<string, Parameter>();
			Columns = new Dictionary<string, ViewColumn>();
		}
	}

	[Serializable]
	public partial class View : MetaNamedElement
	{
		public string Text { get; set; }
		public Dictionary<string, ViewColumn> Columns { get; private set; }
		public Dictionary<string, Trigger> Triggers { get; private set; }

		public View()
		{
			Columns = new Dictionary<string, ViewColumn>();
			Triggers = new Dictionary<string, Trigger>();
		}
	}

	[Serializable]
	public partial class Trigger : MetaNamedElement
	{
		public string Text { get; set; }
		public string Owner { get; set; }
	}

	[Serializable]
	public class Parameter : MetaNamedElement
	{
		public IMetaPrimitiveType Type { get; set; }
	}


	/*public class ProcedureDetails
	{
		public string ProcedureName { get; set; }
		public string ReturnType { get; set; }
        public bool IsList { get; set; }
		public Dictionary<string, string> Columns { get; set; }
	}*/

	public interface IOnTableGenerateLogic
	{
		void Generate(Table table);
	}
}