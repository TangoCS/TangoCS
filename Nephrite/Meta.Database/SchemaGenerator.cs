using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Meta.Database
{
	public partial class Schema
	{
		public void Generate(MetaClass cls)
		{
			if (cls.Persistent == PersistenceType.None) return;

			if (cls.Persistent == PersistenceType.Table)
			{
				var tempListJoinTables = new List<string>();

				Table t = new Table();
				t.Schema = this;
				t.Name = cls.Name;
				t.Description = cls.Caption;
				t.Identity = cls.CompositeKey.Count > 0 ? cls.CompositeKey.Count > 1 ? cls.CompositeKey.Where(c => c is MetaAttribute).Any(a => (a as MetaAttribute).IsIdentity) : cls.Key is MetaAttribute && (cls.Key as MetaAttribute).IsIdentity : false;

				if (t.Identity && cls.CompositeKey.Any(c => c.Type is MetaGuidType))
					throw new Exception(string.Format("Class {0}. Поле не может быть Identity с типом uniqueidentifier", t.Name));

				if (cls.BaseClass != null)
				{
					var column = new Column();
					column.Name = (cls.Name.Contains("_") ? cls.Name.Substring(cls.Name.IndexOf('_') + 1, cls.Name.Length - cls.Name.IndexOf('_') - 1) : cls.Name) + (cls.BaseClass.Key.Type as IMetaIdentifierType).ColumnSuffix;
					column.Type = cls.BaseClass.Key.Type;
					column.Table = t;
					column.Nullable = false;

					PrimaryKey pk = new PrimaryKey();
					pk.Name = "PK_" + cls.Name.Trim();
					pk.Columns = new string[] { column.Name };
					pk.Table = t;
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

					t.Columns.Add(column.Name, column);

				}
				else if (cls.CompositeKey.Count > 0)
				{
					PrimaryKey pk = new PrimaryKey();
					pk.Name = "PK_" + cls.Name.Trim();
					pk.Columns = cls.CompositeKey.Select(o => o.ColumnName).ToArray();
					pk.Table = t;
					t.PrimaryKey = pk;

				}
				Tables.Add(t.Name, t);

				var primaryColumn = new Column();

				foreach (var prop in cls.Properties)
				{
					if (prop is MetaComputedAttribute)
						continue;
					if (prop is MetaReference && prop.UpperBound == -1)
						continue;
					if ((prop is MetaAttribute && (prop as MetaAttribute).IsMultilingual) || 
						(prop is MetaPersistentComputedAttribute && (prop as MetaPersistentComputedAttribute).IsMultilingual))
						continue;

					var column = new Column();
					column.Name = prop.Name;
					column.Table = t;
					column.Type = prop.Type;
					column.DefaultValue = prop.DefaultDBValue;
					if (prop is MetaPersistentComputedAttribute)
					{
						column.ComputedText = (prop as MetaPersistentComputedAttribute).Expression;
					}

					//Если у свойства мощность 0..1 или 1..1
					if (prop is MetaReference && prop.UpperBound == 1)
					{
						if ((prop as MetaReference).RefClass.Key.Type is MetaIntType || (prop as MetaReference).RefClass.Key.Type is MetaGuidType)
						{
							column.Name = (prop as MetaReference).RefClass.ColumnName(prop.Name);
						}
						if ((prop as MetaReference).RefClass.Key.Type is MetaFileType)
						{
							column.Name = "FileGUID";
						}
						column.ForeignKeyName = "FK_" + cls.Name + "_" + (prop as MetaReference).RefClassName;
					}

					if (prop.Type is MetaFileType)
					{
						column.Name = prop.Name + "GUID";
						column.Type = new MetaGuidType();
						column.ForeignKeyName = "FK_" + cls.Name + "_" + prop.Name;
						t.ForeignKeys.Add(column.ForeignKeyName, new ForeignKey() { Table = t, Name = column.ForeignKeyName, RefTable = "N_File", Columns = new[] { column.Name }, RefTableColumns = new[] { "Guid" } });
					}

					column.Nullable = !prop.IsRequired;
					column.IsPrimaryKey = cls.CompositeKey.Any(p => p.Name == prop.Name);

					if (column.IsPrimaryKey)
					{
						primaryColumn = column;
					}

					t.Columns.Add(column.Name, column);
				}

				foreach (var f in cls.Properties.Where(o => o is MetaReference))
				{
					//Если у свойства мощность 0..1 или 1..1 Создаём FK
					if (f.UpperBound == 1)
					{
						var metaRef = f as MetaReference;
						var fkcolumnname = metaRef.RefClass.CompositeKey.Select(o => o.ColumnName).First();
						t.ForeignKeys.Add("FK_" + cls.Name + "_" + f.Name, new ForeignKey() { Table = t, Name = "FK_" + cls.Name + "_" + f.Name, RefTable = metaRef.RefClassName, Columns = new[] { metaRef.RefClass.ColumnName(metaRef.Name) }, RefTableColumns = new[] { fkcolumnname } });

						// Если референс на WF_Activity
						var wf = new string[] {"wf_activity", "wf_workflow", "wf_transition" };
						if (metaRef.RefClassName.ToLower() == "wf_activity" && !wf.Any(o => o == cls.Name.ToLower()))
						{
							var ttr = new Table();
							ttr.Schema = this;
							ttr.Name = cls.Name + "Transition";

							// Добавляем PK
							var trPk = new Column();
							trPk.Name = ttr.Name + "ID";
							trPk.Type = MetaIntType.NotNull();
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

							ttr.Columns.Add("CreateDate", new Column() { Table = ttr, Name = "CreateDate", Type = MetaDateTimeType.NotNull() });
							ttr.Columns.Add("Comment", new Column() { Table = ttr, Name = "Comment", Nullable = true, Type = MetaStringType.Null() });
							ttr.Columns.Add("IsCurrent", new Column() { Table = ttr, Name = "IsCurrent", Type = MetaBooleanType.NotNull() });
							ttr.Columns.Add("IsLast", new Column() { Table = ttr, Name = "IsLast", Type = MetaBooleanType.NotNull() });
							ttr.Columns.Add("SeqNo", new Column() { Table = ttr, Name = "SeqNo", Type = MetaIntType.NotNull() });

							var fknm = "FK_" + ttr.Name + "_Parent";
							ttr.Columns.Add("ParentID", new Column() { Table = ttr, Name = "ParentID", ForeignKeyName = fknm, Type = MetaIntType.NotNull() });
							ttr.ForeignKeys.Add(fknm, new ForeignKey() { Table = ttr, Name = fknm, RefTable = t.Name, Columns = new[] { "ParentID" }, RefTableColumns = new[] { metaRef.RefClass.Key.Name } });

							fknm = "FK_" + ttr.Name + "_Subject";
							ttr.Columns.Add("SubjectID", new Column() { Table = ttr, Name = "SubjectID", ForeignKeyName = fknm, Type = MetaIntType.NotNull() });
							ttr.ForeignKeys.Add(fknm, new ForeignKey() { Table = ttr, Name = fknm, RefTable = "SPM_Subject", Columns = new[] { "SubjectID" }, RefTableColumns = new[] { "SubjectID" } });

							fknm = "FK_" + ttr.Name + "_Workflow";
							ttr.Columns.Add("WorkflowID", new Column() { Table = ttr, Name = "WorkflowID", ForeignKeyName = fknm, Type = MetaIntType.NotNull() });
							ttr.ForeignKeys.Add(fknm, new ForeignKey() { Table = ttr, Name = fknm, RefTable = "WF_Workflow", Columns = new[] { "WorkflowID" }, RefTableColumns = new[] { "WorkflowID" } });

							fknm = "FK_" + ttr.Name + "_Activity";
							ttr.Columns.Add("ActivityID", new Column() { Table = ttr, Name = "ActivityID", ForeignKeyName = fknm, Type = MetaIntType.NotNull() });
							ttr.ForeignKeys.Add(fknm, new ForeignKey() { Table = ttr, Name = fknm, RefTable = "WF_Activity", Columns = new[] { "ActivityID" }, RefTableColumns = new[] { "ActivityID" } });

							fknm = "FK_" + ttr.Name + "_Transition";
							ttr.Columns.Add("TransitionID", new Column() { Table = ttr, Name = "TransitionID", ForeignKeyName = fknm, Type = MetaIntType.NotNull() });
							ttr.ForeignKeys.Add(fknm, new ForeignKey() { Table = ttr, Name = fknm, RefTable = "WF_Transition", Columns = new[] { "TransitionID" }, RefTableColumns = new[] { "TransitionID" } });
							
							Tables.Add(ttr.Name, ttr);
						}
					}

					//Если у свойства мощность 0..* или 1..* Кроме случая, когда есть обратное свойство с мощностью 0..1 Создаём третью таблицу с двумя колонками
					if (f.UpperBound == -1 && (f as MetaReference).InverseProperty == null)
					{
						if (!tempListJoinTables.Contains(t.Name + f.Name) && !tempListJoinTables.Contains(f.Name + t.Name))
						{
							var tj = new Table();
							tj.Schema = this;
							tj.Name = t.Name + f.Name;
							var columnNameLeft = primaryColumn.Name;
							var columnNameRight = f.ColumnName;
							tj.Columns.Add(columnNameLeft, new Column() { Table = tj, Name = columnNameLeft, IsPrimaryKey = true, ForeignKeyName = "FK_" + tj.Name + "_" + t.Name, Type = primaryColumn.Type });
							tj.Columns.Add(columnNameRight, new Column() { Table = tj, Name = columnNameRight, IsPrimaryKey = true, ForeignKeyName = "FK_" + tj.Name + "_" + f.Name, Type = (f as MetaReference).RefClass.Key.Type });

							PrimaryKey joinPk = new PrimaryKey();
							joinPk.Name = "PK_" + tj.Name;
							joinPk.Columns = tj.Columns.Select(o => o.Key).ToArray();
							joinPk.Table = tj;
							tj.PrimaryKey = joinPk;

							tj.ForeignKeys.Add("FK_" + tj.Name + "_" + t.Name, new ForeignKey() { Table = tj, Name = "FK_" + tj.Name + "_" + t.Name, RefTable = t.Name, Columns = new[] { columnNameLeft }, RefTableColumns = new[] { primaryColumn.Name } });
							tj.ForeignKeys.Add("FK_" + tj.Name + "_" + f.Name, new ForeignKey() { Table = tj, Name = "FK_" + tj.Name + "_" + f.Name, RefTable = (f as MetaReference).RefClass.Name, Columns = new[] { columnNameRight }, RefTableColumns = new[] { (f as MetaReference).RefClass.Key.Name } });

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

					foreach (var prop in cls.Properties.Where(o => (o is MetaAttribute && (o as MetaAttribute).IsMultilingual) ||
								(o is MetaPersistentComputedAttribute && (o as MetaPersistentComputedAttribute).IsMultilingual)))
					{
						var column = new Column();
						column.Name = prop.ColumnName;
						column.Table = tdata;
						column.DefaultValue = prop.DefaultDBValue;
						if (prop is MetaPersistentComputedAttribute)
						{
							column.ComputedText = (prop as MetaPersistentComputedAttribute).Expression;
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
					view.Text = string.Format(@"CREATE view {0} as select f.*, s.*
						from {1} f JOIN {2} s ON f.{3} = s.{3}", view.Name, t.Name, tdata.Name, primaryColumn.Name);
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
			if (cls.Persistent == PersistenceType.TableFunction)
			{
				var function = new TableFunction();
				function.Name = cls.Name;
				function.ReturnType = cls.Name;
				//function.Text = "";
				foreach(var param in cls.Parameters)
				{
					var par = new Parameter();
					par.Name = param.Name;
					par.Type = param.Type;
					function.Parameters.Add(par.Name, par);
				}
				foreach(var prop in cls.Properties)
				{
					var column = new ViewColumn();
					column.Name = prop.ColumnName;
					column.Type = prop.Type;
					column.Nullable = !prop.IsRequired;
					column.Description = prop.Description;

					function.Columns.Add(column.Name, column);
				}
				TableFunctions.Add(function.Name, function);
			}
		}
	}
}