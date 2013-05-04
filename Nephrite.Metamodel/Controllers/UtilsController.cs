using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;
using Microsoft.SqlServer.Management.Common;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.SqlServer.Management.Smo;
using System.Text;
using Nephrite.Metamodel.DbStruct;
using System.Xml;
using Nephrite.Metamodel.Model;

namespace Nephrite.Metamodel.Controllers
{
    [ControllerControlsPath("/_controltemplates/Nephrite.Metamodel/")]
    public class UtilsController : BaseController
    {
		//public void Utils()
		//{
		//    RenderView("utils");
		//}

        public void GenerateDB()
        {
            RenderView("generatedb");
        }

        public void GenerateWsdl()
        {
            RenderView("generatewsdl");
        }

		//public void DbReport()
		//{
		//    RenderView("dbreport");
		//}

		//public void DbReportWord()
		//{
		//    RenderWordDoc("dbreport", AppMM.DBName() + "_dbreport.doc");
		//}

		//public void SpmTable()
		//{
		//	RenderView("spmtable");
		//}

		public void TableDesc()
		{
			RenderView("tabledesc");
		}

		public void Docx2MM()
		{
			RenderView("docx2mm");
		}

		public void RunUp()
		{
			RenderView("runup");
		}

		public DbStruct.Database RetrieveDbStruct(string dbname)
		{
			DbStruct.Database dbStruct = new DbStruct.Database();
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
			ServerConnection sc = b.IntegratedSecurity ? new ServerConnection(b.DataSource) : new ServerConnection(b.DataSource, b.UserID, b.Password);
			sc.Connect();
			Server server = new Server(sc);
			if (dbname.IsEmpty())
				dbname = b.InitialCatalog;
			var db = server.Databases[dbname];
			dbStruct.Name = db.Name;
			dbStruct.Created = DateTime.Now;
			foreach (Microsoft.SqlServer.Management.Smo.Table t in db.Tables)
			{
				if (t.IsSystemObject)
					continue;
				dbStruct.Tables.Add(new DbStruct.Table
				{
					Columns = t.Columns.Cast<Column>().Select(o => new DbStruct.TableColumn
					{
						ComputedText = o.ComputedText,
						DefaultValue = o.DefaultConstraint != null ? o.DefaultConstraint.Text : o.Default,
						DefaultConstraintName = o.DefaultConstraint != null ? o.DefaultConstraint.Name : null,
						Name = o.Name,
						Type = o.DataType.ToString(),
						Nullable = o.Nullable,
						MaximumLength = o.DataType.MaximumLength,
						Precision = o.DataType.NumericPrecision,
						Scale = o.DataType.NumericScale
					}).ToList(),
					Name = t.Name,
					Triggers = t.Triggers.Cast<Microsoft.SqlServer.Management.Smo.Trigger>().Select(o => new DbStruct.Trigger
					{
						Name = o.Name,
						Text = o.TextHeader + Environment.NewLine + o.TextBody
					}).ToList(),
					Indexes = t.Indexes.Cast<Microsoft.SqlServer.Management.Smo.Index>().Select(o => new DbStruct.Index
					{
						Name = o.Name,
						IsUnique = o.IsUnique,
						IsPrimaryKey = o.IndexKeyType == IndexKeyType.DriPrimaryKey,
						Columns = o.IndexedColumns.Cast<IndexedColumn>().Select(ic => new DbStruct.IndexColumn
						{
							Ascending = !ic.Descending,
							Name = ic.Name
						}).ToList(),
						FilterDefinition = o.FilterDefinition
					}).ToList(),
					ForeignKeys = t.ForeignKeys.Cast<Microsoft.SqlServer.Management.Smo.ForeignKey>().Select(o => new DbStruct.ForeignKey
					{
						Name = o.Name,
						DeleteCascade = o.DeleteAction == ForeignKeyAction.Cascade,
						ReferencedTable = o.ReferencedTable,
						Columns = o.Columns.Cast<Microsoft.SqlServer.Management.Smo.ForeignKeyColumn>().Select(fkc => new DbStruct.ForeignKeyColumn
						{
							Name = fkc.Name,
							ReferencedColumn = fkc.ReferencedColumn
						}).ToList()
					}).ToList(),
					Identity = t.Columns.Cast<Column>().Any(o => o.Identity)
				});
			}
			foreach (Microsoft.SqlServer.Management.Smo.View v in db.Views)
			{
				if (v.IsSystemObject)
					continue;
				dbStruct.Views.Add(new DbStruct.View
				{
					Name = v.Name,
					Text = v.TextHeader + Environment.NewLine + v.TextBody,
					Triggers = v.Triggers.Cast<Microsoft.SqlServer.Management.Smo.Trigger>().Select(o => new DbStruct.Trigger
					{
						Name = o.Name,
						Text = o.TextHeader + Environment.NewLine + o.TextBody
					}).ToList()
				});
			}
			foreach (Microsoft.SqlServer.Management.Smo.StoredProcedure sp in db.StoredProcedures)
			{
				if (sp.IsSystemObject)
					continue;
				dbStruct.StoredProcedures.Add(new DbStruct.StoredProcedure
				{
					Name = sp.Name,
					Text = sp.TextHeader + Environment.NewLine + sp.TextBody,
					Parameters = sp.Parameters.Cast<Microsoft.SqlServer.Management.Smo.StoredProcedureParameter>().Select(o => new DbStruct.StoredProcedureParameter
					{
						Name = o.Name,
						DataType = o.DataType.ToString()
					}).ToList()
				});
			}
			foreach (UserDefinedFunction sf in db.UserDefinedFunctions)
			{
				if (sf.IsSystemObject)
					continue;
				dbStruct.StoredFunctions.Add(new DbStruct.StoredFunction
				{
					Name = sf.Name,
					Text = sf.TextHeader + Environment.NewLine + sf.TextBody,
					ReturnDataType = sf.DataType == null ? "table" : sf.DataType.ToString(),
					Parameters = sf.Parameters.Cast<UserDefinedFunctionParameter>().Select(o => new DbStruct.StoredFunctionParameter
					{
						Name = o.Name,
						DataType = o.DataType.ToString()
					}).ToList()
				});
			}
			XmlDocument userData = new XmlDocument();
			userData.AppendChild(userData.CreateElement("UserData"));
			var salist = userData.CreateElement("SPM_ActionList");
			userData.DocumentElement.AppendChild(salist);
			foreach (var sa in AppMM.DataContext.SPM_Actions)
			{
				var xe = userData.CreateElement("SPM_Action");
				xe.SetAttribute("Type", sa.Type.ToString());
				xe.SetAttribute("Title", sa.Title);
				xe.SetAttribute("SystemName", sa.SystemName);
				xe.SetAttribute("ID", sa.ActionID.ToString());
				salist.AppendChild(xe);
			}
			var saalist = userData.CreateElement("SPM_ActionAssoList");
			userData.DocumentElement.AppendChild(saalist);
			foreach (var saa in AppMM.DataContext.SPM_ActionAssos)
			{
				var xe = userData.CreateElement("SPM_ActionAsso");
				xe.SetAttribute("ParentID", saa.ParentActionID.ToString());
				xe.SetAttribute("ID", saa.ActionID.ToString());
				saalist.AppendChild(xe);
			}
			dbStruct.UserData = userData.DocumentElement;

			return dbStruct;
		}

