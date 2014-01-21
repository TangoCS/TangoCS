using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nephrite.Meta;
using Nephrite.Meta.Database;
using Nephrite.Web;
using Nephrite.Web.Model;

namespace GenerateDB2Model
{
	class Program
	{
		static void Main(string[] args)
		{
			var ss = new DB2ServerMetadataReader();
			ConnectionManager.SetConnectionString("Database=servants;UserID=db2admin;Password=q121212;Server=193.233.68.82:50000");
			//Base.Model = new DataContext(ConnectionManager.ConnectionString);
			var s = ss.ReadSchema("DBO");
			StringBuilder stringB = new StringBuilder();

			foreach (var s1 in s.Tables)
			{
				foreach (var value in s1.Value.Columns.Values)
				{
					if (value.Type is MetaGuidType)
						stringB.AppendLine("Class - " + value.CurrentTable.Name + "      Column - " + value.Name);
				}
			}

			var result = stringB.ToString();
			//string cs = "Database=servants;UserID=db2admin;Password=q121212;Server=193.233.68.82:50000";
			////new HCoreDataContext(AppWeb.DBConfig);//Solution.App.DataContext;
			//var d = new HCoreDataContext(AppWeb.DBConfig);
			//var s = MetaSolution.Load();
		}
	}
}
