using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;

namespace Nephrite.Web.FormsEngine
{
    public class TemplateType
    {
		public const string SiteView = "S";
		public const string Ascx = "A";
		public const string Aspx = "P";
		public const string Asmx = "M";
		public const string Ashx = "H";
		public const string Svc = "C";

		public static string SiteViewTitle { get { return "Представление сайта"; } }
        public static string AscxTitle { get { return "Ascx"; } }
		public static string AspxTitle { get { return "Aspx"; } }
		public static string AsmxTitle { get { return "Asmx"; } }
		public static string AshxTitle { get { return "Ashx"; } }
		public static string SvcTitle { get { return "Svc"; } }
        
        protected static List<CodifierValue> _list = null;
        public static List<CodifierValue> ToList()
        {
            if (TemplateType._list == null)
            {
                TemplateType._list = new List<CodifierValue> { 
					new CodifierValue { Code = TemplateType.SiteView, Title = TemplateType.SiteViewTitle },
					new CodifierValue { Code = TemplateType.Ascx, Title = TemplateType.AscxTitle },
					new CodifierValue { Code = TemplateType.Aspx, Title = TemplateType.AspxTitle },
					new CodifierValue { Code = TemplateType.Asmx, Title = TemplateType.AsmxTitle },
					new CodifierValue { Code = TemplateType.Ashx, Title = TemplateType.AshxTitle },
					new CodifierValue { Code = TemplateType.Svc, Title = TemplateType.SvcTitle }
				};
            }
            return TemplateType._list;
        }

		public static string Title(string code)
        {
            CodifierValue res = ToList().Where(o => o.Code == code).SingleOrDefault();
            return res != null ? res.Title : "";
        }
    }
}
