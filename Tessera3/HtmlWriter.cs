using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Tessera3Sample
{
	public class HtmlWriter : StringWriter
	{
		public void DocType()
		{
			WriteLine("<!DOCTYPE html>");
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