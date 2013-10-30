using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nephrite.Meta;
using Nephrite.Meta.Database;
using Nephrite.Web;
using System.Data.Linq;
using System.Linq;
using Nephrite.Meta.Database;

namespace TestSchema
{
	[TestClass]
	public class TestSchema
	{
		[TestMethod]
		public void TestMethod1()
		{
			string cs = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=servants;Data Source=TOSHIBA-TOSH\\SQL2008";
			Base.Model = new DataContext(cs);
			var s = MetaSolution.Load();
			var srcSchema = new Schema();
			foreach (var d in s.Classes)
			{
				srcSchema.Generate(d);
			}
			var ownSchema =  new SqlServerMetadataReader().ReadSchema("dbo");
			var dbScript = new DBScriptMSSQL();
			foreach (var table in ownSchema.Tables)
			{
				var srcTable = srcSchema.Tables.Values.SingleOrDefault(t=>t.Name==table.Key);
				 table.Value.Sync(dbScript , srcTable);
				
			}
			//foreach (var table in schema.Tables)
			//{
			//	table.
			//}
		}
	}
}
