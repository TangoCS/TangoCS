using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Meta.Database
{
	[Serializable]
	public partial class Schema
	{
		public Dictionary<string, Table> Tables { get; private set; }
		public Dictionary<string, View> Views { get; private set; }
		public Dictionary<string, Procedure> Procedures { get; private set; }

		public Schema()
		{
			Tables = new Dictionary<string, Table>();
			Views = new Dictionary<string, View>();
			Procedures = new Dictionary<string, Procedure>();
		}
	}

	[Serializable]
	public class Table
	{
		public string Name { get; set; }
		public string Owner { get; set; }
		public string Description { get; set; }
		public bool Identity { get; set; }
		public Dictionary<string, Column> Columns { get; private set; }
		public Dictionary<string, ForeignKey> ForeignKeys { get; private set; }
		public Dictionary<string, Trigger> Triggers { get; private set; }
		public PrimaryKey PrimaryKey { get; set; }

		public Table()
		{
			Columns = new Dictionary<string, Column>();
			ForeignKeys = new Dictionary<string, ForeignKey>();
			Triggers = new Dictionary<string, Trigger>();
			Owner = "dbo";
		}	
	}

	[Serializable]
	public class Column
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public bool Nullable { get; set; }
		public string DefaultValue { get; set; }
		public string ComputedText { get; set; }
		public string Description { get; set; }

		public string ForeignKeyName { get; set; }
		public bool IsPrimaryKey { get; set; }
	}

	[Serializable]
	public class PrimaryKey
	{
		public string Name { get; set; }
		public string[] Columns { get; set; }
	}

	[Serializable]
	public class ForeignKey
	{
		public string Name { get; set; }
		public string RefTable { get; set; }
		public string[] Columns { get; set; }
		public string[] RefTableColumns { get; set; }
		public DeleteOption DeleteOption { get; set; }
	}

	public enum DeleteOption
	{
		Cascade,
		SetNull,
		Restrict
	}

	[Serializable]
	public class Procedure
	{
		public string Name { get; set; }
		public string Text { get; set; }
		public Dictionary<string, Parameter> Parameters { get; private set; }

		public Procedure()
		{
			Parameters = new Dictionary<string, Parameter>();
		}
	}

	[Serializable]
	public class View
	{
		public string Name { get; set; }
		public string Text { get; set; }
		public Dictionary<string, Trigger> Triggers { get; private set; }

		public View()
		{
			Triggers = new Dictionary<string, Trigger>();
		}
	}

	[Serializable]
	public class Trigger
	{
		public string Name { get; set; }
		public string Text { get; set; }
	}

	[Serializable]
	public class Parameter
	{
		public string Name { get; set; }
		public string Type { get; set; }
	}
}