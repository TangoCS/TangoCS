using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nephrite.Meta.Database;

namespace Nephrite.Meta.Database
{
    public partial class Procedure
    {
        public void Sync(IDBScript script, Procedure srcProcedure)
        {
            if (srcProcedure == null)
            {
                script.DeleteProcedure(this);
            }
            else
            {
                if (srcProcedure.Text != this.Text)
                {
                    script.DeleteProcedure(this);
                    script.CreateProcedure(srcProcedure);
                }
            }
        }
    }

    public partial class Function
    {
        public void Sync(IDBScript script, Function srcFunction)
        {
            if (srcFunction == null)
            {
                script.DeleteFunction(this);
            }
            else
            {
                if (srcFunction.Text != this.Text)
                {
                    script.DeleteFunction(this);
                    script.CreateFunction(srcFunction);
                }
            }
        }
    }

    public partial class View
    {

        public void Sync(IDBScript script, View srcView)
        {
            if (srcView == null)
            {
                script.DeleteView(this);
            }
            else
            {
                if (srcView.Text != this.Text)
                {
                    script.DeleteView(this);
                    script.CreateView(srcView);
                }

                var currentTriggers = this.Triggers;
                var srcTriggers = srcView.Triggers;

                foreach (var currentTrigger in currentTriggers)
                {
                    var srcTrigger = srcTriggers.Values.SingleOrDefault(t => t.Name == currentTrigger.Value.Name);
                    currentTrigger.Value.Sync(script, srcTrigger);
                }


                foreach (var srcTrigger in srcTriggers.Where(srcTrigger => currentTriggers.All(t => t.Value.Name != srcTrigger.Value.Name)))
                {
                    script.CreateTrigger(srcTrigger.Value);
                }

            }
        }
    }

    public partial class Table
    {
        public void Sync(IDBScript script, Table srcTable)
        {

            if (srcTable == null)
            {
                // Создали таблицу со всеми колонками и ключами
                script.DeleteTable(this);
            }
            else
            {

                //1 Обновляем колонки 
                var curentColumns = this.Columns;
                var srcColumns = srcTable.Columns;
                //1.1. Добавляем колонки 
                foreach (var srcColumn in srcColumns.Where(srcColumn => curentColumns.All(t => t.Value.Name.ToLower() != srcColumn.Value.Name.ToLower())))
                {
                    script.AddColumn(srcColumn.Value);
                }
                //1.2. Удаляем колонки и синхронизируем 

                // Если есть Computed колонки удаляем 

                //var computedColumns = srcTable.Columns.Values.Where(t => !string.IsNullOrEmpty(t.ComputedText));
                //    foreach (var column in computedColumns)
                //    {
                //        script.DeleteColumn(column);
                //    }
                

                foreach (var column in curentColumns) //.Where(t=>string.IsNullOrEmpty(t.Value.ComputedText)))
                {
                    var srcColumn = srcColumns.Values.SingleOrDefault(t => t.Name.ToLower() == column.Value.Name.ToLower());
                    column.Value.srcTable = srcTable;
                    column.Value.Sync(script, srcColumn);

                }
                // Если есть Computed колонки создааём 
                //foreach (var column in this.Columns.Values.Where(t => !string.IsNullOrEmpty(t.ComputedText)))
                //{
                //    script.AddColumn(column);
                //}
                
                //3 Обновляем primaryKey
                if ((PrimaryKey == null || PrimaryKey.Columns.Count() == 0) && srcTable.PrimaryKey != null && srcTable.PrimaryKey.Columns.Count() > 0)
                    script.CreatePrimaryKey(srcTable.PrimaryKey);
                else
                {
                    if (PrimaryKey != null && PrimaryKey.Columns.Any())
                        PrimaryKey.Sync(script, srcTable.PrimaryKey);
                }



                //2.  Обновляем foreignKey
                var currentForeignKeys = this.ForeignKeys;
                var srcForeignKeys = srcTable.ForeignKeys;
                //2.1. Удаляем foreignKey
                foreach (var curentforeignKey in currentForeignKeys.Where(curentforeignKey => srcForeignKeys.All(t => t.Key.ToLower() != curentforeignKey.Key.ToLower())))
                {
                    script.DeleteForeignKey(curentforeignKey.Value);
                }
                //2.2. Добаляем foreignKey
                foreach (var srcforeignKey in srcForeignKeys.Where(srcforeignKey => currentForeignKeys.All(t => t.Key.ToLower() != srcforeignKey.Key.ToLower())))
                {
                    // Проверяем есть ли таблица в методанных
                    if (srcTable.Schema.Tables.Values.Any(t => srcforeignKey.Value.RefTable.ToLower() == t.Name.ToLower()))
                        script.CreateForeignKey(srcforeignKey.Value);
                }




                if (script.GetType().ToString() == "Nephrite.Meta.Database.DBScriptDB2")
                    return;
                //4 Обновляем trigger
                var currentTriggers = this.Triggers;
                var srcTriggers = srcTable.Triggers;
                //4.1. Удаляем trigger и синхронизируем
                foreach (var currentTrigger in currentTriggers)
                {
                    var srcTrigger = srcTriggers.Values.SingleOrDefault(t => t.Name.ToLower() == currentTrigger.Value.Name.ToLower());
                    currentTrigger.Value.Sync(script, srcTrigger);
                }

                //4.2. Добавляем trigger 
                foreach (var srcTrigger in srcTriggers.Where(srcTrigger => currentTriggers.All(t => t.Value.Name.ToLower() != srcTrigger.Value.Name.ToLower())))
                {
                    script.CreateTrigger(srcTrigger.Value);
                }
            }
        }

    }

