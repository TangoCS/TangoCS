﻿using System;
using System.Linq;
using System.Xml.Linq;

namespace Tango.Meta.Database
{
	public class PostgreSQLMetadataReader : IDatabaseMetadataReader
	{
		string _connectionString = "";
		public PostgreSQLMetadataReader(string connectionString)
		{
			_connectionString = connectionString;
		}

		public Schema ReadSchema(string name)
		{
			var returnSchema = new Schema();
			returnSchema.Name = name;

			
			var dbScript = new DBScriptPostgreSQL(name);
			var doc = dbScript.GetSchemaFromDatabase(_connectionString);
			GenerateSchema(dbScript, returnSchema, doc);
			return returnSchema;
		}

		public Schema ReadSchema(string name, XDocument doc)
		{
			var returnSchema = new Schema();
			returnSchema.Name = name;
			var dbScript = new DBScriptPostgreSQL(name);

			GenerateSchema(dbScript, returnSchema, doc);

			return returnSchema;
		}

		static void GenerateSchema(DBScriptPostgreSQL dbScript, Schema returnSchema, XDocument doc)
		{	
			doc.Descendants("Table").ToList().ForEach(t =>
			{
				var tabArray = !string.IsNullOrEmpty(t.GetAttributeValue("DESCRIPTION")) ? t.GetAttributeValue("DESCRIPTION").Split('|') : new string[] { };
				var tableName = tabArray.Length > 1 ? tabArray[1] : "";
				var tableDescription = tabArray.Length > 0 ? tabArray[0] : "";

				var table = new Table();
				table.Name = string.IsNullOrEmpty(tableName) ? t.GetAttributeValue("NAME") : tableName;
				table.Owner = t.GetAttributeValue("OWNER");
				table.Description = tableDescription;
				//table.Identity = !string.IsNullOrEmpty(t.GetAttributeValue("IDENTITY")) && t.GetAttributeValue("IDENTITY") == "1";
				var xColumnsElement = t.Element("Columns");
				if (xColumnsElement != null)
					xColumnsElement.Descendants("Column").ToList().ForEach(c =>
					{
						var columnArray = !string.IsNullOrEmpty(c.GetAttributeValue("DESCRIPTION")) ? c.GetAttributeValue("DESCRIPTION").Split('|') : new string[] { };
						var columnName = columnArray.Length > 1 ? columnArray[1] : "";
						var columnDescription = columnArray.Length > 0 ? columnArray[0] : "";

						var column = new Column();
						column.Identity = !string.IsNullOrEmpty(c.GetAttributeValue("IDENTITY")) && c.GetAttributeValue("IDENTITY") == "1";
						column.Name = string.IsNullOrEmpty(columnName) ? c.GetAttributeValue("NAME") : columnName;
						column.Nullable = !string.IsNullOrEmpty(c.GetAttributeValue("NULLABLE")) && c.GetAttributeValue("NULLABLE") == "1";
						column.Type = dbScript.GetType(c.GetAttributeValue("TYPE"), !column.Nullable);
						column.ComputedText = c.GetAttributeValue("COMPUTEDTEXT");
						column.Description = columnDescription;
						column.DefaultValue = c.GetAttributeValue("DEFAULTVALUE");
						column.ForeignKeyName = c.GetAttributeValue("FOREIGNKEYNAME");
						column.IsPrimaryKey = !string.IsNullOrEmpty(c.GetAttributeValue("ISPRIMARYKEY")) && c.GetAttributeValue("ISPRIMARYKEY") == "1";
						column.Table = table;
						if (column.Identity) table.Identity = true;
						table.Columns.Add(column.Name, column);
					});

				var xForeignKeysElement = t.Element("ForeignKeys");
				if (xForeignKeysElement != null)
				{
					xForeignKeysElement.Descendants("ForeignKey").ToList().ForEach(c =>
					{
						var fkColumns = c.Descendants("Column")
										.Select(pc => pc.GetAttributeValue("NAME").ToUpper())
										.ToArray();

						var refkColumns = c.Descendants("RefTableColumn")
									   .Select(pc =>
											pc.GetAttributeValue("REFTABLECOLUMNDESCRIPTION") == null ? pc.GetAttributeValue("NAME") :
											pc.GetAttributeValue("REFTABLECOLUMNDESCRIPTION").Split('|').Length > 1 ? pc.GetAttributeValue("REFTABLECOLUMNDESCRIPTION").Split('|')[1] : pc.GetAttributeValue("REFTABLECOLUMNDESCRIPTION")
										).ToArray();

						var foreignKey = new ForeignKey();
						foreignKey.Name = c.GetAttributeValue("NAME");

						var refTableArray = !string.IsNullOrEmpty(c.GetAttributeValue("REFTABLEDESCRIPTION")) ? c.GetAttributeValue("REFTABLEDESCRIPTION").Split('|') : new string[] { };
						var refTableName = refTableArray.Length > 1 ? refTableArray[1] : "";


						foreignKey.RefTable = string.IsNullOrEmpty(refTableName)
												  ? c.GetAttributeValue("REFTABLE")
												  : refTableName;
						foreignKey.IsEnabled = !string.IsNullOrEmpty(c.GetAttributeValue("ISENABLED")) && c.GetAttributeValue("ISENABLED") == "1";
						foreignKey.Columns = table.Columns.Where(pk => fkColumns.Any(fk => fk == pk.Key.ToUpper()))
								 .Select(cr => cr.Value.Name)
								 .ToArray();

						foreignKey.RefTableColumns = refkColumns;
						var xDeleteOptionElement = c.Element("DeleteOption");
						foreignKey.DeleteOption = DeleteOption.Restrict;
						if (xDeleteOptionElement != null && !string.IsNullOrEmpty(xDeleteOptionElement.Value))
							foreignKey.DeleteOption = (DeleteOption)Int32.Parse(xDeleteOptionElement.Value);

						foreignKey.Table = table;
						table.ForeignKeys.Add(foreignKey.Name, foreignKey);
					});
				}

				var xTriggersElement = t.Element("Triggers");
				if (xTriggersElement != null)
					xTriggersElement.Descendants("Trigger").ToList().ForEach(c =>
					{
						var trigger = new Trigger();
						trigger.Owner = table.Name;
						trigger.Name = c.GetAttributeValue("NAME");
						trigger.Text = c.GetAttributeValue("TEXT");
                        if (!table.Triggers.ContainsKey(trigger.Name))
                            table.Triggers.Add(trigger.Name, trigger);
					});

				table.Description = t.GetAttributeValue("DESCRIPTION");


				var xPrimaryKeyElement = t.Element("PrimaryKey");

				if (xPrimaryKeyElement != null)
				{
					var pkColumns = xPrimaryKeyElement.Descendants("Column")
									  .Select(pc => pc.GetAttributeValue("NAME").ToUpper())
									  .ToArray();
					table.PrimaryKey = new PrimaryKey()
					{
						Name = xPrimaryKeyElement.GetAttributeValue("NAME"),
						Columns = table.Columns.Where(pk => pkColumns.Any(c => c == pk.Key.ToUpper())).Select(cr => cr.Value.Name).ToArray(),
						Table = table
					};
				}


				var xIndexesElement = t.Element("Indexes");
				if (xIndexesElement != null)
					xIndexesElement.Descendants("Index").ToList().ForEach(c =>
					{
						var index = new Index();
						index.Name = c.GetAttributeValue("NAME");
						index.Columns = c.Descendants("Column").Select(fc => fc.GetAttributeValue("NAME")).ToArray();
						index.Table = table;
						index.Cluster = c.GetAttributeValue("CLUSTER");
						index.IsUnique = !string.IsNullOrEmpty(c.GetAttributeValue("ISUNIQUE")) && c.GetAttributeValue("ISUNIQUE") == "1";
						//index.AllowPageLocks = !string.IsNullOrEmpty(c.GetAttributeValue("AllowPageLocks")) && c.GetAttributeValue("AllowPageLocks") == "1";
						//index.AllowRowLocks = !string.IsNullOrEmpty(c.GetAttributeValue("AllowRowLocks")) && c.GetAttributeValue("AllowRowLocks") == "1";
						//index.IgnoreDupKey = !string.IsNullOrEmpty(c.GetAttributeValue("IgnoreDupKey")) && c.GetAttributeValue("IgnoreDupKey") == "1";

						table.Indexes.Add(index.Name, index);
					});

				table.Schema = returnSchema;
				if (!returnSchema.Tables.ContainsKey(table.Name))
					returnSchema.Tables.Add(table.Name, table);
			});


			doc.Descendants("View").ToList().ForEach(v =>
			{
				var view = new View();
				view.Name = v.GetAttributeValue("NAME");
				view.Text = v.GetAttributeValue("TEXT");

				var xColumnsElement = v.Element("Columns");
				if (xColumnsElement != null)
					xColumnsElement.Descendants("Column").ToList().ForEach(c =>
					{
						var column = new ViewColumn();
						column.Name = c.GetAttributeValue("NAME");
						column.Nullable = !string.IsNullOrEmpty(c.GetAttributeValue("NULLABLE")) && c.GetAttributeValue("NULLABLE") == "1";
						column.Type = dbScript.GetType(c.GetAttributeValue("TYPE"), !column.Nullable);
						view.Columns.Add(column.Name, column);
					});

				var xTriggersElement = v.Element("Triggers");
				if (xTriggersElement != null)
					xTriggersElement.Descendants("Trigger").ToList().ForEach(c =>
					{
						var trigger = new Trigger();
						trigger.Owner = view.Name;
						trigger.Name = c.GetAttributeValue("NAME");
						trigger.Text = c.GetAttributeValue("TEXT");
						view.Triggers.Add(trigger.Name, trigger);
					});

				returnSchema.Views.Add(view.Name, view);

			});

			/*doc.Descendants("Procedure").ToList().ForEach(p =>
			{
				var procedure = new Procedure();
				procedure.Name = p.GetAttributeValue("NAME");
				procedure.Text = p.GetAttributeValue("TEXT");
				var xParametrsElement = p.Element("Parameters");
				if (xParametrsElement != null)
					xParametrsElement.Descendants("Parameter").ToList().ForEach(c =>
					{
						var Parameter = new Parameter();
						Parameter.Name = c.GetAttributeValue("NAME");
						Parameter.Type = DbScript.GetType(c.GetAttributeValue("TYPE"), true);
						procedure.Parameters.Add(Parameter.Name, Parameter);
					});

				returnSchema.Procedures.Add(procedure.Name, procedure);

			});*/

			/*doc.Descendants("Function").ToList().ForEach(p =>
			{
				var function = new Function();
				function.Name = p.GetAttributeValue("NAME");
				function.Text = p.GetAttributeValue("TEXT");
				var xParametrsElement = p.Element("Parameters");
				if (xParametrsElement != null)
					xParametrsElement.Descendants("Parameter").ToList().ForEach(c =>
					{
						var Parameter = new Parameter();
						Parameter.Name = c.GetAttributeValue("NAME");
						Parameter.Type = dbScript.GetType(c.GetAttributeValue("TYPE"), true);
						if (!string.IsNullOrEmpty(Parameter.Name) && !function.Parameters.ContainsKey(Parameter.Name))
							function.Parameters.Add(Parameter.Name, Parameter);
					});

				returnSchema.Functions.Add(function.Name, function);

			});*/
		}

