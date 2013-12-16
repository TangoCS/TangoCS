using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Xml;
using Nephrite.Metamodel.Model;

namespace Nephrite.Metamodel.Controllers
{
    [ControllerControlsPath("/_controltemplates/Nephrite.Metamodel/")]
    public class UtilsController : BaseController
    {
		//public void Utils()
		//{
		//    RenderView("utils");
		//}

        public void GenerateDB()
        {
            RenderView("generatedb");
        }

        public void GenerateWsdl()
        {
            RenderView("generatewsdl");
        }

		//public void DbReport()
		//{
		//    RenderView("dbreport");
		//}

		//public void DbReportWord()
		//{
		//    RenderWordDoc("dbreport", AppWeb.AppNamespace + "_dbreport.doc");
		//}

		//public void SpmTable()
		//{
		//	RenderView("spmtable");
		//}

		public void TableDesc()
		{
			RenderView("tabledesc");
		}

		public void Docx2MM()
		{
			RenderView("docx2mm");
		}

		public void RunUp()
		{
			RenderView("runup");
		}
    }
}
