using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Meta.Database
{
    public interface IDBScript
    {
        List<string> Scripts { get; set; }
        void CreateTable(Table srcTable);
        void DeleteTable(Table currentTable);
        void CreateForeignKey(ForeignKey srcforeignKey, Table currentTable);
        void DeleteForeignKey(ForeignKey currentForeignKey, Table currentTable);
        void DeletePrimaryKey(PrimaryKey currentPrimaryKey, Table currentTable);
        void CreatePrimaryKey(PrimaryKey srcPrimaryKey, Table curentTable);
        void DeleteColumn(Column currentColumn, Table currentTable);
        void AddColumn(Column srcColumn, Table currentTable , Table srcTable);
        void ChangeColumn(Column srcColumn, Table currentTable);
        void DeleteTrigger(Trigger currentTrigger);
        void CreateTrigger(Trigger srcTrigger);
        void SyncIdentity(Column currentColumn, Table currentTable, Table srcTable);

        void DeleteView(View currentView);
        void CreateView(View srcView);

        void DeleteProcedure(Procedure currentProcedure);
        void CreateProcedure(Procedure srcProcedure);
        void DeleteFunction(Function currentFunction);
        void CreateFunction(Function srcFunction);

		string GetIntType();
		string GetGuidType();
		string GetStringType(int length);
		string GetDecimalType(int precision, int scale);
		string GetDateTimeType();
		string GetDateType();
		string GetZoneDateTimeType();
		string GetLongType();
		string GetByteArrayType(int length);
		string GetBooleanType();

		string ImportData(Table t, bool identityInsert);

    }
 
}
