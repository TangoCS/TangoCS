using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nephrite.Meta;
using Nephrite.Meta.Database;
using Nephrite.Web;
using System.Data.Linq;

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
			var schema = new Schema();
			foreach (var d in s.Classes)
			{
				schema.Generate(d);
			}
			foreach (var table in schema.Tables)
			{
 				table.
			}
		}
	}
}
