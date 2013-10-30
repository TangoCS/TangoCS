using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Meta.Database
{
	public partial class Schema
	{
		public void Generate(MetaClass cls)
		{

			var tempListJoinTables = new List<string>();
			var dbScript = new DBScriptMSSQL();
			//if (!cls.IsPersistent) return;
			if (cls.Name == "ErrorLog")
			{
			
			}
			Table t = new Table();
			t.Name = cls.Name;
			t.Description = cls.Caption;
			t.Identity = cls.CompositeKey.Count > 0 ? cls.CompositeKey.Count > 1 ? cls.CompositeKey.Where(c => c is MetaAttribute).Any(a => (a as MetaAttribute).IsIdentity) : cls.Key is MetaAttribute && (cls.Key as MetaAttribute).IsIdentity : false;

	
			if (cls.CompositeKey.Count > 0)
			{
				PrimaryKey pk = new PrimaryKey();
				pk.Name = "PK_" + cls.Name.Trim();
				pk.Columns = cls.CompositeKey.Select(o => o.Name).ToArray();
				t.PrimaryKey = pk;
			}
			Tables.Add(t.Name, t);


			var primaryColumn = new Column();

			foreach (var prop in cls.Properties)
			{
				if (prop is MetaComputedAttribute)
					continue;
				var column = new Column();
				column.Name = prop.Name;
				column.Type = prop.Type != null ? prop.Type.GetDBType(dbScript) : "";
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
						column.Type = (prop as MetaReference).RefClass.Key.Type.GetDBType(dbScript);

						column.ForeignKeyName = "FK_" + cls.Name + "_" + (prop as MetaReference).RefClassName;
					}

				}
				if (prop.Type is MetaFileType)
				{
					column.Name = prop.Name + "GUID"; 
					column.Type = dbScript.GetGuidType();
					column.ForeignKeyName = "FK_" + cls.Name + "_" + prop.Name;
					t.ForeignKeys.Add(column.ForeignKeyName, new ForeignKey() { Name = column.ForeignKeyName, RefTable = "N_File", Columns = new[] { column.Name}, RefTableColumns = new[] { "Guid" } });
				}



				column.Nullable = !prop.IsRequired;
				column.IsPrimaryKey = cls.CompositeKey.Any(p => p.Name == prop.Name);

				if (column.IsPrimaryKey)
				{
					primaryColumn = column;
				}

				t.Columns.Add(column.Name, column);
			}

			cls.Properties.Where(f => f is MetaReference).ToList().ForEach(f =>
			{
				if (f.UpperBound == 1) //Если у свойства мощность 0..1 или 1..1 Создаём FK
				{
					var metaReference = f as MetaReference;
					t.ForeignKeys.Add("FK_" + cls.Name + "_" + f.Name, new ForeignKey() { Name = "FK_" + cls.Name + "_" + f.Name, RefTable = metaReference.RefClassName, Columns = new[] { metaReference.RefClass.ColumnName(metaReference.Name) }, RefTableColumns = new[] { metaReference.RefClass.ColumnName(metaReference.Name) } });
				}

				if (f.UpperBound == -1 && (f as MetaReference).RefClass.Key.UpperBound == -1)//Если у свойства мощность 0..* или 1..* Кроме случая, когда есть обратное свойство с мощностью 0..1 Создаём третью таблицу с двумя колонками
				{
					if (!tempListJoinTables.Contains(t.Name + f.Name) && !tempListJoinTables.Contains(f.Name + t.Name))
					{
						var joinTable = new Table();
						joinTable.Name = t.Name + f.Name;
						var columnNameLeft = primaryColumn.Name;
						var columnNameRight = (f as MetaReference).RefClass.ColumnName((f as MetaReference).RefClass.Key.Name);
						joinTable.Columns.Add(columnNameLeft, new Column() { Name = columnNameLeft, Nullable = false, Type = f.Type.GetDBType(dbScript) });
						joinTable.Columns.Add(columnNameRight, new Column() { Name = columnNameRight, Nullable = false, Type = (f as MetaReference).RefClass.Key.Type.GetDBType(dbScript) });

						t.ForeignKeys.Add("FK_" + joinTable.Name + "_" + t.Name, new ForeignKey() { Name = "FK_" + joinTable.Name + "_" + t.Name, RefTable = t.Name, Columns = new[] { columnNameLeft }, RefTableColumns = new[] { primaryColumn.Name } });
						t.ForeignKeys.Add("FK_" + joinTable.Name + "_" + (f as MetaReference).RefClass.Name, new ForeignKey() { Name = "FK_" + joinTable.Name + "_" + (f as MetaReference).RefClass.Name, RefTable = (f as MetaReference).RefClass.Name, Columns = new[] { columnNameRight }, RefTableColumns = new[] { (f as MetaReference).RefClass.Key.Name } });

						tempListJoinTables.Add(joinTable.Name);
					}
				}

			});

			Table tdata = null;
			if (cls.IsMultilingual)
			{
				tdata = new Table();
				tdata.Name = cls.Name + "Data";
				foreach (var prop in cls.Properties)
				{
					var column = new Column();
					column.Name = prop.Name;

					if (prop is MetaComputedAttribute)
					{
						column.ComputedText = (prop as MetaComputedAttribute).GetExpression;
					}

					column.Type = prop.Type.GetDBType(dbScript);
					if (prop.Type is MetaFileType)
					{
						column.Name = prop.Name + "GUID";
						column.Type = dbScript.GetGuidType();
					}
					column.Nullable = !prop.IsRequired;
					//column.IsPrimaryKey = cls.CompositeKey.Any(p => p.Name == prop.Name);



					tdata.Columns.Add(column.Name, column);
				}

				// Добавляем PK
				var columnPk = new Column();
				columnPk.Name = tdata.Name + "ID";
				columnPk.IsPrimaryKey = true;
				columnPk.Nullable = false;




				primaryColumn.ForeignKeyName = "FK_" + tdata.Name + "_" + t.Name; // Ссылка на базовую таблицу
				tdata.ForeignKeys.Add(primaryColumn.ForeignKeyName, new ForeignKey() { Name = primaryColumn.ForeignKeyName, RefTable = "C_Language", Columns = new[] { "LanguageCode" }, RefTableColumns = new[] { "LanguageCode" } });



				var languageColumn = new Column();// Ссылка на таблицу C_Language
				languageColumn.Name = "LanguageCode";
				languageColumn.Type = dbScript.GetStringType(2);
				languageColumn.ForeignKeyName = "FK_" + tdata.Name + "_C_Language";
				tdata.Columns.Add("LanguageCode", languageColumn);

				tdata.ForeignKeys.Add(languageColumn.ForeignKeyName, new ForeignKey() { Name = languageColumn.ForeignKeyName, RefTable = "C_Language", Columns = new[] { "LanguageCode" }, RefTableColumns = new[] { "LanguageCode" } });

				Tables.Add(tdata.Name, tdata);

				// Создаём вьюшку
				var view = new View();
				view.Name = "V_" + cls.Name;
				view.Text = string.Format(@"CREATE view {0} as
								select f.*, s.*
								from {1} f JOIN {2} s ON f.{3} = s.{3}", view.Name, t.Name, tdata.Name, primaryColumn.Name);
				Views.Add(view.Name, view);

			}


		}
	}
}