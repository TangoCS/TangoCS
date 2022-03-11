using System;
using System.Collections.Generic;
using System.Linq;

namespace Tango.Meta.Database
{
	public partial class Schema
	{
		public void Generate(IMetaClass cls)
		{
			if (cls.Persistent == PersistenceType.None) return;

			if (cls.Persistent == PersistenceType.Table)
			{
				var tempListJoinTables = new List<string>();

				var t = new Table {
					Schema = this,
					Name = cls.Name,
					//t.Description = cls.Caption;
					Identity = cls.CompositeKey.Count == 1 && cls.Key is ICanBeIdentity && (cls.Key as ICanBeIdentity).IsIdentity
				};

				foreach (var s in cls.Stereotypes)
					if (s is IOnTableGenerateLogic logic)
						logic.Generate(t);

				var primaryColumn = new Column();

				if (cls.BaseClass != null)
				{
					if (cls.CompositeKey.Count > 1)
						throw new Exception(string.Format("Class {0}. При наследовании не должно быть больше одного первичного ключа", t.Name));

					var column = new Column();
					if (cls.CompositeKey.Count < 1)
						column.Name = (cls.Name.Contains("_") ? cls.Name.Substring(cls.Name.IndexOf('_') + 1, cls.Name.Length - cls.Name.IndexOf('_') - 1) : cls.Name) + (cls.BaseClass.Key.Type as IMetaIdentifierType).ColumnSuffix;
					else
						column.Name = cls.CompositeKey[0].Name;
					column.Type = cls.BaseClass.Key.Type;
					column.Table = t;
					column.Nullable = false;

					var pk = new PrimaryKey {
						Name = "PK_" + cls.Name.Trim(),
						Columns = new string[] { column.Name },
						Table = t
					};
					t.PrimaryKey = pk;

					column.ForeignKeyName = "FK_" + cls.Name + "_" + cls.BaseClass.Name;
					t.ForeignKeys.Add(column.ForeignKeyName, new ForeignKey()
					{
						Table = t,
						Name = column.ForeignKeyName,
						RefTable = cls.BaseClass.Name,
						Columns = new[] { column.Name },
						RefTableColumns = new[] { cls.BaseClass.Key.ColumnName }
					});

					primaryColumn = column;
					t.Columns.Add(column.Name, column);

				}
				else if (cls.CompositeKey.Count > 0)
				{
					var pk = new PrimaryKey {
						Name = "PK_" + cls.Name.Trim(),
						Columns = cls.CompositeKey.Select(o => o.ColumnName).ToArray(),
						Table = t
					};
					t.PrimaryKey = pk;

				}
				if (Tables.ContainsKey(t.Name))
					throw new Exception("Duplicate table name " + t.Name);
				Tables.Add(t.Name, t);

				foreach (var prop in cls.Properties)
				{
					if (prop is IMetaComputedAttribute)
						continue;
					if (prop is IMetaReference && prop.UpperBound == -1)
						continue;
					if (prop is ICanBeMultilingual && (prop as ICanBeMultilingual).IsMultilingual)
						continue;

					var column = new Column {
						IsPrimaryKey = cls.CompositeKey.Any(p => p.Name == prop.Name),
						Nullable = !prop.IsRequired
					};

					if (cls.BaseClass != null && column.IsPrimaryKey)
						continue;

					column.Name = prop.Name;
					column.Table = t;
					column.Type = prop.Type;
					column.DefaultValue = prop.DefaultDBValue;

					if (prop is IMetaPersistentComputedAttribute)
					{
						column.ComputedText = (prop as IMetaPersistentComputedAttribute).Expression;
					}

					//Если у свойства мощность 0..1 или 1..1
					if (prop is IMetaReference && prop.UpperBound == 1)
					{
						var r = prop as IMetaReference;
						if (r.RefClass.Key.Type is IMetaIdentifierType)
						{
							column.Name = prop.ColumnName;
						}
						if (r.RefClass.Key.Type is MetaFileType)
						{
							column.Name = "FileGUID";
						}
						column.ForeignKeyName = "FK_" + cls.Name + "_" + r.RefClass.Name;
					}

					if (prop.Type is MetaFileType)
					{
						column.Name = prop.Name + "GUID";
						column.Type = new MetaGuidType();
						column.ForeignKeyName = "FK_" + cls.Name + "_" + prop.Name;
						t.ForeignKeys.Add(column.ForeignKeyName, new ForeignKey() { Table = t, Name = column.ForeignKeyName, RefTable = "N_File", Columns = new[] { column.Name }, RefTableColumns = new[] { "Guid" } });
					}

					if (column.IsPrimaryKey)
					{
						primaryColumn = column;
					}

					column.Identity = prop is ICanBeIdentity ? (prop as ICanBeIdentity).IsIdentity : false;

					t.Columns.Add(column.Name, column);
				}

				foreach (var f in cls.Properties.Where(o => o is IMetaReference))
				{
					//Если у свойства мощность 0..1 или 1..1 Создаём FK
					var metaRef = f as IMetaReference;
					if (f.UpperBound == 1)
					{
						var fkcolumnname = metaRef.RefClass.BaseClass != null ? metaRef.RefClass.BaseClass.Key.ColumnName : metaRef.RefClass.CompositeKey.Select(o => o.ColumnName).First();
						t.ForeignKeys.Add("FK_" + cls.Name + "_" + f.Name, new ForeignKey() {
							Table = t,
							Name = "FK_" + cls.Name + "_" + f.Name,
							RefTable = metaRef.RefClass.Stereotype<STable>()?.Name ?? metaRef.RefClass.Name,
							Columns = new[] { metaRef.ColumnName },
							RefTableColumns = new[] { fkcolumnname }
						});

						// Если референс на WF_Activity
						var wf = new string[] {"wf_activity", "wf_workflow", "wf_transition" };
						if (metaRef.RefClass.Name.ToLower() == "wf_activity" && !wf.Any(o => o == cls.Name.ToLower()))
						{
							var ttr = new Table();
							ttr.Schema = this;
							ttr.Name = cls.Name + "Transition";

							// Добавляем PK
							var trPk = new Column();
							trPk.Name = ttr.Name + "ID";
							trPk.Type = TypeFactory.Int;
							trPk.IsPrimaryKey = true;
							trPk.Nullable = false;
							trPk.Table = ttr;
							trPk.Identity = true;

							var pk = new PrimaryKey();
							pk.Name = "PK_" + ttr.Name.Trim();
							pk.Columns = new string[] { trPk.Name };
							pk.Table = ttr;
							ttr.Identity = true;
							ttr.PrimaryKey = pk;

							ttr.Columns.Add(trPk.Name, trPk);

							ttr.Columns.Add("CreateDate", new Column() { Table = ttr, Name = "CreateDate", Type = TypeFactory.DateTime });
							ttr.Columns.Add("Comment", new Column() { Table = ttr, Name = "Comment", Nullable = true, Type = TypeFactory.String });
							ttr.Columns.Add("IsCurrent", new Column() { Table = ttr, Name = "IsCurrent", Type = TypeFactory.Boolean });
							ttr.Columns.Add("IsLast", new Column() { Table = ttr, Name = "IsLast", Type = TypeFactory.Boolean });
							ttr.Columns.Add("SeqNo", new Column() { Table = ttr, Name = "SeqNo", Type = TypeFactory.Int });

							var fknm = "FK_" + ttr.Name + "_Parent";
							ttr.Columns.Add("ParentID", new Column() { Table = ttr, Name = "ParentID", ForeignKeyName = fknm, Type = TypeFactory.Int });
							ttr.ForeignKeys.Add(fknm, new ForeignKey() { Table = ttr, Name = fknm, RefTable = t.Name, Columns = new[] { "ParentID" }, RefTableColumns = new[] { metaRef.RefClass.Key.Name } });

							fknm = "FK_" + ttr.Name + "_Subject";
							ttr.Columns.Add("SubjectID", new Column() { Table = ttr, Name = "SubjectID", ForeignKeyName = fknm, Type = TypeFactory.Int });
							ttr.ForeignKeys.Add(fknm, new ForeignKey() { Table = ttr, Name = fknm, RefTable = "SPM_Subject", Columns = new[] { "SubjectID" }, RefTableColumns = new[] { "SubjectID" } });

							fknm = "FK_" + ttr.Name + "_Workflow";
							ttr.Columns.Add("WorkflowID", new Column() { Table = ttr, Name = "WorkflowID", ForeignKeyName = fknm, Type = TypeFactory.Int });
							ttr.ForeignKeys.Add(fknm, new ForeignKey() { Table = ttr, Name = fknm, RefTable = "WF_Workflow", Columns = new[] { "WorkflowID" }, RefTableColumns = new[] { "WorkflowID" } });

							fknm = "FK_" + ttr.Name + "_Activity";
							ttr.Columns.Add("ActivityID", new Column() { Table = ttr, Name = "ActivityID", ForeignKeyName = fknm, Type = TypeFactory.Int });
							ttr.ForeignKeys.Add(fknm, new ForeignKey() { Table = ttr, Name = fknm, RefTable = "WF_Activity", Columns = new[] { "ActivityID" }, RefTableColumns = new[] { "ActivityID" } });

							fknm = "FK_" + ttr.Name + "_Transition";
							ttr.Columns.Add("TransitionID", new Column() { Table = ttr, Name = "TransitionID", ForeignKeyName = fknm, Type = TypeFactory.Int });
							ttr.ForeignKeys.Add(fknm, new ForeignKey() { Table = ttr, Name = fknm, RefTable = "WF_Transition", Columns = new[] { "TransitionID" }, RefTableColumns = new[] { "TransitionID" } });

							// Для случая, если есть наследуемый класс, совпадающий с классом формируемым по референсу на WF_Activity
							if (ttr.Name.ToLower().Contains("transition") && Tables.Any(o => o.Key == ttr.Name))
							{
								var trtable = Tables.First(o => o.Key == ttr.Name).Value;
								foreach(var trclm in trtable.Columns.Values)
								{
									if (!trclm.IsPrimaryKey) ttr.Columns.Add(trclm.Name, trclm);
								}
								Tables.Remove(ttr.Name);
							}
							Tables.Add(ttr.Name, ttr);
						}
					}

					//Если у свойства мощность 0..* или 1..* Кроме случая, когда есть обратное свойство с мощностью 0..1 Создаём третью таблицу с двумя колонками
					if (f.UpperBound == -1 && metaRef.InverseProperty == null)
					{
						if (!tempListJoinTables.Contains(t.Name + f.Name) && !tempListJoinTables.Contains(f.Name + t.Name))
						{
							var tj = new Table();
							tj.Schema = this;
							tj.Name = t.Name + f.Name;
							var columnNameLeft = primaryColumn.Name;
							var columnNameRight = f.ColumnName;
							tj.Columns.Add(columnNameLeft, new Column() { 
								Table = tj, Name = columnNameLeft, IsPrimaryKey = true, 
								ForeignKeyName = "FK_" + tj.Name + "_" + t.Name, 
								Type = primaryColumn.Type 
							});
							tj.Columns.Add(columnNameRight, new Column() { 
								Table = tj, Name = columnNameRight, IsPrimaryKey = true, 
								ForeignKeyName = "FK_" + tj.Name + "_" + f.Name, 
								Type = metaRef.RefClass.Key.Type 
							});

							PrimaryKey joinPk = new PrimaryKey {
								Name = "PK_" + tj.Name,
								Columns = tj.Columns.Select(o => o.Key).ToArray(),
								Table = tj
							};
							tj.PrimaryKey = joinPk;

							tj.ForeignKeys.Add("FK_" + tj.Name + "_" + t.Name, new ForeignKey() { 
								Table = tj, Name = "FK_" + tj.Name + "_" + t.Name, RefTable = t.Name, 
								Columns = new[] { columnNameLeft }, 
								RefTableColumns = new[] { primaryColumn.Name } 
							});
							tj.ForeignKeys.Add("FK_" + tj.Name + "_" + f.Name, new ForeignKey() { 
								Table = tj, Name = "FK_" + tj.Name + "_" + f.Name, RefTable = metaRef.RefClass.Name, 
								Columns = new[] { columnNameRight }, 
								RefTableColumns = new[] { metaRef.RefClass.Key.Name } 
							});

							Tables.Add(tj.Name, tj);
							tempListJoinTables.Add(tj.Name);
						}
					}
				}

				if (cls.IsMultilingual)
				{
					var tdata = new Table();
					tdata.Schema = this;
					tdata.Name = cls.Name + "Data";

					// Добавляем PK
					var columnPk = new Column();
					columnPk.Name = tdata.Name + (primaryColumn.Type is MetaGuidType ? "GUID" : "ID");
					columnPk.Type = primaryColumn.Type;
					columnPk.IsPrimaryKey = true;
					columnPk.Nullable = false;
					columnPk.Table = tdata;
					if (primaryColumn.Type is MetaIntType) 
					{
						columnPk.Identity = true;
						tdata.Identity = true;
					}

					PrimaryKey pk = new PrimaryKey();
					pk.Name = "PK_" + tdata.Name.Trim();
					pk.Columns = new string[] { columnPk.Name }; 
					pk.Table = tdata;
					tdata.PrimaryKey = pk;

					tdata.Columns.Add(columnPk.Name, columnPk);

					// Ссылка на базовую таблицу
					primaryColumn.ForeignKeyName = "FK_" + tdata.Name + "_" + t.Name;
					var fk = new Column();
					fk.Name = primaryColumn.Name;
					fk.ForeignKeyName = primaryColumn.ForeignKeyName; 
					fk.Type = primaryColumn.Type;
					fk.IsPrimaryKey = false;
					fk.Nullable = false;
					fk.Table = tdata;

					tdata.Columns.Add(fk.Name, fk);

					tdata.ForeignKeys.Add(fk.ForeignKeyName, new ForeignKey() { Table = tdata, Name = fk.ForeignKeyName, RefTable = cls.Name, Columns = new[] { fk.Name }, RefTableColumns = new[] { fk.Name } });

					var languageColumn = new Column(); // Ссылка на таблицу C_Language
					languageColumn.Name = "LanguageCode";
					languageColumn.Type = new MetaStringType() { Length = 2 };
					languageColumn.ForeignKeyName = "FK_" + tdata.Name + "_C_Language";
					languageColumn.Nullable = false;
					languageColumn.Table = tdata;
					tdata.Columns.Add(languageColumn.Name, languageColumn);

					tdata.ForeignKeys.Add(languageColumn.ForeignKeyName, new ForeignKey() { Table = tdata, Name = languageColumn.ForeignKeyName, RefTable = "C_Language", Columns = new[] { "LanguageCode" }, RefTableColumns = new[] { "LanguageCode" } });

					foreach (var prop in cls.Properties.Where(o => o is ICanBeMultilingual && (o as ICanBeMultilingual).IsMultilingual))
					{
						var column = new Column();
						column.Name = prop.ColumnName;
						column.Table = tdata;
						column.DefaultValue = prop.DefaultDBValue;
						if (prop is IMetaPersistentComputedAttribute)
						{
							column.ComputedText = (prop as IMetaPersistentComputedAttribute).Expression;
						}

						column.Type = prop.Type;
						if (prop.Type is MetaFileType)
						{
							column.Name = prop.Name + "GUID";
							column.Type = new MetaGuidType();
						}
						column.Nullable = !prop.IsRequired;
						tdata.Columns.Add(column.Name, column);
					}

					Tables.Add(tdata.Name, tdata);

					// Создаём вьюшку
					var view = new View();
					foreach(var cl in t.Columns.Values)	
					{
						var vcl = new ViewColumn();
						vcl.Name = cl.Name;
						vcl.Type = cl.Type;
						vcl.Description = cl.Description;
						vcl.Nullable = cl.Nullable;
						view.Columns.Add(vcl.Name, vcl);
					}
					foreach (var cl in tdata.Columns.Values)
						if (cl.Name != columnPk.Name && cl.Name != fk.Name) 
						{
							var vcl = new ViewColumn();
							vcl.Name = cl.Name;
							vcl.Type = cl.Type;
							vcl.Description = cl.Description;
							vcl.Nullable = cl.Nullable;
							view.Columns.Add(vcl.Name, vcl);
						}
					view.Name = "V_" + cls.Name;
					var fcol = t.Columns.Values.Select(o => o.Name).Join(",f.");
					var scol = tdata.Columns.Values.Where(o => o.Name != columnPk.Name && o.Name != fk.Name).Select(o => o.Name).Join(",s.");
					view.Text = string.Format(@"CREATE view dbo.{0} as select f.{4}, s.{5}
						from dbo.{1} f JOIN dbo.{2} s ON f.{3} = s.{3};", view.Name, t.Name, tdata.Name, primaryColumn.Name, fcol, scol);
					Views.Add(view.Name, view);
				}
			}
			if (cls.Persistent == PersistenceType.View)
			{
				var view = new View();
				view.Name = cls.Name;
				view.Text = "";

				if (cls.CompositeKey != null && cls.CompositeKey.Count() != 0)
				{
					var column = new ViewColumn();
					column.Name = cls.Key.ColumnName;
					column.Type = cls.Key.Type;
					column.Nullable = !cls.Key.IsRequired;
					
					view.Columns.Add(column.Name, column);
				}
				foreach (var prop in cls.Properties.Where(t => !cls.CompositeKey.Any(o => o == t)))
				{
					var column = new ViewColumn();
					column.Name = prop.ColumnName;
					column.Type = prop.Type;
					column.Nullable = !prop.IsRequired;

					view.Columns.Add(column.Name, column);
				}
				Views.Add(view.Name, view);
			}
			if (cls.Persistent == PersistenceType.Procedure)
			{
				var procedure = new Procedure();
				procedure.Name = cls.Name;
				//procedure.Text = "";
				foreach(var param in cls.Parameters)
				{
					var par = new Parameter();
					par.Name = param.Name;
					par.Type = param.Type;
					procedure.Parameters.Add(par.Name, par);
				}
				Procedures.Add(procedure.Name, procedure);
			}
			if (cls.Persistent == PersistenceType.TableFunction)
			{
				var function = new TableFunction();
				var stf = cls.Stereotype<STableFunction>();
				function.Name = stf?.Name ?? cls.Name;
				function.ReturnType = stf?.ReturnType.Name ?? cls.Name;
				//function.Text = "";
				foreach (var param in cls.Parameters)
				{
					var par = new Parameter();
					par.Name = param.Name;
					par.Type = param.Type;
					function.Parameters.Add(par.Name, par);
				}
				foreach (var prop in cls.Properties)
				{
					var column = new ViewColumn();
					column.Name = prop.ColumnName;
					column.Type = prop.Type;
					column.Nullable = !prop.IsRequired;
					//column.Description = prop.Description;

					function.Columns.Add(column.Name, column);
				}
				TableFunctions.Add(function.Name, function);
			}
		}
	}
}