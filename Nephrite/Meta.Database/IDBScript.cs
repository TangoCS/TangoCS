using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nephrite.Meta.Database
{
	public interface IDBScript
	{
		//List<string> Scripts { get; set; }
		void Comment(string comment);
		void CreateTable(Table srcTable);
		void DeleteTable(Table currentTable);
		void CreateForeignKey(ForeignKey srcforeignKey);
		void DeleteForeignKey(ForeignKey currentForeignKey);
		void DeletePrimaryKey(PrimaryKey currentPrimaryKey);
		void CreatePrimaryKey(PrimaryKey srcPrimaryKey);
		void DeleteColumn(Column currentColumn);
		void AddComputedColumn(Column srcColumn);
		void ChangeColumn(Column srcColumn);
		void DeleteTrigger(Trigger currentTrigger);
		void CreateTrigger(Trigger srcTrigger);
		void SyncIdentity(Table srcTable);
		void AddColumn(Column srcColumn);
		void DeleteView(View currentView);
		void CreateView(View srcView);
		XElement GetMeta(string connectionString);
		void DeleteProcedure(Procedure currentProcedure);
		void CreateProcedure(Procedure srcProcedure);
		void DeleteFunction(Function currentFunction);
		void CreateFunction(Function srcFunction);
		void DeleteTableFunction(TableFunction currentFunction);
		void CreateTableFunction(TableFunction srcFunction);
		void DeleteDefaultValue(Column currentColumn);
		void AddDefaultValue(Column srcColumn);
		void DeleteIndex(Index currentIndex);
		void SyncIdentityColumn(Column srcColumn);

		string ToString();
	}
}