    public partial class Column
    {

        public Table srcTable { get; set; }
        public void Sync(IDBScript script, Column srcColumn)
        {
            if (srcColumn == null)
            {
                script.DeleteColumn(this);
            }
            else
            {
				bool chg_type = srcColumn.Type.GetType() != Type.GetType();
				bool chg_def = !Identity && ((DefaultValue == null ? "" : DefaultValue.ToLower()) != (srcColumn.DefaultValue == null ? "" : srcColumn.DefaultValue.ToLower()));
                bool chg_null = Nullable != srcColumn.Nullable;
				bool chg_comp = ((ComputedText == null ? "" : ComputedText.ToLower()) != (srcColumn.ComputedText == null ? "" : srcColumn.ComputedText.ToLower()));
				
				// Обновляем Type, значение Default и Nullable
				if (chg_def)
				{
					//script.Comment(String.Format("Обновление столбца: DEF={0} -> {1}",
					//		srcColumn.DefaultValue == null ? "" : srcColumn.DefaultValue.ToLower(),
					//		DefaultValue == null ? "" : DefaultValue.ToLower()));
					script.DeleteDefaultValue(srcColumn);
					script.AddDefaultValue(srcColumn);
				}

				if (chg_type || chg_null || chg_comp)
                {
					//script.Comment(String.Format("Обновление столбца: TYPE={0}, DEFAULT={1}, NULLABLE={2}, EXPRESSION={3}", chg_type, chg_def, chg_null, chg_comp));
					

                    if (!string.IsNullOrEmpty(DefaultValue))
                    {
                        script.DeleteDefaultValue(this);
                    }
                    script.ChangeColumn(srcColumn);
                }

                if (IsPrimaryKey && Table.Identity != srcTable.Identity)
                {
					script.Comment(String.Format("у таблицы {0} identity не совпадает", Table.Name));
					/*
                    //Находим таблицы ссылающиеся на текущую и у даляем их
                    var childrenForeignKeys = CurrentTable.Schema.Tables.Where(t => t.Value.ForeignKeys.Any(f => f.Value.RefTable.ToLower() == CurrentTable.Name.ToLower())).SelectMany(t => t.Value.ForeignKeys).ToList();
                    if (CurrentTable.PrimaryKey != null)
                    {
                        foreach (var foreignKey in childrenForeignKeys)
                        { script.DeleteForeignKey(foreignKey.Value); }
                    }
                    // Удаляем ссылки pk fk так же обнуляем их обьекты и таблицы для создания их в дальнейшем
                    if (CurrentTable.PrimaryKey.Columns.Any(t => t == this.Name.ToLower()))
                    {
                        script.DeletePrimaryKey(CurrentTable.PrimaryKey);
                    }
                    var toRemove = CurrentTable.ForeignKeys.Select(t => t.Key).ToArray();
                    var listUpperFk = CurrentTable.ForeignKeys;
                    listUpperFk.ToList().ForEach(t => t.Key.ToUpper());
                    foreach (var key in toRemove)
                    {
                        var fk = listUpperFk[key.ToUpper()];
                        script.DeleteForeignKey(fk);
                        var rFk = listUpperFk.FirstOrDefault(t => t.Key == key.ToUpper());
                        CurrentTable.ForeignKeys.ToList().Remove(rFk);
                    }

                    script.SyncIdentity(srcTable);
                    //Возвращаем PK 
                    if (CurrentTable.PrimaryKey != null)
                    {
                        script.CreatePrimaryKey(CurrentTable.PrimaryKey);
                    }
                    //Возвращаем FK Children
                    if (CurrentTable.PrimaryKey != null)
                    {
                        foreach (var foreignKey in childrenForeignKeys)
                        { script.CreateForeignKey(foreignKey.Value); }
                    }
					*/
                }

            }
        }

    }
    public partial class Trigger
    {
        public void Sync(IDBScript script, Trigger srcTrigger)
        {
            if (srcTrigger == null)
            {
                script.DeleteTrigger(this);
            }
            else
            {
                // Обновляем Type, значение Default и Nullable
                if (Text.ToLower() != srcTrigger.Text.ToLower())
                {
                    script.DeleteTrigger(this);
                    script.CreateTrigger(srcTrigger);
                }

            }
        }

    }
    public partial class PrimaryKey
    {

        public void Sync(IDBScript script, PrimaryKey srcPrimaryKey)
        {
            if (srcPrimaryKey == null)
            {
                script.DeletePrimaryKey(this);
            }
            else
            {
                if (Name.ToLower() != srcPrimaryKey.Name.ToLower())
                {
                    script.DeletePrimaryKey(this);
                    script.CreatePrimaryKey(srcPrimaryKey);
                }
            }
        }

    }
}