		/*public List<ProcedureDetails> ReadProceduresDetails()
		{
			var mapType = new DataTypeMapper();
			var listProcedureDetails = new List<ProcedureDetails>();
			using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
			{
				using (NpgsqlCommand cmd = new NpgsqlCommand("select * from DBO.MM_DBProgrammability", con))
				{
					cmd.CommandType = CommandType.Text;

					con.Open();
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var procedureDetails = new ProcedureDetails();
							procedureDetails.ProcedureName = reader["name"].ToString();
							XDocument doc = XDocument.Parse(reader["returns"].ToString());

							if (doc.Descendants("Column").Any())
							{
								procedureDetails.ReturnType = procedureDetails.ProcedureName + "Result";
								procedureDetails.Columns = new Dictionary<string, string>();
								doc.Descendants("Column").ToList().ForEach(c => procedureDetails.Columns.Add(c.GetAttributeValue("Name"), mapType.MapFromSqlServerDBType(c.GetAttributeValue("Type"), null, null, null).ToString()));
							}
							else if (doc.Descendants("SingleResult").Any())
							{
								procedureDetails.ReturnType = mapType.MapFromSqlServerDBType(doc.Descendants("SingleResult").FirstOrDefault().GetAttributeValue("Type"), null, null, null).ToString();
							}
							else
							{
								procedureDetails.ReturnType = "void";
							}
							listProcedureDetails.Add(procedureDetails);

						}
					}
				}
			}
			return listProcedureDetails;
		}*/
	}
}
