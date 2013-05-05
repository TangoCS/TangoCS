using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Microsoft.SqlServer.Management.Smo;

namespace Nephrite.Meta
{
	/*public interface IMetaDataBase : IMetaElement
	{
		Dictionary<string, IMetaSchema> Schemas { get; }
	}

	public interface IMetaSchema : IMetaElement
	{
		IMetaSchema Parent { get; }
		Dictionary<string, IMetaSchema> Schemas { get; }

		Dictionary<string, IMetaColumnSet> ColumnSets { get; }
	}

	public interface IMetaColumnSet : IMetaElement
	{
		Dictionary<string, IMetaPersistentColumn> PersistentColumns { get; }
		Dictionary<string, IMetaComputedColumn> ComputedColumns { get; }
	}

	public interface IMetaTable : IMetaColumnSet
	{

	}

	public interface IMetaView : IMetaColumnSet
	{

	}

	public static class MetaTableExtensions
	{
		public static List<IMetaPersistentColumn> PrimaryKey(this IMetaTable table)
		{
			return table.PersistentColumns.Values.Where(o => o.IsPrimaryKey).ToList();
		}
	}

	public interface IMetaColumn { }
	public interface IMetaPersistentColumn 
	{
		bool IsPrimaryKey { get; }
		string DefaultValue { get; }
		IMetaSQLDataType SQLDataType { get; }
	}

	public interface IMetaComputedColumn : IMetaColumn
	{

	}

	public interface IMetaSimpleColumn : IMetaPersistentColumn
	{
		bool IsIdentity { get; }
		int Length { get; }
		int Precision { get; }
		int Scale { get; }
	}

	public enum DeleteRule
	{
		Restrict,
		SetNull,
		Cascade
	}

	public interface IMetaFKColumn : IMetaPersistentColumn
	{
		DeleteRule DeleteRule { get; }
	}

	public interface IMetaSQLDataType : IMetaElement
	{
		DataType SQLServerDataType { get; }
		 
	}*/

}