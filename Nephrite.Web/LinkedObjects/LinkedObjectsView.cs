using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Nephrite.Http;
using Nephrite.Layout;
using Nephrite.MVC;
using Nephrite.Web.Controls;


namespace Nephrite.Web.LinkedObjects
{
	public class LinkedObjectsView
	{
		public static string RenderLinkedObjects(ILinkedObjectsProvider obj)
		{
			StringBuilder sb = new StringBuilder();
			var l = AppLayout.Current;
			sb.Append(l.ListTableBegin(new { style = "width:100%" }));
			sb.Append(l.ListHeaderBegin(null));
			sb.Append(l.THBegin(new { style = "width:150px" }));
			sb.Append("Связь");
			sb.Append(l.THEnd());
			sb.Append(l.THBegin(null));
			sb.Append("Объекты");
			sb.Append(l.THEnd());
			sb.Append(l.ListHeaderEnd());
			foreach (var link in obj.GetLinks(LinkType.All))
			{
				StringBuilder sb1 = new StringBuilder();
				bool objectsExists = false;
				var cls = link.Class;
				sb1.Append(l.ListRowBegin("", null));
				sb1.Append(l.TDBegin(new { style = "vertical-align: top" }));
				sb1.Append(cls.Caption + ", " + link.Property.Caption);
				sb1.Append(l.TDEnd());
				sb1.Append(l.TDBegin(null));
				var oplist = cls.Operations;
				var op = oplist.FirstOrDefault(o => o.Name == "Edit" || o.Name == "View");
				foreach (var linkedobj in link.VisibleObjects)
				{
					if (op == null)
						sb1.Append(HttpUtility.HtmlEncode(linkedobj.Title));
					else
					{
						HtmlParms p = new HtmlParms();
						p.Add("oid", linkedobj.ObjectID > 0 ? linkedobj.ObjectID.ToString() : linkedobj.ObjectGUID.ToString());
						//sb1.Append(ActionLink.To(op).With(p, true).Link(linkedobj.Title));
					}
					sb1.Append("<br />");
					objectsExists = true;
				}
				sb1.Append(l.TDEnd());
				sb1.Append(l.ListRowEnd());
				if (objectsExists)
					sb.Append(sb1);
			}

			sb.Append(l.ListTableEnd());
			return sb.ToString();
		}
	}
}