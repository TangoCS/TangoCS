using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Nephrite.Web
{
    public class Cramb
    {
        List<string> titles = new List<string>();
        List<string> url = new List<string>();

        private Cramb() { }
        static Cramb instance
        {
            get
            {
                if (HttpContext.Current.Items["Cramb"] == null)
                    HttpContext.Current.Items["Cramb"] = new Cramb();
                return (Cramb)HttpContext.Current.Items["Cramb"];
            }
        }

		public static string Render()
		{
			return Render(">");
		}
        public static string Render(string delimiter)
        {
            Cramb c = instance;
            if (c.titles.Count == 0)
                return "&nbsp;";
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < c.titles.Count; i++)
            {
                if (i > 0)
                    res.Append(" ").Append(delimiter).Append(" ");
                if (c.url[i] == "")
                    res.Append(c.titles[i]);
                else
                    res.AppendFormat("<a href='{0}'>{1}</a>", c.url[i], c.titles[i]);
            }
            return res.ToString();
        }

        public static void Add(string title, string url)
        {
            instance.titles.Add(title);
            instance.url.Add(url);
        }

        public static void Add(string title)
        {
            instance.titles.Add(title);
            instance.url.Add("");
        }
    }
}
