using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nephrite.Web;

namespace Nephrite.Meta.Database
{
	class DBScriptPostgreSQL : IDBScript
	{
		public void Comment(string comment)
		{
			throw new NotImplementedException();
		}

		public void CreateTable(Table srcTable)
		{
			throw new NotImplementedException();
		}

		public void DeleteTable(Table currentTable)
		{
			throw new NotImplementedException();
		}

		public void CreateForeignKey(ForeignKey srcforeignKey)
		{
			throw new NotImplementedException();
		}

		public void DeleteForeignKey(ForeignKey currentForeignKey)
		{
			throw new NotImplementedException();
		}

		public void DeletePrimaryKey(PrimaryKey currentPrimaryKey)
		{
			throw new NotImplementedException();
		}

		public void CreatePrimaryKey(PrimaryKey srcPrimaryKey)
		{
			throw new NotImplementedException();
		}

		public void DeleteColumn(Column currentColumn)
		{
			throw new NotImplementedException();
		}

		public void AddComputedColumn(Column srcColumn)
		{
			throw new NotImplementedException();
		}

		public void ChangeColumn(Column srcColumn)
		{
			throw new NotImplementedException();
		}

		public void DeleteTrigger(Trigger currentTrigger)
		{
			throw new NotImplementedException();
		}

		public void CreateTrigger(Trigger srcTrigger)
		{
			throw new NotImplementedException();
		}

		public void SyncIdentity(Table srcTable)
		{
			throw new NotImplementedException();
		}

		public void AddColumn(Column srcColumn)
		{
			throw new NotImplementedException();
		}

		public void DeleteView(View currentView)
		{
			throw new NotImplementedException();
		}

		public void CreateView(View srcView)
		{
			throw new NotImplementedException();
		}

		public System.Xml.Linq.XElement GetMeta()
		{
			throw new NotImplementedException();
		}

		public void DeleteProcedure(Procedure currentProcedure)
		{
			throw new NotImplementedException();
		}

		public void CreateProcedure(Procedure srcProcedure)
		{
			throw new NotImplementedException();
		}

		public void DeleteFunction(Function currentFunction)
		{
			throw new NotImplementedException();
		}

		public void CreateFunction(Function srcFunction)
		{
			throw new NotImplementedException();
		}

		public void DeleteDefaultValue(Column currentColumn)
		{
			throw new NotImplementedException();
		}

		public void AddDefaultValue(Column srcColumn)
		{
			throw new NotImplementedException();
		}

		public void DeleteIndex(Index currentIndex)
		{
			throw new NotImplementedException();
		}

		public void SyncIdentityColumn(Column srcColumn)
		{
			throw new NotImplementedException();
		}

		public string GetIntType()
		{
			return "integer";
		}

		public string GetGuidType()
		{
			return "uuid";
		}

		public string GetStringType(int length)
		{
			return string.Format("varchar[{0}]", length);
		}

		public string GetDecimalType(int precision, int scale)
		{
			return string.Format("numeric[{0},{1}]", precision, scale);
		}

		public string GetDateTimeType()
		{
			return "timestamp";
		}

		public string GetDateType()
		{
			return "date";
		}

		public string GetZoneDateTimeType()
		{
			return "timestamptz";
		}

		public string GetLongType()
		{
			return "bigint";
		}

		public string GetByteArrayType(int length)
		{
			return "bytea";
		}

		public string GetBooleanType()
		{
			return "boolean";
		}

		public string GetXmlType()
		{
			return "xml";
		}

		public MetaPrimitiveType GetType(string dataType, bool notNull)
		{
			var type = dataType.Contains("(") ? dataType.Substring(0, dataType.IndexOf("(", System.StringComparison.Ordinal)) : dataType;
			int precision = -1;
			int scale = -1;
			var match = Regex.Match(dataType, @"\((.*?)\)");

			if (match.Groups.Count > 1)
			{
				var value = match.Groups[1].Value;
				string[] arrayVal = value.Split(',');
				precision = arrayVal[0].ToInt32(-1);
				if (arrayVal.Length > 1)
				{
					scale = arrayVal[1].ToInt32(-1);
				}
			}

			switch (type.ToUpper())
			{
				case "INTEGER":
					return notNull ? MetaIntType.NotNull() : MetaIntType.Null();
				case "VARCHAR":
					if (precision == 36)
						return notNull ? MetaGuidType.NotNull() : MetaGuidType.Null();
					else if (precision == -1)
						return notNull ? MetaStringType.NotNull() : MetaStringType.Null();
					else
						return new MetaStringType() { Length = precision, NotNullable = notNull };
				case "NUMERIC":
					return new MetaDecimalType() { Precision = precision, Scale = scale, NotNullable = notNull };
				case "TIMESTAMP":
					return notNull ? MetaDateTimeType.NotNull() : MetaDateTimeType.Null();
				case "DATE":
					return notNull ? MetaDateType.NotNull() : MetaDateType.Null();
				case "BIGINT":
					return notNull ? MetaLongType.NotNull() : MetaLongType.Null();
				case "BYTEA":
					return notNull ? MetaByteArrayType.NotNull() : MetaByteArrayType.Null();
				case "BOOLEAN":
					return notNull ? MetaBooleanType.NotNull() : MetaBooleanType.Null();
				case "XML":
					return notNull ? MetaXmlType.NotNull() : MetaXmlType.Null();
				default:
					return new MetaStringType();
			}
		}

	}
}
