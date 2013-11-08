using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace Nephrite.Meta.Database
{
	public class DBScriptDB2 : IDBScript
	{
		public DBScriptDB2()
		{
			Scripts = new List<string>();
		}

		public List<string> Scripts { get; set; }





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

		public void AddColumn(Column srcColumn)
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

		public void DeleteView(View currentView)
		{
			throw new NotImplementedException();
		}

		public void CreateView(View srcView)
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

		public string GetIntType()
		{
			throw new NotImplementedException();
		}

		public string GetGuidType()
		{
			throw new NotImplementedException();
		}

		public string GetStringType(int length)
		{
			throw new NotImplementedException();
		}

		public string GetDecimalType(int precision, int scale)
		{
			throw new NotImplementedException();
		}

		public string GetDateTimeType()
		{
			throw new NotImplementedException();
		}

		public string GetDateType()
		{
			throw new NotImplementedException();
		}

		public string GetZoneDateTimeType()
		{
			throw new NotImplementedException();
		}

		public string GetLongType()
		{
			throw new NotImplementedException();
		}

		public string GetByteArrayType(int length)
		{
			throw new NotImplementedException();
		}

		public string GetBooleanType()
		{
			throw new NotImplementedException();
		}

		string GetStringValue(SqlDataReader reader, int index)
		{
			if (reader.IsDBNull(index))
				return "null";
			else
			{
				switch (reader.GetDataTypeName(index))
				{
					case "money":
						return reader.GetSqlMoney(index).Value.ToString(CultureInfo.InvariantCulture);
					case "float":
						return reader.GetSqlDouble(index).Value.ToString(CultureInfo.InvariantCulture);
					case "int":
						return reader.GetInt32(index).ToString();
					case "smallint":
						return reader.GetInt16(index).ToString();
					case "tinyint":
						return reader.GetByte(index).ToString();
					case "bigint":
						return reader.GetInt64(index).ToString();
					case "nvarchar":
						return "N'" + reader.GetString(index).Replace("'", "''") + "'";
					case "varchar":
						return "N'" + reader.GetString(index).Replace("'", "''") + "'";
					case "bit":
						return reader.GetBoolean(index) ? "1" : "0";
					case "uniqueidentifier":
						return "N'" + reader.GetGuid(index).ToString() + "'";
					case "char":
						return "N'" + reader.GetString(index).Replace("'", "''").Replace("\0", " ") + "'";
					case "nchar":
						return "N'" + reader.GetString(index).Replace("'", "''").Replace("\0", " ") + "'";
					case "text":
						return "N'" + reader.GetString(index).Replace("'", "''") + "'";
					case "decimal":
						return reader.GetDecimal(index).ToString(CultureInfo.InvariantCulture);
					case "date":
						return String.Format("CAST('{0}' AS Date)", reader.GetDateTime(index).ToString("yyyy-MM-dd"));
					case "datetime":
						return String.Format("CAST('{0}' AS timestamp)", reader.GetSqlDateTime(index).Value.ToString("yyyy-MM-dd HH:mm:ss"), reader.GetSqlDateTime(index).TimeTicks.ToString("X8"));
					case "image":
						StringBuilder result = new StringBuilder();
						byte[] data = reader.GetSqlBytes(index).Value;
						for (int x = 0; x < data.Length; x++)
							result.Append(data[x].ToString("X2"));
						return string.Format("blob(X'{0}')", result.ToString());
					case "xml":
						return String.Format("N'{0}'", reader.GetSqlXml(index).Value.Replace("'", "''"));
					case "varbinary":
						StringBuilder result1 = new StringBuilder();
						byte[] data1 = reader.GetSqlBytes(index).Value;
						for (int x = 0; x < data1.Length; x++)
							result1.Append(data1[x].ToString("X2"));
						return string.Format("blob(X'{0}')", result1.ToString());
					default:
						throw new Exception("unknown data type: " + reader.GetDataTypeName(index));
				}
			}
		}
		public string ImportData(Table t, bool identityInsert, SqlConnection DbConnection)
		{

			if (DbConnection.State == System.Data.ConnectionState.Closed)
				DbConnection.Open();

			var columns = string.Join(", ", t.Columns.Values.Select(c => string.Format("{0}", c.Name)).ToArray());
			SqlCommand cmd = DbConnection.CreateCommand();
			cmd.CommandType = System.Data.CommandType.Text;
			cmd.CommandText = string.Format("select {0} from [{1}] ", columns, t.Name);

			var sqlInsert = string.Empty;
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					StringCollection sc = new StringCollection();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						sc.Add(GetStringValue(reader, i));
					}
					sqlInsert += string.Format("INSERT INTO {0} ({1})  VALUES ({2}); \r\n", t.Name, columns, string.Join(",", sc.Cast<string>().ToArray<string>()));
				}
			}

			if (identityInsert && t.Identity)
			{
				sqlInsert = string.Format("ALTER TABLE {0} ALTER COLUMN {1} SET GENERATED BY DEFAULT;\r\n {2} ALTER TABLE {0} ALTER COLUMN {1} SET GENERATED ALWAYS;", t.Name, t.Columns.Values.FirstOrDefault(c => c.IsPrimaryKey).Name, sqlInsert);
			}
			return sqlInsert;
		}


		public void AddComputedColumn(Column srcColumn)
		{
			throw new NotImplementedException();
		}

	}
}