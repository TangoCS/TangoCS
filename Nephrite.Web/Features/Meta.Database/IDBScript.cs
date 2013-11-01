using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Nephrite.Meta.Database
{
    public interface IDBScript
    {
        List<string> Scripts { get; set; }
        void CreateTable(Table srcTable);
        void DeleteTable(Table currentTable);
        void CreateForeignKey(ForeignKey srcforeignKey );
        void DeleteForeignKey(ForeignKey currentForeignKey );
        void DeletePrimaryKey(PrimaryKey currentPrimaryKey );
        void CreatePrimaryKey(PrimaryKey srcPrimaryKey);
        void DeleteColumn(Column currentColumn);
        void AddColumn(Column srcColumn);
        void ChangeColumn(Column srcColumn );
        void DeleteTrigger(Trigger currentTrigger);
        void CreateTrigger(Trigger srcTrigger);
		void SyncIdentity(Table srcTable);

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

		string ImportData(Table t, bool identityInsert, SqlConnection DbConnection);

    }
 
}
