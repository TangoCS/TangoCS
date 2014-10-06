using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;

namespace Nephrite.Meta.Database
{
	public partial class Schema
	{
		public void Generate(MetaClass cls)
		{
			if (!cls.IsPersistent) return;

			var tempListJoinTables = new List<string>();
			var dbScript = A.DBScript;

			Table t = new Table();
			t.Name = cls.Name;
			t.Description = cls.Caption;
			t.Identity = cls.CompositeKey.Count > 0 ? cls.CompositeKey.Count > 1 ? cls.CompositeKey.Where(c => c is MetaAttribute).Any(a => (a as MetaAttribute).IsIdentity) : cls.Key is MetaAttribute && (cls.Key as MetaAttribute).IsIdentity : false;
			t.Schema = this;
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
				pk.CurrentTable = t;
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
				pk.Columns = cls.CompositeKey.Select(o => o.Name).ToArray();
				pk.CurrentTable = t;
				t.PrimaryKey = pk;

			}
			Tables.Add(t.Name, t);

			var primaryColumn = new Column();

			foreach (var prop in cls.Properties)
			{
				if (prop is MetaComputedAttribute)
					continue;
				if (prop is MetaReference && (prop as MetaReference).UpperBound == -1)
					continue;

				var column = new Column();
				column.Name = prop.Name;
				column.Table = t;
				column.Type = prop.Type;
				if (prop is MetaPersistentComputedAttribute)
				{
					column.ComputedText = (prop as MetaPersistentComputedAttribute).Expression;
				}

				if (prop is MetaReference)
				{
					if (prop.UpperBound == 1) //Если у свойства мощность 0..1 или 1..1
					{
						if ((prop as MetaReference).RefClass.Key.Type is MetaIntType || (prop as MetaReference).RefClass.Key.Type is MetaGuidType)
						{
							column.Name = (prop as MetaReference).RefClass.ColumnName(prop.Name);
						}
						if ((prop as MetaReference).RefClass.Key.Type is MetaFileType)
						{
							column.Name = "FileGUID";
						}
						column.Type = (prop as MetaReference).RefClass.Key.Type;

						column.ForeignKeyName = "FK_" + cls.Name + "_" + (prop as MetaReference).RefClassName;
					}
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
				if (f.UpperBound == 1) //Если у свойства мощность 0..1 или 1..1 Создаём FK
				{
					var metaReference = f as MetaReference;
					var fkcolumnname = metaReference.RefClass.CompositeKey.Select(o => o.Name).First();
					t.ForeignKeys.Add("FK_" + cls.Name + "_" + f.Name, new ForeignKey() { Table = t, Name = "FK_" + cls.Name + "_" + f.Name, RefTable = metaReference.RefClassName, Columns = new[] { metaReference.RefClass.ColumnName(metaReference.Name) }, RefTableColumns = new[] { fkcolumnname } });
				}

				if (f.UpperBound == -1 && (f as MetaReference).RefClass.Key.UpperBound == -1) //Если у свойства мощность 0..* или 1..* Кроме случая, когда есть обратное свойство с мощностью 0..1 Создаём третью таблицу с двумя колонками
				{
					if (!tempListJoinTables.Contains(t.Name + f.Name) && !tempListJoinTables.Contains(f.Name + t.Name))
					{
						var joinTable = new Table();
						joinTable.Name = t.Name + f.Name;
						var columnNameLeft = primaryColumn.Name;
						var columnNameRight = (f as MetaReference).RefClass.ColumnName((f as MetaReference).RefClass.Key.Name);
						joinTable.Columns.Add(columnNameLeft, new Column() { Table = t, Name = columnNameLeft, Nullable = false, Type = f.Type });
						joinTable.Columns.Add(columnNameRight, new Column() { Table = t, Name = columnNameRight, Nullable = false, Type = (f as MetaReference).RefClass.Key.Type });

						t.ForeignKeys.Add("FK_" + joinTable.Name + "_" + t.Name, new ForeignKey() { Table = t, Name = "FK_" + joinTable.Name + "_" + t.Name, RefTable = t.Name, Columns = new[] { columnNameLeft }, RefTableColumns = new[] { primaryColumn.Name } });
						t.ForeignKeys.Add("FK_" + joinTable.Name + "_" + (f as MetaReference).RefClass.Name, new ForeignKey() { Table = t, Name = "FK_" + joinTable.Name + "_" + (f as MetaReference).RefClass.Name, RefTable = (f as MetaReference).RefClass.Name, Columns = new[] { columnNameRight }, RefTableColumns = new[] { (f as MetaReference).RefClass.Key.Name } });

						tempListJoinTables.Add(joinTable.Name);
					}
				}
			});

			Table tdata = null;
			if (cls.IsMultilingual)
			{
				tdata = new Table();
				tdata.Schema = this;
				tdata.Name = cls.Name + "Data";
				foreach (var prop in cls.Properties)
				{
					var column = new Column();
					column.Name = prop.Name;
					column.Table = tdata;
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

				// Добавляем PK
				var columnPk = new Column();
				columnPk.Name = tdata.Name + "ID";
				columnPk.IsPrimaryKey = true;
				columnPk.Nullable = false;
				columnPk.Table = tdata;

				primaryColumn.ForeignKeyName = "FK_" + tdata.Name + "_" + t.Name; // Ссылка на базовую таблицу
				tdata.ForeignKeys.Add(primaryColumn.ForeignKeyName, new ForeignKey() { Table = tdata, Name = primaryColumn.ForeignKeyName, RefTable = "C_Language", Columns = new[] { "LanguageCode" }, RefTableColumns = new[] { "LanguageCode" } });

				var languageColumn = new Column(); // Ссылка на таблицу C_Language
				languageColumn.Name = "LanguageCode";
				languageColumn.Type = new MetaStringType() { Length = 2 };
				languageColumn.ForeignKeyName = "FK_" + tdata.Name + "_C_Language";
				languageColumn.Table = tdata;
				tdata.Columns.Add("LanguageCode", languageColumn);

				tdata.ForeignKeys.Add(languageColumn.ForeignKeyName, new ForeignKey() { Table = tdata, Name = languageColumn.ForeignKeyName, RefTable = "C_Language", Columns = new[] { "LanguageCode" }, RefTableColumns = new[] { "LanguageCode" } });

				Tables.Add(tdata.Name, tdata);

				// Создаём вьюшку
				var view = new View();
				view.Name = "V_" + cls.Name;
				view.Text = string.Format(@"CREATE view {0} as select f.*, s.*
					from {1} f JOIN {2} s ON f.{3} = s.{3}", view.Name, t.Name, tdata.Name, primaryColumn.Name);
				Views.Add(view.Name, view);
			}
		}
	}
}