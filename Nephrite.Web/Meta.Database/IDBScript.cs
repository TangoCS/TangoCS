using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml;
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
		XElement GetMeta();
		void DeleteProcedure(Procedure currentProcedure);
		void CreateProcedure(Procedure srcProcedure);
		void DeleteFunction(Function currentFunction);
		void CreateFunction(Function srcFunction);
		void DeleteDefaultValue(Column currentColumn);
		void AddDefaultValue(Column srcColumn);
		void DeleteIndex(Index currentIndex);
		void SyncIdentityColumn(Column srcColumn);

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
		string GetXmlType();

		string ImportData(Table t, bool identityInsert, SqlConnection DbConnection);
		MetaPrimitiveType GetType(string dataType);

		string ToString();
	}

}
