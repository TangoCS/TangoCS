using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Html
{
	public partial class HtmlControl : StringWriter
	{
		public void DocType()
		{
			WriteLine("<!DOCTYPE html>");
		}

		void A(Action<ATagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("a");
			if (attributes != null) attributes(new ATagAttributes(tb)); 
			if (inner != null) inner();
			Write(tb);
		}

		public void Html(Action inner = null)
		{
			WriteLine("<html>");
			if (inner != null) inner();
			WriteLine("</html>");
		}

		public void Head(Action inner = null)
		{
			WriteLine("<head>");
			if (inner != null) inner();
			WriteLine("</head>");
		}

		public void Body(Action inner = null)
		{
			WriteLine("<body>");
			if (inner != null) inner();
			WriteLine("</body>");
		}
	}
}