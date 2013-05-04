using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Management.Smo;
using Nephrite.Web;

namespace Nephrite.Metamodel.DbSync
{
	internal class Column
	{
		internal string Name { get; set; }
		internal DataType Type { get; set; }
		internal bool Nullable { get; set; }
		internal string DefaultValue { get; set; }
		internal string ComputedText { get; set; }
		internal string Description { get; set; }
		internal void Sync(Table table, Microsoft.SqlServer.Management.Smo.Table t)
		{
			if (Type.SqlDataType == SqlDataType.NVarCharMax && Type.MaximumLength == 0)
				Type.MaximumLength = -1;
			if (!ComputedText.IsEmpty() && t.Columns.Contains(Name))
			{
				table.Model.Log("DROP COMPUTED COLUMN " + Name);
				t.Columns[Name].Drop();
			}
			if (!t.Columns.Contains(Name))
			{
				table.Model.Log("ADD COLUMN " + Name + (ComputedText.IsEmpty() ? (" " + Type.ToString() + (Nullable ? " NULL" : " NOT NULL")) : (" AS (" + ComputedText + ")")));
				var col = new Microsoft.SqlServer.Management.Smo.Column(t, Name, Type) { Nullable = Nullable };
				col.Computed = !ComputedText.IsEmpty();
				col.ComputedText = ComputedText.IsEmpty() ? "" : ComputedText;
				if (!String.IsNullOrEmpty(DefaultValue))
				{
					table.Model.Log("SET DEFAULT: " + DefaultValue);
					DefaultConstraint dc = col.AddDefaultConstraint();
					dc.Text = DefaultValue;
				}
				if (!Description.IsEmpty())
				{
					var desc = col.ExtendedProperties["MS_Description"];
					if (desc == null)
						col.ExtendedProperties.Add(new ExtendedProperty(col, "MS_Description", Description));
					else
						desc.Value = Description;
				}
				t.Columns.Add(col);
			}
			else
			{
				var c = t.Columns[Name];
				if (!IsDataTypesEquals(c.DataType, Type) || c.Nullable != Nullable || c.ComputedText != (ComputedText ?? ""))
				{
					if (!t.Indexes.Cast<Microsoft.SqlServer.Management.Smo.Index>().Any(o => o.FilterDefinition.ToLower().Contains(c.Name.ToLower())))
					{
						if (c.Nullable && !Nullable)
						{
							string checkSQL = String.Format("SELECT COUNT(1) FROM {0} WHERE {1} IS NULL", t.Name, Name);
							if (Convert.ToInt32(table.Model.ServerConnection.ExecuteScalar(checkSQL)) > 0)
							{
								if (DefaultValue.IsEmpty())
									throw new Exception("Для свойства " + t.Name + "." + Name + " необходимо задать значение по умолчанию в БД, т.к. в таблице имеются строки с пустыми значениями.");
								string updateSQL = "UPDATE [" + table.Name + "] SET [" + Name + "] = " + DefaultValue + " WHERE [" + Name + "] IS NULL";
								table.Model.Log(updateSQL);
								table.Model.ServerConnection.ExecuteNonQuery(updateSQL);
							}
						}
						// Если на столбец навешаны индексы, надо их удалить
						foreach (var index in t.Indexes.Cast<Microsoft.SqlServer.Management.Smo.Index>().Where(o => o.IndexedColumns.Contains(Name)).ToList())
							index.Drop();
						table.Model.Log("ALTER COLUMN " + Name + " " + Type.ToString() + (Nullable ? " NULL" : " NOT NULL"));
						c.Nullable = Nullable;
						c.DataType = Type;
						if (c.DataType.SqlDataType == SqlDataType.Decimal)
						{
							c.DataType.NumericPrecision = Type.NumericPrecision;
							c.DataType.NumericScale = Type.NumericScale;
						}
					}
					else
					{
						table.Model.Log("SKIP ALTER COLUMN " + Name + " (ссылки из вычислимых столбцов или индексов с фильтром)");
					}
				}
				if (!String.IsNullOrEmpty(DefaultValue))
				{
					DefaultConstraint dc = c.DefaultConstraint;
					if (dc == null)
					{
						table.Model.Log("SET DEFAULT: " + DefaultValue);
						dc = c.AddDefaultConstraint();
					}
					dc.Text = DefaultValue;
					table.Model.Log("ALTER DEFAULT: " + DefaultValue);
				}
				if (!Description.IsEmpty())
				{
					var desc = c.ExtendedProperties["MS_Description"];
					if (desc == null)
						c.ExtendedProperties.Add(new ExtendedProperty(c, "MS_Description", Description));
					else
						desc.Value = Description;
				}
			}
		}
		bool IsDataTypesEquals(DataType dt1, DataType dt2)
		{
			if (dt1.SqlDataType != dt2.SqlDataType)
				return false;
			
			switch (dt1.SqlDataType)
			{
				case SqlDataType.BigInt:
					return true;
				case SqlDataType.Binary:
					return dt1.MaximumLength == dt2.MaximumLength;
				case SqlDataType.Bit:
					return true;
				case SqlDataType.Char:
					return dt1.MaximumLength == dt2.MaximumLength;
				case SqlDataType.Date:
					return true;
				case SqlDataType.DateTime:
					return true;
				case SqlDataType.DateTime2:
					return true;
				case SqlDataType.DateTimeOffset:
					return true;
				case SqlDataType.Decimal:
					return dt1.NumericPrecision == dt2.NumericPrecision && dt1.NumericScale == dt2.NumericScale;
				case SqlDataType.Float:
					return true;
				case SqlDataType.Geography:
					return true;
				case SqlDataType.Geometry:
					return true;
				case SqlDataType.HierarchyId:
					return true;
				case SqlDataType.Image:
					return true;
				case SqlDataType.Int:
					return true;
				case SqlDataType.Money:
					return true;
				case SqlDataType.NChar:
					return dt1.MaximumLength == dt2.MaximumLength;
				case SqlDataType.NText:
					return true;
				case SqlDataType.NVarChar:
					return dt1.MaximumLength == dt2.MaximumLength;
				case SqlDataType.NVarCharMax:
					return true;
				case SqlDataType.None:
					return true;
				case SqlDataType.Numeric:
					return dt1.NumericPrecision == dt2.NumericPrecision && dt1.NumericScale == dt2.NumericScale;
				case SqlDataType.Real:
					return true;
				case SqlDataType.SmallDateTime:
					return true;
				case SqlDataType.SmallInt:
					return true;
				case SqlDataType.SmallMoney:
					return true;
				case SqlDataType.SysName:
					return true;
				case SqlDataType.Text:
					return true;
				case SqlDataType.Time:
					return true;
				case SqlDataType.Timestamp:
					return true;
				case SqlDataType.TinyInt:
					return true;
				case SqlDataType.UniqueIdentifier:
					return true;
				case SqlDataType.UserDefinedDataType:
					return true;
				case SqlDataType.UserDefinedTableType:
					return true;
				case SqlDataType.UserDefinedType:
					return true;
				case SqlDataType.VarBinary:
					return true;
				case SqlDataType.VarBinaryMax:
					return true;
				case SqlDataType.VarChar:
					return dt1.MaximumLength == dt2.MaximumLength;
				case SqlDataType.VarCharMax:
					return true;
				case SqlDataType.Variant:
					return true;
				case SqlDataType.Xml:
					return true;
			}
			return true;
		}
	}
}