		public string CreateRunUp(DbStruct.Database sourceDb)
		{
			var targetDb = RetrieveDbStruct("");

			StringBuilder runup = new StringBuilder();
			runup.AppendLine("BEGIN TRY");
			runup.AppendLine("BEGIN TRANSACTION");

			// Удалить ненужные констреинты
			foreach (var src in sourceDb.Tables)
			{
				var tgt = targetDb.Tables.Get(src.Name);
				if (tgt != null)
				{
					foreach (var srcfk in src.ForeignKeys)
					{
						var tgtfk = tgt.ForeignKeys.Get(srcfk.Name);
						if (tgtfk == null || tgtfk.ReferencedTable != srcfk.ReferencedTable ||
							tgtfk.Columns.Select(o => o.Name).OrderBy(o => o).Join(",").ToLower() !=
							srcfk.Columns.Select(o => o.Name).OrderBy(o => o).Join(",").ToLower())
						{
							runup.AppendFormat("ALTER TABLE [{0}] DROP CONSTRAINT {1}{2}", src.Name, srcfk.Name, Environment.NewLine);
						}
					}
				}
			}

			// Удалить ненужные вьюхи
			foreach (var src in sourceDb.Views)
			{
				var tgt = targetDb.Views.Get(src.Name);
				if (tgt == null)
				{
					runup.AppendFormat("DROP VIEW [{0}]{1}", src.Name, Environment.NewLine);
				}
			}

			// Удалить ненужные процы
			foreach (var src in sourceDb.StoredProcedures)
			{
				var tgt = targetDb.StoredProcedures.Get(src.Name);
				if (tgt == null)
				{
					runup.AppendFormat("DROP PROCEDURE {0}{1}", src.Name, Environment.NewLine);
				}
			}

			// Удалить ненужные функции
			foreach (var src in sourceDb.StoredFunctions)
			{
				var tgt = targetDb.StoredFunctions.Get(src.Name);
				if (tgt == null)
				{
					runup.AppendFormat("DROP FUNCTION {0}{1}", src.Name, Environment.NewLine);
				}
			}

			// Удалить ненужные таблицы
			foreach (var src in sourceDb.Tables)
			{
				var tgt = targetDb.Tables.Get(src.Name);
				if (tgt == null)
				{
					runup.AppendFormat("DROP TABLE [{0}]{1}", src.Name, Environment.NewLine);
				}
			}

			Func<DbStruct.Table, TableColumn, bool> isIdentity = (t, tc) => t.Indexes.Any(o => o.Columns.Any(o1 => o1.Name == tc.Name) &&
				o.IsPrimaryKey && t.Identity);

			// Обновить таблицы
			foreach (var tgt in targetDb.Tables)
			{
				var src = sourceDb.Tables.Get(tgt.Name);
				if (src == null)
				{
					runup.AppendFormat("CREATE TABLE [{0}]({1}", tgt.Name, Environment.NewLine);
					foreach (var col in tgt.Columns)
						runup.AppendFormat("	[{0}] {1}{2}{3}{4}", col.Name, col.ComputedText.IsEmpty() ?
							(col.FullType + (col.Nullable ? " NULL" : " NOT NULL") + (col.DefaultValue.IsEmpty() ? "" : " DEFAULT " + col.DefaultValue)) :
							("AS " + col.ComputedText), isIdentity(tgt, col) ? " IDENTITY(1,1)" : "", col == tgt.Columns.Last() ? "" : ",", Environment.NewLine);
					runup.AppendLine(")");
					foreach (var pk in tgt.Indexes.Where(o => o.IsPrimaryKey))
					{
						runup.AppendFormat("ALTER TABLE [{0}] ADD CONSTRAINT [{1}] PRIMARY KEY CLUSTERED ({2}){3}", tgt.Name, pk.Name, pk.Columns.Select(o => o.Name).Join(","), Environment.NewLine);
					}
					foreach (var tfk in tgt.ForeignKeys)
					{
						runup.AppendFormat("ALTER TABLE [{0}] ADD CONSTRAINT {1} FOREIGN KEY ([{2}]) REFERENCES [{3}]	([{4}]) ON UPDATE NO ACTION ON DELETE {5}{6}",
							tgt.Name, tfk.Name, tfk.Columns.Select(o => o.Name).Join(","),
							tfk.ReferencedTable, tfk.Columns.Select(o => o.ReferencedColumn).Join(","), tfk.DeleteCascade ? "CASCADE" : "NO ACTION", Environment.NewLine);
					}
				}
				else
				{
					// Удалить индексы
					foreach (var i in src.Indexes)
						if (tgt.Indexes.Get(i.Name) == null)
						{
							runup.AppendFormat("DROP INDEX {0} ON [{1}]{2}", i.Name, tgt.Name, Environment.NewLine);
						}

					// Удалить триггеры
					foreach (var tr in src.Triggers)
						if (tgt.Triggers.Get(tr.Name) == null)
						{
							runup.AppendFormat("DROP TRIGGER {0}{1}", tr.Name, Environment.NewLine);
						}

					// Удалить столбцы
					foreach (var c in src.Columns)
						if (tgt.Columns.Get(c.Name) == null)
						{
							if (!c.DefaultConstraintName.IsEmpty())
							{
								runup.AppendFormat("ALTER TABLE [{0}] DROP CONSTRAINT {1}{2}", src.Name, c.DefaultConstraintName, Environment.NewLine);
							}
							runup.AppendFormat("ALTER TABLE [{0}] DROP COLUMN [{1}]{2}", src.Name, c.Name, Environment.NewLine);
						}

					// Обновить столбцы
					foreach (var tc in tgt.Columns)
					{
						var sc = src.Columns.Get(tc.Name);
						if (sc == null)
						{
							runup.AppendFormat("ALTER TABLE [{0}] ADD {1} {2} {3} {4} {5}{6}", src.Name, tc.Name, tc.ComputedText.IsEmpty() ? tc.FullType : "",
								tc.ComputedText.IsEmpty() ? (tc.Nullable ? "NULL" : "NOT NULL") : "", tc.DefaultValue.IsEmpty() ? "" : ("DEFAULT (" + tc.DefaultValue + ")"),
								tc.ComputedText.IsEmpty() ? "" : "AS (" + tc.ComputedText + ")", Environment.NewLine);
						}
						else if (sc.FullType != tc.FullType || sc.Nullable != tc.Nullable|| sc.ComputedText != tc.ComputedText)
						{
							if (sc.ComputedText != tc.ComputedText)
							{
								runup.AppendFormat("ALTER TABLE [{0}] DROP COLUMN [{1}]{2}", src.Name, sc.Name, Environment.NewLine);
								runup.AppendFormat("ALTER TABLE [{0}] ADD {1} {2}{3}", src.Name, tc.Name,
									tc.ComputedText.IsEmpty() ? "" : "AS (" + tc.ComputedText + ")", Environment.NewLine);
							}
							else
							{
								runup.AppendFormat("ALTER TABLE [{0}] ALTER COLUMN [{1}] {2} {3} {4} {5}{6}", src.Name, tc.Name, tc.FullType,
									tc.ComputedText.IsEmpty() ? (tc.Nullable ? "NULL" : "NOT NULL") : "", tc.DefaultValue.IsEmpty() ? "" : ("DEFAULT (" + tc.DefaultValue + ")"),
									tc.ComputedText.IsEmpty() ? "" : "AS (" + tc.ComputedText + ")", Environment.NewLine);
							}
						}
						else if (sc.DefaultValue != tc.DefaultValue || sc.DefaultConstraintName != tc.DefaultConstraintName)
						{
							if (!sc.DefaultConstraintName.IsEmpty())
							{
								runup.AppendFormat("ALTER TABLE [{0}] DROP CONSTRAINT {1}{2}", src.Name, sc.DefaultConstraintName, Environment.NewLine);
							}
							if (!tc.DefaultConstraintName.IsEmpty())
							{
								runup.AppendFormat("ALTER TABLE [{0}] ADD CONSTRAINT [{1}] DEFAULT {2} FOR [{3}]{4}", src.Name, tc.DefaultConstraintName, tc.DefaultValue, tc.Name, Environment.NewLine);
							}
						}
					}

					// Создать внешние ключи
					foreach (var tfk in tgt.ForeignKeys)
					{
						var sfk = src.ForeignKeys.Get(tfk.Name);
						if (sfk == null || tfk.ReferencedTable != sfk.ReferencedTable ||
							tfk.Columns.Select(o => o.Name).OrderBy(o => o).Join(",").ToLower() !=
							sfk.Columns.Select(o => o.Name).OrderBy(o => o).Join(",").ToLower() ||
							sfk.DeleteCascade != tfk.DeleteCascade)
						{
							if (sfk != null)
								runup.AppendFormat("ALTER TABLE [{0}] DROP CONSTRAINT {1}{2}", src.Name, sfk.Name, Environment.NewLine);
							runup.AppendFormat("ALTER TABLE [{0}] ADD CONSTRAINT {1} FOREIGN KEY ([{2}]) REFERENCES [{3}]	([{4}]) ON UPDATE NO ACTION ON DELETE {5}{6}",
								tgt.Name, tfk.Name, tfk.Columns.Select(o => o.Name).Join(","),
								tfk.ReferencedTable, tfk.Columns.Select(o => o.ReferencedColumn).Join(","), tfk.DeleteCascade ? "CASCADE" : "NO ACTION", Environment.NewLine);
						}
					}
					if (src.Identity != tgt.Identity)
					{
						runup.AppendFormat("{0}: SET IDENTITY {1}{2}", tgt.Name, tgt.Identity ? "ON" : "OFF", Environment.NewLine);
					}
				}
			}
			runup.AppendLine("COMMIT TRANSACTION");
			runup.AppendLine("print 'Database structure successfully updated!'");
			runup.AppendLine("END TRY");
			runup.AppendLine("BEGIN CATCH");
			runup.AppendLine("ROLLBACK TRANSACTION");
			runup.AppendLine("print 'Error at line: ' + convert(varchar(50), ERROR_LINE())");
			runup.AppendLine("print ERROR_MESSAGE()");
			runup.AppendLine("GOTO RunupEnd");
			runup.AppendLine("END CATCH");

			// В целевой БД
			if (sourceDb.UserData != null)
			{
				List<SPM_Action> sourceActions = sourceDb.UserData.SelectNodes("//SPM_Action").
					OfType<XmlElement>().Select(o => new SPM_Action
					{
						ActionID = o.GetAttribute("ID").ToInt32(0),
						Type = o.GetAttribute("Type").ToInt32(0),
						Title = o.GetAttribute("Title"),
						SystemName = o.GetAttribute("SystemName")
					}).ToList();
				List<SPM_ActionAsso> sourceActionAsso = sourceDb.UserData.SelectNodes("//SPM_ActionAsso").
					OfType<XmlElement>().Select(o => new SPM_ActionAsso
					{
						ActionID = o.GetAttribute("ID").ToInt32(0),
						ParentActionID = o.GetAttribute("ParentID").ToInt32(0)
					}).ToList();
				// Создать защищаемые объекты уровня 0
				foreach (var action in AppMM.DataContext.SPM_Actions.Where(o => o.Type == 0))
				{
					// если нет
					var sourceAction0 = sourceActions.FirstOrDefault(o => o.Type == 0 && o.SystemName == action.SystemName);
					if (sourceAction0 == null)
					{
						runup.AppendLine("INSERT INTO [SPM_Action]([Title],[Type],[SystemName],[ActionTypeID])");
						runup.AppendFormat("VALUES('{0}',0,'{1}',1)\r\n", action.Title, action.SystemName);
						runup.AppendFormat("declare @action{0} int\r\nset @action{0} = @@IDENTITY", action.ActionID);
						// Создать уровни 1
						foreach (var asso1 in AppMM.DataContext.SPM_ActionAssos.Where(o => o.ParentActionID == action.ActionID).ToList())
						{
							runup.AppendLine("INSERT INTO [SPM_Action]([Title],[Type],[SystemName],[ActionTypeID])");
							runup.AppendFormat("VALUES('{0}',0,'{1}',1)\r\n", asso1.SPM_Action.Title, asso1.SPM_Action.SystemName);
							runup.AppendFormat("declare @action{0} int\r\nset @action{0} = @@IDENTITY", asso1.ActionID);

							runup.AppendLine("INSERT INTO [SPM_ActionAsso]([ActionID],[ParentActionID])");
							runup.AppendFormat("VALUES(@action{0},@action{1})\r\n", asso1.ActionID, action.ActionID);

							// Создать уровни 2
							foreach (var asso2 in AppMM.DataContext.SPM_ActionAssos.Where(o => o.ParentActionID == asso1.ActionID).ToList())
							{
								runup.AppendLine("INSERT INTO [SPM_Action]([Title],[Type],[SystemName],[ActionTypeID])");
								runup.AppendFormat("VALUES('{0}',0,'{1}',1)\r\n", asso2.SPM_Action.Title, asso2.SPM_Action.SystemName);

								runup.AppendLine("INSERT INTO [SPM_ActionAsso]([ActionID],[ParentActionID])");
								runup.AppendFormat("VALUES(@@IDENTITY,@action{0})\r\n", asso1.ActionID);
							}
						}
					}
					else
					{
						if (sourceAction0.Title != action.Title)
						{
							runup.AppendFormat("UPDATE SPM_Action SET Title = '{0}' WHERE ActionID = {1}\r\n", action.Title, sourceAction0.ActionID);
						}
						// ЗО есть, проверить наличие дочерних
						foreach (var asso1 in AppMM.DataContext.SPM_ActionAssos.Where(o => o.ParentActionID == action.ActionID).ToList())
						{
							var sourceAction1 = (from saa in sourceActionAsso.Where(o => o.ParentActionID == sourceAction0.ActionID)
												 join sa in sourceActions on saa.ActionID equals sa.ActionID
												 where sa.SystemName == asso1.SPM_Action.SystemName
												 select sa).FirstOrDefault();
							if (sourceAction1 == null)
							{
								// Создать уровень 1
								runup.AppendLine("INSERT INTO [SPM_Action]([Title],[Type],[SystemName],[ActionTypeID])");
								runup.AppendFormat("VALUES('{0}',0,'{1}',1)\r\n", asso1.SPM_Action.Title, asso1.SPM_Action.SystemName);
								runup.AppendFormat("declare @action{0} int\r\nset @action{0} = @@IDENTITY\r\n", asso1.ActionID);

								runup.AppendLine("INSERT INTO [SPM_ActionAsso]([ActionID],[ParentActionID])");
								runup.AppendFormat("VALUES(@action{0},{1})\r\n", asso1.ActionID, sourceAction0.ActionID);

								// Создать уровни 2
								foreach (var asso2 in AppMM.DataContext.SPM_ActionAssos.Where(o => o.ParentActionID == asso1.ActionID).ToList())
								{
									runup.AppendLine("INSERT INTO [SPM_Action]([Title],[Type],[SystemName],[ActionTypeID])");
									runup.AppendFormat("VALUES('{0}',0,'{1}',1)\r\n", asso2.SPM_Action.Title, asso2.SPM_Action.SystemName);

									runup.AppendLine("INSERT INTO [SPM_ActionAsso]([ActionID],[ParentActionID])");
									runup.AppendFormat("VALUES(@@IDENTITY,@action{0})\r\n", asso1.ActionID);
								}
							}
							else
							{
								if (sourceAction1.Title != asso1.SPM_Action.Title)
								{
									runup.AppendFormat("UPDATE SPM_Action SET Title = '{0}' WHERE ActionID = {1}\r\n", asso1.SPM_Action.Title, sourceAction1.ActionID);
								}
								// ЗО есть, проверить наличие дочерних
								foreach (var asso2 in AppMM.DataContext.SPM_ActionAssos.Where(o => o.ParentActionID == asso1.ActionID).ToList())
								{
									var sourceAction2 = (from saa in sourceActionAsso.Where(o => o.ParentActionID == sourceAction1.ActionID)
														 join sa in sourceActions on saa.ActionID equals sa.ActionID
														 where sa.SystemName == asso2.SPM_Action.SystemName
														 select sa).FirstOrDefault();
									if (sourceAction2 == null)
									{
										// Создать уровень 2
										runup.AppendLine("INSERT INTO [SPM_Action]([Title],[Type],[SystemName],[ActionTypeID])");
										runup.AppendFormat("VALUES('{0}',0,'{1}',1)\r\n", asso2.SPM_Action.Title, asso2.SPM_Action.SystemName);
										runup.AppendFormat("declare @action{0} int\r\nset @action{0} = @@IDENTITY\r\n", asso2.ActionID);

										runup.AppendLine("INSERT INTO [SPM_ActionAsso]([ActionID],[ParentActionID])");
										runup.AppendFormat("VALUES(@action{0},{1})\r\n", asso2.ActionID, sourceAction1.ActionID);
									}
									else
									{
										if (sourceAction2.Title != asso2.SPM_Action.Title)
										{
											runup.AppendFormat("UPDATE SPM_Action SET Title = '{0}' WHERE ActionID = {1}\r\n", asso2.SPM_Action.Title, sourceAction2.ActionID);
										}
									}
								}
							}
						}
					}
				}
			}
			runup.AppendLine("RunupEnd:");
			
			return runup.ToString();
		}

		public void GetDbStruct(string dbname)
		{
			var d = RetrieveDbStruct(dbname);

			Response.AppendHeader("Content-Type", "text/xml");
			Response.AppendHeader("Content-disposition", "attachment; filename=DbStruct_" + d.Name + ".xml");
			Response.Write(d.Serialize());
			Response.End();
		}
    }
}
