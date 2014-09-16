using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Meta.Database
{
	class DBScriptPostgreSQL : IDBScript
	{
		public void Comment(string comment)
		{
			throw new NotImplementedException();
		}

		public void CreateTable(Table srcTable)
		{
			throw new NotImplementedException();
		}

		public void DeleteTable(Table currentTable)
		{
			throw new NotImplementedException();
		}

		public void CreateForeignKey(ForeignKey srcforeignKey)
		{
			throw new NotImplementedException();
		}

		public void DeleteForeignKey(ForeignKey currentForeignKey)
		{
			throw new NotImplementedException();
		}

		public void DeletePrimaryKey(PrimaryKey currentPrimaryKey)
		{
			throw new NotImplementedException();
		}

		public void CreatePrimaryKey(PrimaryKey srcPrimaryKey)
		{
			throw new NotImplementedException();
		}

		public void DeleteColumn(Column currentColumn)
		{
			throw new NotImplementedException();
		}

		public void AddComputedColumn(Column srcColumn)
		{
			throw new NotImplementedException();
		}

		public void ChangeColumn(Column srcColumn)
		{
			throw new NotImplementedException();
		}

		public void DeleteTrigger(Trigger currentTrigger)
		{
			throw new NotImplementedException();
		}

		public void CreateTrigger(Trigger srcTrigger)
		{
			throw new NotImplementedException();
		}

		public void SyncIdentity(Table srcTable)
		{
			throw new NotImplementedException();
		}

		public void AddColumn(Column srcColumn)
		{
			throw new NotImplementedException();
		}

		public void DeleteView(View currentView)
		{
			throw new NotImplementedException();
		}

		public void CreateView(View srcView)
		{
			throw new NotImplementedException();
		}

		public System.Xml.Linq.XElement GetMeta()
		{
			throw new NotImplementedException();
		}

		public void DeleteProcedure(Procedure currentProcedure)
		{
			throw new NotImplementedException();
		}

		public void CreateProcedure(Procedure srcProcedure)
		{
			throw new NotImplementedException();
		}

		public void DeleteFunction(Function currentFunction)
		{
			throw new NotImplementedException();
		}

		public void CreateFunction(Function srcFunction)
		{
			throw new NotImplementedException();
		}

		public void DeleteDefaultValue(Column currentColumn)
		{
			throw new NotImplementedException();
		}

		public void AddDefaultValue(Column srcColumn)
		{
			throw new NotImplementedException();
		}

		public void DeleteIndex(Index currentIndex)
		{
			throw new NotImplementedException();
		}

		public void SyncIdentityColumn(Column srcColumn)
		{
			throw new NotImplementedException();
		}

		public string GetIntType()
		{
			throw new NotImplementedException();
		}

		public string GetGuidType()
		{
			throw new NotImplementedException();
		}

		public string GetStringType(int length)
		{
			throw new NotImplementedException();
		}

		public string GetDecimalType(int precision, int scale)
		{
			throw new NotImplementedException();
		}

		public string GetDateTimeType()
		{
			throw new NotImplementedException();
		}

		public string GetDateType()
		{
			throw new NotImplementedException();
		}

		public string GetZoneDateTimeType()
		{
			throw new NotImplementedException();
		}

		public string GetLongType()
		{
			throw new NotImplementedException();
		}

		public string GetByteArrayType(int length)
		{
			throw new NotImplementedException();
		}

		public string GetBooleanType()
		{
			throw new NotImplementedException();
		}

		public string GetXmlType()
		{
			throw new NotImplementedException();
		}

		public MetaPrimitiveType GetType(string dataType, bool notNull)
		{
			throw new NotImplementedException();
		}

	}
}
