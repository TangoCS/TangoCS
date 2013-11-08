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
				// Создали таблицу со всемми колонками и ключами
				script.DeleteTable(this);
			}
			else
			{

				//1 Обновляем колонки 
				var curentColumns = this.Columns;
				var srcColumns = srcTable.Columns;
				//1.1. Добавляем колонки 
				foreach (var srcColumn in srcColumns.Where(srcColumn => curentColumns.All(t => t.Value.Name != srcColumn.Value.Name)))
				{
					script.AddColumn(srcColumn.Value);
				}
				//1.2. Удаляем колонки и синхронизируем 
				foreach (var column in curentColumns)
				{
					var srcColumn = srcColumns.Values.SingleOrDefault(t => t.Name == column.Value.Name);
					column.Value.srcTable = srcTable;
					column.Value.Sync(script, srcColumn);

				}
			




				//2.  Обновляем foreignKey
				var currentForeignKeys = this.ForeignKeys;
				var srcForeignKeys = srcTable.ForeignKeys;
				//2.1. Удаляем foreignKey
				foreach (var curentforeignKey in currentForeignKeys.Where(curentforeignKey => srcForeignKeys.All(t => t.Key != curentforeignKey.Key)))
				{
					script.DeleteForeignKey(curentforeignKey.Value);
				}
				//2.2. Добаляем foreignKey
				foreach (var srcforeignKey in srcForeignKeys.Where(srcforeignKey => currentForeignKeys.All(t => t.Key != srcforeignKey.Key)))
				{
					script.CreateForeignKey(srcforeignKey.Value);
				}



				//3 Обновляем primaryKey
				if (PrimaryKey == null && srcTable.PrimaryKey != null)
					script.CreatePrimaryKey(srcTable.PrimaryKey);
				else
				{
					if (PrimaryKey != null)
						PrimaryKey.Sync(script, srcTable.PrimaryKey);
				}


				//4 Обновляем trigger
				var currentTriggers = this.Triggers;
				var srcTriggers = srcTable.Triggers;
				//4.1. Удаляем trigger и синхронизируем
				foreach (var currentTrigger in currentTriggers)
				{
					var srcTrigger = srcTriggers.Values.SingleOrDefault(t => t.Name == currentTrigger.Value.Name);
					currentTrigger.Value.Sync(script, srcTrigger);
				}

				//4.2. Добавляем trigger 
				foreach (var srcTrigger in srcTriggers.Where(srcTrigger => currentTriggers.All(t => t.Value.Name != srcTrigger.Value.Name)))
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
				// Обновляем Type, значение Default и Nullable
				if (Type != srcColumn.Type || DefaultValue != srcColumn.DefaultValue || Nullable != srcColumn.Nullable || ComputedText != srcColumn.ComputedText)
				{
					if (CurrentTable.Name == "C_FIAS_AddressObject")
					{

					}
					var computedColumns = CurrentTable.Columns.Values.Where(t => !string.IsNullOrEmpty(t.ComputedText));
					if (string.IsNullOrEmpty(ComputedText))
					{
						foreach (var column in computedColumns)
						{
							script.DeleteColumn(column);
						}
					}
					script.ChangeColumn(srcColumn);

					if (string.IsNullOrEmpty(ComputedText))
					{
						foreach (var column in computedColumns)
						{
							script.AddColumn(column);
						}
					}
				}
				if (IsPrimaryKey && CurrentTable.Identity != srcTable.Identity)
				{


					//Находим таблицы ссылающиеся на текущую и у даляем их
					var childrenForeignKeys = CurrentTable.Schema.Tables.Where(t => t.Value.ForeignKeys.Any(f => f.Value.RefTable == CurrentTable.Name)).SelectMany(t => t.Value.ForeignKeys).ToList();
					if (CurrentTable.PrimaryKey != null)
					{
						foreach (var foreignKey in childrenForeignKeys)
						{ script.DeleteForeignKey(foreignKey.Value); }
					}
					// Удаляем ссылки pk fk так же обнуляем их обьекты и таблицы для создания их в дальнейшем
					if (CurrentTable.PrimaryKey.Columns.Any(t => t == this.Name))
					{
						script.DeletePrimaryKey(CurrentTable.PrimaryKey);
					}
					var toRemove = CurrentTable.ForeignKeys.Select(t => t.Key).ToArray();
					foreach (var key in toRemove)
					{

						script.DeleteForeignKey(CurrentTable.ForeignKeys[key]);
						CurrentTable.ForeignKeys.Remove(key);
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
				if (Text != srcTrigger.Text)
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
				if (Name != srcPrimaryKey.Name)
				{
					script.DeletePrimaryKey(this);
					script.CreatePrimaryKey(srcPrimaryKey);
				}
			}
		}

	}
}
