using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Nephrite.Web;

namespace Nephrite.Meta.Database
{
	public class SqlServerMetadataReader : IDatabaseMetadataReader
	{
		public Schema ReadSchema(string name)
		{
			var returnSchema = new Schema();
			var DbScript = new DBScriptMSSQL(name);
			using (SqlConnection con = new SqlConnection(ConnectionManager.ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("usp_dbschema", con))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@schema", SqlDbType.VarChar).Value = name;
					con.Open();
					using (XmlReader reader = cmd.ExecuteXmlReader())
					{
						while (reader.Read())
						{
							string s = reader.ReadOuterXml();
							XDocument doc = XDocument.Parse(s);
							doc.Descendants("Table").ToList().ForEach(t =>
							{
								var table = new Table();
								table.Name = t.GetAttributeValue("Name");
								table.Owner = t.GetAttributeValue("Owner");
								table.Description = t.GetAttributeValue("Description");
								//table.Identity = !string.IsNullOrEmpty(t.GetAttributeValue("Identity")) && t.GetAttributeValue("Identity") == "1";
								var xColumnsElement = t.Element("Columns");
								if (xColumnsElement != null)
									xColumnsElement.Descendants("Column").ToList().ForEach(c =>
									{
										var column = new Column();
										column.Name = c.GetAttributeValue("Name");
										column.Identity = !string.IsNullOrEmpty(c.GetAttributeValue("Identity")) && c.GetAttributeValue("Identity") == "1";
										column.Nullable = !string.IsNullOrEmpty(c.GetAttributeValue("Nullable")) && c.GetAttributeValue("Nullable") == "1";
										column.Type = DbScript.GetType(c.GetAttributeValue("Type"), !column.Nullable);
										column.ComputedText = c.GetAttributeValue("ComputedText");
										column.Description = c.GetAttributeValue("Description");
										column.DefaultValue = c.GetAttributeValue("DefaultValue");
										column.ForeignKeyName = c.GetAttributeValue("ForeignKeyName");
										column.Description = c.GetAttributeValue("Description");
										column.IsPrimaryKey = !string.IsNullOrEmpty(c.GetAttributeValue("IsPrimaryKey")) && c.GetAttributeValue("IsPrimaryKey") == "1";
										column.Table = table;
										if (column.Identity) table.Identity = true;
										table.Columns.Add(column.Name, column);
									});

								var xForeignKeysElement = t.Element("ForeignKeys");
								if (xForeignKeysElement != null)
									xForeignKeysElement.Descendants("ForeignKey").ToList().ForEach(c =>
									{
										var foreignKey = new ForeignKey();
										foreignKey.Name = c.GetAttributeValue("Name");
										foreignKey.RefTable = c.GetAttributeValue("RefTable");
										foreignKey.IsEnabled = !string.IsNullOrEmpty(c.GetAttributeValue("IsEnabled")) && c.GetAttributeValue("IsEnabled") == "1";
										foreignKey.Columns = c.Descendants("Column").Select(fc => fc.GetAttributeValue("Name")).ToArray();
										foreignKey.RefTableColumns = c.Descendants("RefTableColumn").Select(fc => fc.GetAttributeValue("Name")).ToArray();
										var xDeleteOptionElement = t.Element("DeleteOption");
										foreignKey.DeleteOption = DeleteOption.Restrict;
										if (xDeleteOptionElement != null)
											foreignKey.DeleteOption = (DeleteOption)Int32.Parse(xDeleteOptionElement.Value);

										foreignKey.Table = table;
										table.ForeignKeys.Add(foreignKey.Name, foreignKey);
									});

								var xTriggersElement = t.Element("Triggers");
								if (xTriggersElement != null)
									xTriggersElement.Descendants("Trigger").ToList().ForEach(c =>
									{
										var trigger = new Trigger();
										trigger.Owner = table.Name;
										trigger.Name = c.GetAttributeValue("Name");
										trigger.Text = c.GetAttributeValue("Text");
										table.Triggers.Add(trigger.Name, trigger);
									});

								table.Description = t.GetAttributeValue("Description");


								var xPrimaryKeyElement = t.Element("PrimaryKey");
								if (xPrimaryKeyElement != null)
									table.PrimaryKey = new PrimaryKey()
									{
										Name = xPrimaryKeyElement.GetAttributeValue("Name"),
										Columns =
											xPrimaryKeyElement.Descendants("Column")
															  .Select(pc => pc.GetAttributeValue("Name"))
															  .ToArray(),
										Table = table
									};


								var xIndexesElement = t.Element("Indexes");
								if (xIndexesElement != null)
									xIndexesElement.Descendants("Index").ToList().ForEach(c =>
									{
										var index = new Index();
										index.Name = c.GetAttributeValue("Name");
										index.Columns = c.Descendants("Column").Select(fc => fc.GetAttributeValue("Name")).ToArray();
										index.Table = table;
										index.Cluster = c.GetAttributeValue("Cluster");
										index.AllowPageLocks = !string.IsNullOrEmpty(c.GetAttributeValue("AllowPageLocks")) && c.GetAttributeValue("AllowPageLocks") == "1";
										index.AllowRowLocks = !string.IsNullOrEmpty(c.GetAttributeValue("AllowRowLocks")) && c.GetAttributeValue("AllowRowLocks") == "1";
										index.IgnoreDupKey = !string.IsNullOrEmpty(c.GetAttributeValue("IgnoreDupKey")) && c.GetAttributeValue("IgnoreDupKey") == "1";
										index.IsUnique = !string.IsNullOrEmpty(c.GetAttributeValue("IsUnique")) && c.GetAttributeValue("IsUnique") == "1";

										table.Indexes.Add(index.Name, index);
									});

								table.Schema = returnSchema;
								returnSchema.Tables.Add(table.Name, table);
							});


							doc.Descendants("View").ToList().ForEach(v =>
							{
								var view = new View();
								view.Name = v.GetAttributeValue("Name");
								view.Text = v.GetAttributeValue("Text");
								var xColumnsElement = v.Element("Columns");
								if (xColumnsElement != null)
									xColumnsElement.Descendants("Column").ToList().ForEach(c =>
									{
										var column = new Column();
										column.Name = c.GetAttributeValue("Name");
										
										column.Nullable = !string.IsNullOrEmpty(c.GetAttributeValue("Nullable")) && c.GetAttributeValue("Nullable") == "1";
										column.Type = DbScript.GetType(c.GetAttributeValue("Type"), !column.Nullable);
										view.Columns.Add(column.Name, column);
									});

								var xTriggersElement = v.Element("Triggers");
								if (xTriggersElement != null)
									xTriggersElement.Descendants("Trigger").ToList().ForEach(c =>
									{
										var trigger = new Trigger();
										trigger.Owner = view.Name;
										trigger.Name = c.GetAttributeValue("Name");
										trigger.Text = c.GetAttributeValue("Text");
										view.Triggers.Add(trigger.Name, trigger);
									});

								returnSchema.Views.Add(view.Name, view);

							});

							doc.Descendants("Procedure").ToList().ForEach(p =>
							{
								var procedure = new Procedure();
								procedure.Name = p.GetAttributeValue("Name");
								procedure.Text = p.GetAttributeValue("Text");
								var xParametrsElement = p.Element("Parameters");
								if (xParametrsElement != null)
									xParametrsElement.Descendants("Parameter").ToList().ForEach(c =>
									{
										var Parameter = new Parameter();
										Parameter.Name = c.GetAttributeValue("Name");
										Parameter.Type = DbScript.GetType(c.GetAttributeValue("Type"), true);
										procedure.Parameters.Add(Parameter.Name, Parameter);
									});

								returnSchema.Procedures.Add(procedure.Name, procedure);

							});

							doc.Descendants("Function").ToList().ForEach(p =>
							{
								var function = new Function();
								function.Name = p.GetAttributeValue("Name");
								function.Text = p.GetAttributeValue("Text");
								var xParametrsElement = p.Element("Parameters");
								if (xParametrsElement != null)
									xParametrsElement.Descendants("Parameter").ToList().ForEach(c =>
									{
										var Parameter = new Parameter();
										Parameter.Name = c.GetAttributeValue("Name");
										Parameter.Type = DbScript.GetType(c.GetAttributeValue("Type"), true);
										function.Parameters.Add(Parameter.Name, Parameter);
									});

								if (!returnSchema.Functions.ContainsKey(function.Name))
									returnSchema.Functions.Add(function.Name, function);

							});


						}
					}
				}
			}
			returnSchema.Name = name;
			return returnSchema;
		}

		public List<ProcedureDetails> ReadProceduresDetails()
		{
			var mapType = new DataTypeMapper();
			var listProcedureDetails = new List<ProcedureDetails>();
			using (SqlConnection con = new SqlConnection(ConnectionManager.ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("select * from [dbo].[MM_DBProgrammability]", con))
				{
					cmd.CommandType = CommandType.Text;

					con.Open();
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var procedureDetails = new ProcedureDetails();
							procedureDetails.ProcedureName = reader["Name"].ToString();
							XDocument doc = XDocument.Parse(reader["Returns"].ToString());

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
		}
	}


}